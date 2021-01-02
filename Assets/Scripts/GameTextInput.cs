using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameTextInput : MonoBehaviour
{

    #region --------------------    Public Properties

    /// <summary>
    /// Used to set the default text value for labels based off of the object's name
    /// </summary>
    public static Dictionary<string, Dictionary<GameManager.CountryName, string>> textValues = new Dictionary<string, Dictionary<GameManager.CountryName, string>>
    {
        { "LaunchLabel", new Dictionary<GameManager.CountryName, string> { { GameManager.CountryName.USA, "LAUNCH" }, { GameManager.CountryName.USSR, "ZAPUSK" } } },
        { "TestLabel", new Dictionary<GameManager.CountryName, string> { { GameManager.CountryName.USA, "PUBLIC TEST" }, { GameManager.CountryName.USSR, "OTSENIVAT'" } } },
        { "ReportLabel", new Dictionary<GameManager.CountryName, string> { { GameManager.CountryName.USA, "RESEARCH REPORT" }, { GameManager.CountryName.USSR, "OTCHET" } } },
        { "PropagandaLabel", new Dictionary<GameManager.CountryName, string> { { GameManager.CountryName.USA, "PROPAGANDA" }, { GameManager.CountryName.USSR, "|" } } },
        { "ExperimentLabel", new Dictionary<GameManager.CountryName, string> { { GameManager.CountryName.USA, "PERFORM EXPERIMENT" }, { GameManager.CountryName.USSR, "ISSLEDOVANIYE" } } }
    };

    /// <summary>
    /// Returns the text input
    /// </summary>
    public InputField textInput => _textInput;

    #endregion

    #region --------------------    Public Methods

    /// <summary>
    /// Initializes the labels to match the player's country
    /// </summary>
    public void Init()
    {
        Country _player = GameManager.playerCountry;
        _textLabels.ForEach(l =>
        {
            l.textField.text = (textValues[l.name][_player.countryName] != "|") ? textValues[l.name][_player.countryName] : l.textField.text;
        });
    }

    #endregion

    #region --------------------    Private Fields

    /// <summary>
    /// The text input for the game text input
    /// </summary>
    [SerializeField] private InputField _textInput = null;

    /// <summary>
    /// The previously input text value
    /// </summary>
    private string _textValueCache = "";

    /// <summary>
    /// The list of all available text labels
    /// </summary>
    [SerializeField] private List<ActionLabel> _textLabels = new List<ActionLabel>();

    #endregion

    #region --------------------    Private Methods

    /// <summary>
    /// Perform startup config
    /// </summary>
    private void Awake()
    {
        _textInput.onValueChanged.AddListener(_RestrictTextInput);
        _textInput.onValueChanged.AddListener(_UpdateLabelVisuals);
        _textInput.onValueChanged.AddListener(_CheckForCompleteInput);
    }

    /// <summary>
    /// Selects the locking text field
    /// </summary>
    private void Start() => _textInput.Select();

    /// <summary>
    /// Focuses on the text input
    /// </summary>
    private void Update()
    {
        if (GameManager.state == GameManager.GameState.Gameplay && !textInput.isFocused)
        {
            textInput.Select();
            textInput.text = "";
        }
    }

    /// <summary>
    /// Restricts the value of the text box to base values for the labels listed
    /// </summary>
    /// <param name="_pValue"></param>
    private void _RestrictTextInput (string _pValue)
    {
        //  Clear out cache if emptying text input
        _textValueCache = (_pValue == "") ? "" : _textValueCache;

        //  Breakout if the value is unchanged or if the player is currently performing a QTE
        if (_pValue == _textValueCache || _pValue == "" || GameManager.instance.progressMod > 0f || GameManager.instance.timerMod < 0f) return;

        //  Used for determining if the text value will be reset to cache or if it will hold
        bool _isValid = false;

        //  Get textValue Keys
        foreach (KeyValuePair<string, Dictionary<GameManager.CountryName, string>> k in textValues)
        {
            _isValid = _isValid || ((_textLabels.Exists(t => t.name == k.Key)) && textValues[k.Key][GameManager.playerCountry.countryName].StartsWith(_pValue.ToUpper()));
        }

        //  Reset the value or maintain if it's valid
        _textValueCache = (_isValid && _pValue != "|") ? _pValue : _textValueCache;
        _textInput.text = _textValueCache;
    }

    /// <summary>
    /// Updates the visuals the appropriate labels
    /// </summary>
    /// <param name="_pValue"></param>
    private void _UpdateLabelVisuals(string _pValue)
    {
        //  Update value to match cache
        _pValue = _textValueCache;

        Country _player = GameManager.playerCountry;

        //  Update the labels to display input progression
        _textLabels.ForEach(l =>
        {
            l.textField.text = (textValues[l.name][_player.countryName].StartsWith(_pValue.ToUpper()) && textValues[l.name][_player.countryName] != "|") ?
                $"<color=#{ColorUtility.ToHtmlStringRGBA(l.fillColor)}>{_pValue.ToUpper()}</color>{textValues[l.name][_player.countryName].Substring(_pValue.Length)}" :
                ((textValues[l.name][_player.countryName] != "|")? textValues[l.name][_player.countryName] : l.textField.text);
        });
    }

    /// <summary>
    /// Checks for completeness of the text input
    /// </summary>
    /// <param name="_pValue"></param>
    private void _CheckForCompleteInput(string _pValue)
    {
        //  Update value to match cache
        _pValue = _textValueCache;

        //  Breakout if the value is unchanged
        if (_pValue == "") return;

        //  Reset text input if applicable
        ActionLabel _label = _textLabels.Find(l => textValues[l.name][GameManager.playerCountry.countryName] == _pValue.ToUpper());
        _textInput.text = _label != null ? "" : _textValueCache;
        _label?.Invoke();

    }

    #endregion

}