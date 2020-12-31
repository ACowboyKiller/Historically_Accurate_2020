using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    #region --------------------    Public Enumerations

    /// <summary>
    /// The different states of the game
    /// </summary>
    public enum GameState { Splash, Setup, Gameplay, Results };

    /// <summary>
    /// The difficulty of the game
    /// </summary>
    public enum GameDifficulty { Easy, Medium, Hard };

    /// <summary>
    /// The available game languages
    /// </summary>
    public enum CountryName { USA, USSR };

    #endregion

    #region --------------------    Public Properties

    /// <summary>
    /// The singleton instance
    /// </summary>
    public static GameManager instance { get; private set; } = null;

    /// <summary>
    /// The current state of the game
    /// </summary>
    public static GameState state { get; private set; } = GameState.Splash;

    /// <summary>
    /// The current difficulty of the game
    /// </summary>
    public static GameDifficulty difficulty { get; private set; } = GameDifficulty.Easy;

    /// <summary>
    /// Returns the player's country of the game
    /// </summary>
    public static Country playerCountry => instance._countries.Find(c => c.isPlayerControlled);

    /// <summary>
    /// Returns the ai's country of the game
    /// </summary>
    public static Country aiCountry => instance._countries.Find(c => !c.isPlayerControlled);

    /// <summary>
    /// Returns the timer bar for the game's QTEs
    /// </summary>
    public CustomProgressBar timerBar => _timer;

    /// <summary>
    /// Returns the progress bar for the game's QTEs
    /// </summary>
    public CustomProgressBar progressBar => _progress;

    /// <summary>
    /// The amount to modifier the timer bar value by
    /// </summary>
    public float timerMod { get; set; } = 1f;

    /// <summary>
    /// The amount to modifier the progress bar value by
    /// </summary>
    public float progressMod { get; set; } = -1f;

    /// <summary>
    /// Used to track when the timer bar is empty
    /// </summary>
    public delegate void TimerEvent();
    public TimerEvent OnTimerEmptyEvent;

    /// <summary>
    /// Used to track when the progress bar is full
    /// </summary>
    public delegate void ProgressEvent();
    public ProgressEvent OnProgressFullEvent;

    #endregion

    #region --------------------    Public Methods

    /// <summary>
    /// Selects the game language for gameplay
    /// </summary>
    /// <param name="_pCountry"></param>
    public void SelectCountry(CountryName _pCountry) => _countries.Find(c => c.countryName == _pCountry).isPlayerControlled = true;

    /// <summary>
    /// Sets the game's difficulty
    /// </summary>
    /// <param name="_pDifficulty"></param>
    public void SetDifficulty(GameDifficulty _pDifficulty) => difficulty = _pDifficulty;

    public void LoseGame(Country _pCountry)
    {

    }

    public void WinGame(Country _pCountry)
    {

    }

    #endregion

    #region --------------------    Private Fields

    /// <summary>
    /// The available countries in the game
    /// </summary>
    [SerializeField] private List<Country> _countries = new List<Country>();

    /// <summary>
    /// The timer for the game's QTEs
    /// </summary>
    [SerializeField] private CustomProgressBar _timer = null;

    /// <summary>
    /// The progress bar for the game's QTEs
    /// </summary>
    [SerializeField] private CustomProgressBar _progress = null;

    #endregion

    #region --------------------    Private Methods

    /// <summary>
    /// Calls startup config
    /// </summary>
    private void Awake() => _SetSingleton();

    ///  Sets up singleton instance
    private bool _SetSingleton()
    {
        instance = instance ?? this;
        if (instance == this) return true;
        Destroy(gameObject);
        return false;
    }

    /// <summary>
    /// Used for updating the progress bars
    /// </summary>
    private void Update()
    {
        /// Push the timer bar as needed
        timerBar.percent = timerBar.percent + (Time.deltaTime * timerMod);
        if (timerBar.percent <= 0f)
        {
            timerMod = 5f;
            progressMod = -5f;
            OnTimerEmptyEvent?.Invoke();
        }

        /// Push the progress bar as needed
        progressBar.percent = progressBar.percent + (Time.deltaTime * progressMod);
        if (progressBar.percent >= 1f)
        {
            progressMod = -5f;
            timerMod = 5f;
            OnProgressFullEvent?.Invoke();
        }
    }

    #endregion

}