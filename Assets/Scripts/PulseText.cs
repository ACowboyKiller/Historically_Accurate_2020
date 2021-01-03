using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class PulseText : MonoBehaviour
{

    #region --------------------    Public Enumerations



    #endregion

    #region --------------------    Public Events



    #endregion

    #region --------------------    Public Properties



    #endregion

    #region --------------------    Public Methods

    /// <summary>
    /// Pulses the text
    /// </summary>
    public void Pulse()
    {
        _tween = _text.DOFade(0f, 0.5f).OnComplete(() => _text.DOFade(1f, 0.5f).OnComplete(Pulse));
    }

    /// <summary>
    /// Steadies the text
    /// </summary>
    public void Steady()
    {
        _tween.Pause();
        _text.DOFade(1f, 0.5f);
    }

    #endregion

    #region --------------------    Private Fields

    [SerializeField] private TMP_Text _text = null;
    private DG.Tweening.Core.TweenerCore<Color, Color, DG.Tweening.Plugins.Options.ColorOptions> _tween = null;
    [SerializeField] private bool _pulseOnStart = false;

    #endregion

    #region --------------------    Private Methods

    private void Start()
    {
        if (_pulseOnStart) Pulse();
    }

    #endregion

}