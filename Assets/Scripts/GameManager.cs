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
    /// Returns the player's country of the game
    /// </summary>
    public static Country playerCountry => instance._countries.Find(c => c.isPlayerControlled);

    /// <summary>
    /// Returns the ai's country of the game
    /// </summary>
    public static Country aiCountry => instance._countries.Find(c => !c.isPlayerControlled);

    #endregion

    #region --------------------    Public Methods

    /// <summary>
    /// Selects the game language for gameplay
    /// </summary>
    /// <param name="_pCountry"></param>
    public void SelectCountry(CountryName _pCountry) => _countries.Find(c => c.countryName == _pCountry).isPlayerControlled = true;

    public void LoseGame(Country _pCountry)
    {

    }

    #endregion

    #region --------------------    Private Fields

    /// <summary>
    /// The available countries in the game
    /// </summary>
    [SerializeField] private List<Country> _countries = new List<Country>();

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

    #endregion

}