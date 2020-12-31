using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LaunchQTE : MonoBehaviour
{

    #region --------------------    Public Enumerations



    #endregion

    #region --------------------    Public Events



    #endregion

    #region --------------------    Public Properties



    #endregion

    #region --------------------    Public Methods

    /// <summary>
    /// Completed event
    /// </summary>
    public void CompleteEvent()
    {
        _complete = true;
        _Unsubscribe();
        _rocket.DOLocalMoveY(0.002f, 3f).SetEase(Ease.OutQuad);
        _smoke.Play();
        GameManager.playerCountry.CompleteLaunch(true);
        /// TODO:   Play a sound
    }

    /// <summary>
    /// Released too early
    /// </summary>
    public void FailEvent()
    {
        gameObject.SetActive(false);
        _Unsubscribe();
        GameManager.playerCountry.CompleteLaunch(false);
        /// TODO:   Play a sound
    }

    #endregion

    #region --------------------    Private Fields

    private float _timerTime = 3f;
    private float _progressTime = 1f;
    private bool _complete = false;

    [SerializeField] private Transform _rocket = null;
    [SerializeField] private ParticleSystem _smoke = null;

    #endregion

    #region --------------------    Private Methods

    /// <summary>
    /// Subscribes to the progress bars and updates the modifiers
    /// </summary>
    private void OnEnable()
    {
        //  Reset
        _complete = false;
        _rocket.localPosition = Vector3.zero;
        _smoke.Clear();

        GameManager.instance.OnProgressFullEvent += CompleteEvent;
        GameManager.instance.OnTimerEmptyEvent += FailEvent;

        //  Begins countdown
        GameManager.instance.timerMod = (-1 / _timerTime) / (3 - (int)GameManager.difficulty);
    }

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