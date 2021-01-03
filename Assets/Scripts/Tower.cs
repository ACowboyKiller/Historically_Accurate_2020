using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Tower : MonoBehaviour
{

    #region --------------------    Public Enumerations



    #endregion

    #region --------------------    Public Events



    #endregion

    #region --------------------    Public Properties



    #endregion

    #region --------------------    Public Methods



    #endregion

    #region --------------------    Private Fields

    [SerializeField] private PropagandaQTE _qte = null;
    [SerializeField] private ParticleSystem _sparks = null;
    private bool _isClickable = false;

    #endregion

    #region --------------------    Private Methods

    private void OnEnable()
    {
        _sparks.Clear();
        _isClickable = true;
    }

    /// <summary>
    /// Completes tower click
    /// </summary>
    private void OnMouseDown()
    {
        if (!_qte.isActive || !_isClickable) return;
        _isClickable = false;
        _sparks.Play();
        if (_qte.isPlayerControlled) SoundManager.SFX("PositiveButton");
        GameManager.instance.progressBar.percent += (1f / _qte.towerCount);
        transform.DOLocalMoveY(0f, 1f)
            .OnComplete(() => { 
                gameObject.SetActive(false); 
            });
    }

    #endregion

}