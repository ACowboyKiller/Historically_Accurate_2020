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
        _complete = false;
        _flask.gameObject.SetActive(true);
        _chemicals.Clear();

        GameManager.instance.OnProgressFullEvent += CompleteEvent;
        GameManager.instance.OnTimerEmptyEvent += FailEvent;

        //  Begins countdown
        _isActive = true;
        GameManager.instance.timerMod = (-1 / _timerTime) / (3 - (int)GameManager.difficulty);

        //  Determine click count
        _clickCount = Random.Range(2f, 6f);
    }

    /// <summary>
    /// Plays the flask animation
    /// </summary>
    public void FlaskAnim()
    {
        /// TODO:   Play a sound
        _flask.gameObject.SetActive(true);
        _flask.localPosition = Vector3.zero;
        _chemicals.Clear();
        _flask.DOLocalMoveY(0f, 3f).OnComplete(() => { _flask.gameObject.SetActive(false); });
        _chemicals.Play();
    }

    /// <summary>
    /// Completed event
    /// </summary>
    public void CompleteEvent()
    {
        _complete = true;
        _Unsubscribe();
        if (_action == ActionLabel.GameAction.Experiment)
        {
            GameManager.playerCountry.CompleteExperiment(true);
        }
        else
        {
            GameManager.playerCountry.CompleteReport(true);
        }
        _isActive = false;
    }

    /// <summary>
    /// Released too early
    /// </summary>
    public void FailEvent()
    {
        _Unsubscribe();
        if (_action == ActionLabel.GameAction.Experiment)
        {
            GameManager.playerCountry.CompleteExperiment(false);
        }
        else
        {
            GameManager.playerCountry.CompleteReport(false);
        }
        _flask.gameObject.SetActive(false);
        _isActive = false;
        /// TODO:   Play a sound
    }

    #endregion

    #region --------------------    Private Fields

    private bool _isActive = false;
    private float _timerTime = 3f;
    private float _clickCount = 0f;
    private bool _complete = false;

    [SerializeField] private ActionLabel.GameAction _action = ActionLabel.GameAction.Experiment;
    [SerializeField] private Transform _flask = null;
    [SerializeField] private ParticleSystem _chemicals = null;

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
    }

    #endregion

}