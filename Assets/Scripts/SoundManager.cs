using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SoundManager : MonoBehaviour
{

    #region --------------------    Public Enumerations



    #endregion

    #region --------------------    Public Events



    #endregion

    #region --------------------    Public Properties

    /// <summary>
    /// The singleton instance
    /// </summary>
    public static SoundManager instance { get; private set; } = null;

    /// <summary>
    /// The static flag for sfx
    /// </summary>
    public static bool isSFXEnabled { get; private set; } = true;

    #endregion

    #region --------------------    Public Methods

    /// <summary>
    /// Plays a sound effect
    /// </summary>
    /// <param name="_pTitle"></param>
    public static void SFX(string _pTitle) => instance.PlaySFX(_pTitle);

    /// <summary>
    /// Toggles the game music
    /// </summary>
    public void ToggleMusic()
    {
        _isMusicEnabled = !_isMusicEnabled;
        _musicImg.sprite = (_isMusicEnabled) ? _musicOn : _musicOff;
        DOTween.To(() => _musicSource.volume, x => _musicSource.volume = x, (_isMusicEnabled) ? 0.5f : 0f, 0.5f);
    }

    /// <summary>
    /// Toggles SFX
    /// </summary>
    public void ToggleSFX()
    {
        isSFXEnabled = !isSFXEnabled;
        _SFXImg.sprite = (isSFXEnabled) ? _SFXOn : _SFXOff;
    }

    /// <summary>
    /// Plays a sound effect by title
    /// </summary>
    /// <param name="_pTitle"></param>
    public void PlaySFX(string _pTitle)
    {
        if (!isSFXEnabled) return;
        _audioClips.Find(g => g.name == _pTitle)?.GetComponent<AudioSource>().Play();
    }

    #endregion

    #region --------------------    Private Fields

    private bool _isMusicEnabled = true;
    [SerializeField] private Image _musicImg;
    [SerializeField] private Sprite _musicOn;
    [SerializeField] private Sprite _musicOff;
    [SerializeField] private AudioSource _musicSource;

    [SerializeField] private Image _SFXImg;
    [SerializeField] private Sprite _SFXOn;
    [SerializeField] private Sprite _SFXOff;

    [SerializeField] private List<GameObject> _audioClips = new List<GameObject>();

    #endregion

    #region --------------------    Private Methods

    private void Awake()
    {
        instance = instance ?? this;
        if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    #endregion

}