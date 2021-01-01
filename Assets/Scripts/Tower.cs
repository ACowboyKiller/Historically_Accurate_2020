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

    #endregion

    #region --------------------    Private Methods

    private void OnEnable()
    {
        _sparks.Clear();
    }

    /// <summary>
    /// Completes tower click
    /// </summary>
    private void OnMouseDown()
    {
        if (!_qte.isActive) return;
        _sparks.Play();
        transform.DOLocalMoveY(0f, 1f)
            .OnComplete(() => { 
                gameObject.SetActive(false); 
                GameManager.instance.progressBar.percent += (1f / _qte.towerCount); 
            });
    }

    #endregion

}