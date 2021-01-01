using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RocketQTE : MonoBehaviour
{

    #region --------------------    Public Enumerations



    #endregion

    #region --------------------    Public Events



    #endregion

    #region --------------------    Public Properties

    /// <summary>
    /// Returns the rocket's transform
    /// </summary>
    public Transform rocket => _rocket;

    #endregion

    #region --------------------    Public Methods

    /// <summary>
    /// Subscribes to the progress bars and updates the modifiers
    /// </summary>
    public void Init()
    {
        //  Reset
        _complete = false;
        _rocketTween?.Pause();
        _rocket.gameObject.SetActive(true);
        _rocket.localPosition = Vector3.zero;
        _smoke.Clear();

        GameManager.instance.OnProgressFullEvent += CompleteEvent;
        GameManager.instance.OnTimerEmptyEvent += FailEvent;

        //  Begins countdown
        _isActive = true;

        //  Set progress time
        _progressTime = Random.Range(2f, 3f);
        GameManager.instance.timerMod = (-1 / (_timerTime * _progressTime)) / (3 - (int)GameManager.difficulty);
    }

    /// <summary>
    /// Plays the rocket animation
    /// </summary>
    public void RocketAnim()
    {
        /// TODO:   Play a sound
        _rocketTween?.Pause();
        _rocket.gameObject.SetActive(true);
        _rocket.localPosition = Vector3.zero;
        _smoke.Clear();
        _rocketTween = _rocket.DOLocalMoveY(0.002f, 3f)
            .SetEase(Ease.InQuad)
            .OnComplete(() => { rocket.gameObject.SetActive(_isActive); });
        _smoke.Play();
    }

    /// <summary>
    /// Completed event
    /// </summary>
    public void CompleteEvent()
    {
        _complete = true;
        _Unsubscribe();
        _isActive = false;
        if (_action == ActionLabel.GameAction.Launch)
        {
            GameManager.playerCountry.CompleteLaunch(true);
        }
        else
        {
            GameManager.playerCountry.CompleteTest(true);
        }
        GameManager.instance.EndQTE();
    }

    /// <summary>
    /// Released too early
    /// </summary>
    public void FailEvent()
    {
        _Unsubscribe();
        _isActive = false;
        if (_action == ActionLabel.GameAction.Launch)
        {
            GameManager.playerCountry.CompleteLaunch(false);
        }
        else
        {
            GameManager.playerCountry.CompleteTest(false);
        }
        _rocket.gameObject.SetActive(false);
        GameManager.instance.EndQTE();
        /// TODO:   Play a sound
    }

    #endregion

    #region --------------------    Private Fields

    private bool _isActive = false;
    private float _timerTime = 3f;
    private float _progressTime = 1f;
    private bool _complete = false;

    [SerializeField] private ActionLabel.GameAction _action = ActionLabel.GameAction.Launch;
    [SerializeField] private Transform _rocket = null;
    [SerializeField] private ParticleSystem _smoke = null;

    private DG.Tweening.Core.TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions> _rocketTween = null;

    #endregion

    #region --------------------    Private Methods

    /// <summary>
    /// Unsubscribes from the progress bars
    /// </summary>
    private void _Unsubscribe()
    {
        GameManager.instance.OnProgressFullEvent -= CompleteEvent;
        GameManager.instance.OnTimerEmptyEvent -= FailEvent;
    }

    /// <summary>
    /// Begins the launch QTE
    /// </summary>
    private void OnMouseDown()
    {
        if (!_isActive) return;
        GameManager.instance.progressMod = (1 / _progressTime);
    }

    /// <summary>
    /// If released too early
    /// </summary>
    private void OnMouseUp()
    {
        if (!_isActive) return;
        if (!_complete)
        {
            GameManager.instance.timerMod = 5f;
            GameManager.instance.progressMod = -5f;
            FailEvent();
        }
    }

    #endregion

}