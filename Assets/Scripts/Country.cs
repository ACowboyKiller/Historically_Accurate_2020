using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Country : MonoBehaviour
{

    #region --------------------    Public Enumerations



    #endregion

    #region --------------------    Public Events



    #endregion

    #region --------------------    Public Properties

    /// <summary>
    /// Returns the name of the country
    /// </summary>
    public GameManager.CountryName countryName => _countryName;

    /// <summary>
    /// Stores whether or not the country is controlled by the player
    /// </summary>
    public bool isPlayerControlled { get; set; } = false;

    /// <summary>
    /// Returns the number of workforce tokens the player has
    /// </summary>
    public int workforceTokens => _workforceTokens;

    #endregion

    #region --------------------    Public Methods

    /// <summary>
    /// Initializes the country
    /// </summary>
    public void Initialize(bool _pIsPlayerControlled = false)
    {
        isPlayerControlled = _pIsPlayerControlled;
        _StreamPride();
        _StreamFunding();
    }

    /// <summary>
    /// Purchase a workforce token
    /// </summary>
    public bool BuyWorkForceToken()
    {
        if (_fundingPoints < _tokenCost) return false;
        _fundingPoints -= _tokenCost;
        _tokenCost *= 2;
        _workforceTokens++;
        /// TODO:   Play some animation
        return true;
    }

    /// <summary>
    /// Consumes a workforce token
    /// </summary>
    public bool UseWorkForceToken()
    {
        if (_workforceTokens <= 0) return false;
        _workforceTokens--;
        /// TODO:   Play some animation
        return true;
    }

    /// <summary>
    /// Performs some animation whenever a launch is completed
    /// </summary>
    public void CompleteLaunch(bool _pSuccess = false)
    {
        /// TODO:   Play some animation
    }

    /// <summary>
    /// Performs some animation whenever a public test is completed
    /// </summary>
    public void CompleteTest(bool _pSuccess = false)
    {
        /// TODO:   Play some animation
    }

    /// <summary>
    /// Performs some animation whenever a research report is completed
    /// </summary>
    public void CompleteReport(bool _pSuccess = false)
    {
        /// TODO:   Play some animation
    }

    /// <summary>
    /// Performs some animation whenever propaganda is completed
    /// </summary>
    public void CompletePropaganda(bool _pSuccess = false)
    {
        /// TODO:   Play some animation
    }

    /// <summary>
    /// Performs some animation whenever an expirement is completed and updates the research point total
    /// </summary>
    public void CompleteExperiment(bool _pSuccess = false)
    {
        _researchPoints += _researchValue;
        /// TODO:   Play some animation
    }

    /// <summary>
    /// Begins the launch qte
    /// </summary>
    /// <param name="_pbool"></param>
    public void LaunchQTE(bool _pbool = false)
    {

    }

    /// <summary>
    /// Begins the test qte
    /// </summary>
    /// <param name="_pbool"></param>
    public void TestQTE(bool _pbool = false)
    {

    }

    /// <summary>
    /// Begins the report qte
    /// </summary>
    /// <param name="_pbool"></param>
    public void ReportQTE(bool _pbool = false)
    {

    }

    /// <summary>
    /// Begins the propaganda qte
    /// </summary>
    /// <param name="_pbool"></param>
    public void PropagandaQTE(bool _pbool = false)
    {

    }

    /// <summary>
    /// Begins the experiment qte
    /// </summary>
    /// <param name="_pbool"></param>
    public void ExperimentQTE(bool _pbool = false)
    {

    }

    #endregion

    #region --------------------    Private Fields

    private int _level = 0;

    [SerializeField] GameManager.CountryName _countryName = GameManager.CountryName.USA;

    [SerializeField] private int _nationalPride = 0;
    [SerializeField] private int _prideStream = 0;
    private float _prideTimer = 0f;

    [SerializeField] private int _fundingPoints = 0;
    [SerializeField] private int _fundingStream = 0;
    private float _fundingTimer = 0f;

    [SerializeField] private int _researchPoints = 0;
    [SerializeField] private int _researchValue = 0;

    [SerializeField] private int _workforceTokens = 3;
    [SerializeField] private int _tokenCost = 1;

    #endregion

    #region --------------------    Private Methods

    /// <summary>
    /// Adds / Subtracts national pride and checks to see if pride has reached 0
    /// </summary>
    private void _StreamPride()
    {
        //  Breakout if game is not active
        if (GameManager.state != GameManager.GameState.Gameplay) return;

        //  Configure timer
        _prideTimer = 0f;
        DOTween.To(() => _prideTimer, x => _prideTimer = x, 1f, 1f).OnComplete(_StreamPride);

        //  Perform Stream
        _nationalPride = Mathf.Clamp(_prideStream + _nationalPride, 0, 1000);
        /// TODO:   Play some animation
        
        //  Check for game end
        if (_nationalPride == 0) GameManager.instance.LoseGame(this);
    }

    /// <summary>
    /// Adds / Subtracts funding stream from points
    /// </summary>
    private void _StreamFunding()
    {
        //  Breakout if game is not active
        if (GameManager.state != GameManager.GameState.Gameplay) return;

        //  Configure timer
        _fundingTimer = 0f;
        DOTween.To(() => _fundingTimer, x => _fundingTimer = x, 1f, 1f).OnComplete(_StreamFunding);

        //  Perform stream
        _fundingPoints = Mathf.Clamp(_fundingPoints + _fundingStream, 0, 99999);
        /// TODO:   Play some animation
    }

    #endregion

}