using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FlaskQTE : MonoBehaviour
{

    #region --------------------    Public Enumerations



    #endregion

    #region --------------------    Public Events



    #endregion

    #region --------------------    Public Properties

    /// <summary>
    /// Returns the flask's transform
    /// </summary>
    public Transform flask => _flask;

    #endregion

    #region --------------------    Public Methods

    /// <summary>
    /// Subscribes to the progress bars and updates the modifiers
    /// </summary>
    public void Init()
    {
        //  Reset
        _flaskTween?.Pause();
        _flask.gameObject.SetActive(true);
        _chemicals.Clear();

        GameManager.instance.OnProgressFullEvent += CompleteEvent;
        GameManager.instance.OnTimerEmptyEvent += FailEvent;

        //  Begins countdown
        _isActive = true;
        GameManager.instance.progressMod = 0f;

        //  Determine click count
        _clickCount = Random.Range(10f, 20f);
        GameManager.instance.timerMod = (-1 / (_timerTime * _clickCount)) / (3 - (int)GameManager.difficulty);
    }

    /// <summary>
    /// Plays the flask animation
    /// </summary>
    public void FlaskAnim()
    {
        /// TODO:   Play a sound
        if (_country.isPlayerControlled) SoundManager.SFX("PositiveButton");
        _flaskTween?.Pause();
        _flask.gameObject.SetActive(true);
        _flask.localPosition = Vector3.zero;
        _chemicals.Clear();
        _flaskTween = _flask.DOLocalMoveY(0f, 3f)
            .OnComplete(() => { _flask.gameObject.SetActive(_isActive); });
        _chemicals.Play();
    }

    /// <summary>
    /// Completed event
    /// </summary>
    public void CompleteEvent()
    {
        _Unsubscribe();
        _isActive = false;
        if (_action == ActionLabel.GameAction.Experiment)
        {
            GameManager.playerCountry.CompleteExperiment(true);
        }
        else
        {
            GameManager.playerCountry.CompleteReport(true);
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
        if (_action == ActionLabel.GameAction.Experiment)
        {
            GameManager.playerCountry.CompleteExperiment(false);
        }
        else
        {
            GameManager.playerCountry.CompleteReport(false);
        }
        _flask.gameObject.SetActive(false);
        GameManager.instance.EndQTE();
        if (_country.isPlayerControlled) SoundManager.SFX("NegativeButton");
        /// TODO:   Play a sound
    }

    #endregion

    #region --------------------    Private Fields

    private bool _isActive = false;
    private float _timerTime = 0.25f;
    private float _clickCount = 0f;

    [SerializeField] private Country _country = null;
    [SerializeField] private ActionLabel.GameAction _action = ActionLabel.GameAction.Experiment;
    [SerializeField] private Transform _flask = null;
    [SerializeField] private ParticleSystem _chemicals = null;

    private DG.Tweening.Core.TweenerCore<Vector3, Vector3, DG.Tweening.Plugins.Options.VectorOptions> _flaskTween = null;

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
        GameManager.instance.progressBar.percent += 1f / Mathf.Max(_clickCount, 1f);
        if (_country.isPlayerControlled) SoundManager.SFX("FlaskClick");
    }

    #endregion

}