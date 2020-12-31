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
        _rocket.gameObject.SetActive(true);
        _rocket.localPosition = Vector3.zero;
        _smoke.Clear();

        GameManager.instance.OnProgressFullEvent += CompleteEvent;
        GameManager.instance.OnTimerEmptyEvent += FailEvent;

        //  Begins countdown
        GameManager.instance.timerMod = (-1 / _timerTime) / (3 - (int)GameManager.difficulty);
    }

    /// <summary>
    /// Plays the rocket animation
    /// </summary>
    public void RocketAnim()
    {
        /// TODO:   Play a sound
        _rocket.gameObject.SetActive(true);
        _rocket.localPosition = Vector3.zero;
        _smoke.Clear();
        _rocket.DOLocalMoveY(0.002f, 3f).SetEase(Ease.InQuad).OnComplete(() => { rocket.gameObject.SetActive(false); });
        _smoke.Play();
    }

    /// <summary>
    /// Completed event
    /// </summary>
    public void CompleteEvent()
    {
        _complete = true;
        _Unsubscribe();
        if (_action == ActionLabel.GameAction.Launch)
        {
            GameManager.playerCountry.CompleteLaunch(true);
        }
        else
        {
            GameManager.playerCountry.CompleteTest(true);
        }
    }

    /// <summary>
    /// Released too early
    /// </summary>
    public void FailEvent()
    {
        _Unsubscribe();
        if (_action == ActionLabel.GameAction.Launch)
        {
            GameManager.playerCountry.CompleteLaunch(false);
        }
        else
        {
            GameManager.playerCountry.CompleteTest(false);
        }
        _rocket.gameObject.SetActive(false);
        /// TODO:   Play a sound
    }

    #endregion

    #region --------------------    Private Fields

    private float _timerTime = 3f;
    private float _progressTime = 1f;
    private bool _complete = false;

    [SerializeField] private ActionLabel.GameAction _action = ActionLabel.GameAction.Launch;
    [SerializeField] private Transform _rocket = null;
    [SerializeField] private ParticleSystem _smoke = null;

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
        GameManager.instance.progressMod = (1 / _progressTime);
    }

    /// <summary>
    /// If released too early
    /// </summary>
    private void OnMouseUp()
    {
        if (!_complete)
        {
            GameManager.instance.timerMod = 5f;
            GameManager.instance.progressMod = -5f;
            FailEvent();
        }
    }

    #endregion

}