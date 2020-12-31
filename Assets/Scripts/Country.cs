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
    /// Used to determine the cost of each level to reach
    /// </summary>
    public static Dictionary<GameManager.CountryName, int[]> fundingCosts = new Dictionary<GameManager.CountryName, int[]>
    {
        { GameManager.CountryName.USA, new int[] { 100, 200, 300, 400, 500, 600, 700, 800, 900, 950 } },
        { GameManager.CountryName.USSR, new int[] { 80, 180, 270, 370, 460, 560, 650, 750, 840, 950 } }
    };

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
        if (_workforceTokens < 3 && _fundingPoints < _tokenCost) return false;
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
        if (_pSuccess)
        {
            _launchQTE.RocketAnim();
            _level++;
            if (_level >= 11) GameManager.instance.WinGame(this);
            _nationalPride += 250;
            //_prideStream = _prideStream;
            _fundingPoints -= fundingCosts[countryName][_level];
            _fundingStream += 10;
            //_researchPoints = _researchPoints;
            //_researchValue = _researchValue;
        }
        else
        {
            /// TODO:   Play some animation
            _nationalPride -= 100;
            //_prideStream = _prideStream;
            _fundingPoints -= fundingCosts[countryName][_level];
            _fundingStream = Mathf.Max(_fundingStream - 3, 0);
            //_researchPoints = _researchPoints;
            //_researchValue = _researchValue;
        }
    }

    /// <summary>
    /// Begins the launch qte
    /// </summary>
    /// <param name="_pbool"></param>
    public void LaunchQTE(bool _pEmpty = false) => _launchQTE.Init();

    /// <summary>
    /// Performs some animation whenever a public test is completed
    /// </summary>
    public void CompleteTest(bool _pSuccess = false)
    {
        if (_pSuccess)
        {
            /// TODO:   Play some animation
            _testQTE.RocketAnim();
            _nationalPride += 250;
            //_prideStream = _prideStream;
            _fundingPoints -= fundingCosts[countryName][_level];
            _fundingStream += 10;
            //_researchPoints = _researchPoints;
            //_researchValue = _researchValue;
        }
        else
        {
            /// TODO:   Play some animation
            _nationalPride -= 100;
            //_prideStream = _prideStream;
            _fundingPoints -= fundingCosts[countryName][_level];
            _fundingStream = Mathf.Max(_fundingStream - 3, 0);
            //_researchPoints = _researchPoints;
            //_researchValue = _researchValue;
        }
    }

    /// <summary>
    /// Begins the test qte
    /// </summary>
    /// <param name="_pbool"></param>
    public void TestQTE(bool _pEmpty = false) => _testQTE.Init();

    /// <summary>
    /// Performs some animation whenever a research report is completed
    /// </summary>
    public void CompleteReport(bool _pSuccess = false)
    {
        if (_pSuccess)
        {
            /// TODO:   Play some animation
            _nationalPride += 250;
            //_prideStream = _prideStream;
            _fundingPoints -= fundingCosts[countryName][_level];
            _fundingStream += 10;
            //_researchPoints = _researchPoints;
            //_researchValue = _researchValue;
        }
        else
        {
            /// TODO:   Play some animation
            _nationalPride -= 100;
            //_prideStream = _prideStream;
            _fundingPoints -= fundingCosts[countryName][_level];
            _fundingStream = Mathf.Max(_fundingStream - 3, 0);
            //_researchPoints = _researchPoints;
            //_researchValue = _researchValue;
        }
    }

    /// <summary>
    /// Begins the report qte
    /// </summary>
    /// <param name="_pbool"></param>
    public void ReportQTE(bool _pEmpty = false) => _reportQTE.Init();

    /// <summary>
    /// Performs some animation whenever propaganda is completed
    /// </summary>
    public void CompletePropaganda(bool _pSuccess = false)
    {
        if (_pSuccess)
        {
            /// TODO:   Play some animation
            _nationalPride += 250;
            //_prideStream = _prideStream;
            _fundingPoints -= fundingCosts[countryName][_level];
            _fundingStream += 10;
            //_researchPoints = _researchPoints;
            //_researchValue = _researchValue;
        }
        else
        {
            /// TODO:   Play some animation
            _nationalPride -= 100;
            //_prideStream = _prideStream;
            _fundingPoints -= fundingCosts[countryName][_level];
            _fundingStream = Mathf.Max(_fundingStream - 3, 0);
            //_researchPoints = _researchPoints;
            //_researchValue = _researchValue;
        }
    }

    /// <summary>
    /// Begins the propaganda qte
    /// </summary>
    /// <param name="_pbool"></param>
    public void PropagandaQTE(bool _pEmpty = false) => _propagandaQTE.Init();

    /// <summary>
    /// Performs some animation whenever an expirement is completed and updates the research point total
    /// </summary>
    public void CompleteExperiment(bool _pSuccess = false)
    {
        if (_pSuccess)
        {
            /// TODO:   Play some animation
            _nationalPride += 250;
            //_prideStream = _prideStream;
            _fundingPoints -= fundingCosts[countryName][_level];
            _fundingStream += 10;
            //_researchPoints = _researchPoints;
            //_researchValue = _researchValue;
        }
        else
        {
            /// TODO:   Play some animation
            _nationalPride -= 100;
            //_prideStream = _prideStream;
            _fundingPoints -= fundingCosts[countryName][_level];
            _fundingStream = Mathf.Max(_fundingStream - 3, 0);
            //_researchPoints = _researchPoints;
            //_researchValue = _researchValue;
        }
    }

    /// <summary>
    /// Begins the experiment qte
    /// </summary>
    /// <param name="_pbool"></param>
    public void ExperimentQTE(bool _pEmpty = false) => _experimentQTE.Init();

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

    [SerializeField] private RocketQTE _launchQTE = null;
    [SerializeField] private RocketQTE _testQTE = null;
    [SerializeField] private FlaskQTE _experimentQTE = null;
    [SerializeField] private FlaskQTE _reportQTE = null;
    [SerializeField] private PropagandaQTE _propagandaQTE = null;

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
        _nationalPride = Mathf.Max(_prideStream + _nationalPride, 0);
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
        _fundingPoints = Mathf.Max(_fundingPoints + _fundingStream, 0);
        /// TODO:   Play some animation
    }

    #endregion

}