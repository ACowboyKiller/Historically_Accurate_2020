using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PropagandaQTE : MonoBehaviour
{

    #region --------------------    Public Enumerations



    #endregion

    #region --------------------    Public Events



    #endregion

    #region --------------------    Public Properties

    /// <summary>
    /// Returns whether or not the qte is active
    /// </summary>
    public bool isActive => _isActive;

    /// <summary>
    /// Stores the number of towers to show
    /// </summary>
    public int towerCount { get; private set; } = 0;

    #endregion

    #region --------------------    Public Methods

    /// <summary>
    /// Subscribes to the progress bars and updates the modifiers
    /// </summary>
    public void Init()
    {
        //  Reset
        _complete = false;

        GameManager.instance.OnProgressFullEvent += CompleteEvent;
        GameManager.instance.OnTimerEmptyEvent += FailEvent;

        //  Begins countdown
        _isActive = true;
        GameManager.instance.timerMod = (-1 / _timerTime) / (3 - (int)GameManager.difficulty);

        //  Set progress time
        towerCount = Random.Range(2, _towers.Count);
        for (int i = 0; i < towerCount; i ++)
        {
            int _index = Random.Range(0, _towers.Count);
            int _break = 0;
            while (_towers[_index].gameObject.activeInHierarchy || _break < _towers.Count + 2)
            {
                _index = (_index + 1) % _towers.Count;
                _break++;
            }
            _towers[_index].gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Plays the done animation
    /// </summary>
    public void DoneAnim()
    {
        _towers.ForEach(t => t.gameObject.SetActive(false));
        /// TODO:   Play a sound
    }

    /// <summary>
    /// Completed event
    /// </summary>
    public void CompleteEvent()
    {
        _complete = true;
        _Unsubscribe();
        GameManager.playerCountry.CompletePropaganda(true);
        _isActive = false;
        _towers.ForEach(t => t.gameObject.SetActive(false));
    }

    /// <summary>
    /// Released too early
    /// </summary>
    public void FailEvent()
    {
        _Unsubscribe();
        GameManager.playerCountry.CompletePropaganda(false);
        _isActive = false;
        _towers.ForEach(t => t.gameObject.SetActive(false));
        /// TODO:   Play a sound
    }

    #endregion

    #region --------------------    Private Fields

    private bool _isActive = false;
    private float _timerTime = 5f;
    private bool _complete = false;

    [SerializeField] private List<Tower> _towers = new List<Tower>();

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

    #endregion

}