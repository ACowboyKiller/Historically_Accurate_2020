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
    public static Dictionary<GameManager.CountryName, int[]> researchCosts = new Dictionary<GameManager.CountryName, int[]>
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

    /// <summary>
    /// Returns the token cost
    /// </summary>
    public int tokenCost => _tokenCost;

    /// <summary>
    /// Returns whether or not the country is the USA
    /// </summary>
    public bool isUSA => countryName == GameManager.CountryName.USA;

    #endregion

    #region --------------------    Public Methods

    /// <summary>
    /// Initializes the country
    /// </summary>
    public void Initialize()
    {
        _StreamPride();
        _StreamFunding();
        _USSRStreamPropaganda();
        if (isPlayerControlled)
        {
            for (int i = 0; i < 10; i++)
            {
                GameManager.instance.levelRequirements[i].text = $"<sprite name=Research> {researchCosts[countryName][i]}";
            }
        }
        else
        {
            _aiInstantActions.Clear();

            _aiInstantActions.Add(ActionLabel.GameAction.Launch, CompleteLaunch);
            _aiInstantActions.Add(ActionLabel.GameAction.Test, CompleteTest);
            _aiInstantActions.Add(ActionLabel.GameAction.Report, CompleteReport);
            _aiInstantActions.Add(ActionLabel.GameAction.Propaganda, CompletePropaganda);
            _aiInstantActions.Add(ActionLabel.GameAction.Experiment, CompleteExperiment);

            _prideStream = 0;
        }
    }

    /// <summary>
    /// Resets the country
    /// </summary>
    public void ResetCountry()
    {
        isPlayerControlled = false;
        _isInQTE = false;
        _testLevel = 0;
        _level = 0;
        _nationalPride = _defaultPride;
        _prideStream = _defaultPrideStream;
        _prideTimer = 0f;
        _fundingPoints = _defaultFunding;
        _fundingStream = _defaultFundingStream;
        _fundingTimer = 0f;
        _researchPoints = _defaultResearch;
        _researchValue = _defaultResearchValue;
        _workforceTokens = 3;
        _tokenCost = 8;
        _aiActionTimer = 0f;
    }

    /// <summary>
    /// Purchase a workforce token
    /// </summary>
    public bool BuyWorkForceToken()
    {
        if (_workforceTokens == 3)
        {
            if (isPlayerControlled) SoundManager.SFX("NegativeButton");
            return false;
        }
        if (_fundingPoints < _tokenCost)
        {
            if (isPlayerControlled) GameManager.instance.FlashStatLabelBack(GameManager.instance.fundingBack, Color.red);
            if (isPlayerControlled) SoundManager.SFX("NegativeButton");
            return false;
        }
        _fundingPoints -= _tokenCost;
        _tokenCost *= 2;
        _workforceTokens++;
        if (isPlayerControlled) GameManager.instance.gainTokenParticles.Play();
        GameManager.instance.UpdateTokens();
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
        if (isPlayerControlled) GameManager.instance.useTokenParticles.Play();
        GameManager.instance.UpdateTokens();
        /// TODO:   Play some animation
        return true;
    }

    /// <summary>
    /// Returns whether or not the country has funding points for an action
    /// </summary>
    /// <param name="_pAction"></param>
    /// <returns></returns>
    public bool HasFunding(ActionLabel.GameAction _pAction)
    {
        switch (_pAction)
        {
            case ActionLabel.GameAction.Experiment: return _fundingPoints >= (_level + 1) * 100 / ((isUSA) ? 10 : 8);
            case ActionLabel.GameAction.Propaganda: return _fundingPoints >= (_level + 1) * 100 / ((isUSA) ? 4 : 3);
            case ActionLabel.GameAction.Report: return true;
            case ActionLabel.GameAction.Test: return _fundingPoints >= (_level + 1) * ((isUSA)? 110: 100) / 2;
            case ActionLabel.GameAction.Launch: return _fundingPoints >= (_level + 1) * 100;
        }
        return false;
    }

    /// <summary>
    /// Returns whether or not the country has the research points required to launch
    /// </summary>
    /// <returns></returns>
    public bool HasLaunchResearchPoints() => _researchPoints >= researchCosts[countryName][_level];

    /// <summary>
    /// Has the next level been tested
    /// </summary>
    /// <returns></returns>
    public bool HasTestedLevel() => _testLevel > _level;

    /// <summary>
    /// Returns whether or not the country has the research points required to test
    /// </summary>
    /// <returns></returns>
    public bool HasTestResearchPoints() => _testLevel < 10 && _researchPoints >= researchCosts[countryName][_testLevel];

    /// <summary>
    /// Performs some animation whenever a launch is completed
    /// </summary>
    public void CompleteLaunch(bool _pSuccess = false)
    {
        _isInQTE = false;
        if (_pSuccess)
        {
            _launchQTE.RocketAnim();
            _level++;
            if (_level > 9 && isPlayerControlled) GameManager.instance.WinGame(this, "\n(You've reched the Moon!)");
            if (_level > 9 && !isPlayerControlled) GameManager.instance.LoseGame(GameManager.playerCountry, "\n(Your opponent has reached the Moon!)");
            _nationalPride += (isUSA) ? 165 : 150;
            if (isPlayerControlled) GameManager.instance.FlashStatLabelBack(GameManager.instance.prideBack, Color.green);
            //_prideStream = _prideStream;
            _fundingPoints -= _level * 100;
            _fundingStream += (isUSA) ? 2 : 1;
            //_researchPoints = _researchPoints;
            //_researchValue = _researchValue;
            if (isPlayerControlled)
            {
                DOTween.To(() => GameManager.instance.launchProgress.percent,
                    x => GameManager.instance.launchProgress.percent = x,
                    GameManager.instance.launchProgress.percent + (1f / 10f), 0.5f)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() => { GameManager.instance.levelIcons[_level - 1].color = GameManager.instance.launchProgress.effectColor; });
            }
            else
            {
                DOTween.To(() => GameManager.instance.aiLaunchProgress.percent,
                    x => GameManager.instance.aiLaunchProgress.percent = x,
                    GameManager.instance.aiLaunchProgress.percent + (1f / 10f), 0.5f)
                    .SetEase(Ease.OutQuad);
            }
        }
        else
        {
            /// TODO:   Play some animation
            _nationalPride -= ((isUSA) ? 100 : 80);
            if (isPlayerControlled) GameManager.instance.FlashStatLabelBack(GameManager.instance.prideBack, Color.red);
            //_prideStream = _prideStream;
            _fundingPoints -= (_level + 1) * 100;
            _fundingStream = Mathf.Max(_fundingStream - 3, 1);
            //_researchPoints = _researchPoints;
            //_researchValue = _researchValue;
        }
    }

    /// <summary>
    /// Begins the launch qte
    /// </summary>
    /// <param name="_pbool"></param>
    public void LaunchQTE(bool _pEmpty = false)
    {
        _isInQTE = true;
        _launchQTE.Init();
    }

    /// <summary>
    /// Performs some animation whenever a public test is completed
    /// </summary>
    public void CompleteTest(bool _pSuccess = false)
    {
        _isInQTE = false;
        if (_pSuccess)
        {
            /// TODO:   Play some animation
            _testQTE.RocketAnim();
            _testLevel++;
            _nationalPride += (isUSA) ? 55 : 50;
            if (isPlayerControlled) GameManager.instance.FlashStatLabelBack(GameManager.instance.prideBack, Color.green);
            _prideStreamTween.Pause();
            _prideStream = 5;
            _prideStreamTween = DOTween.To(() => _prideStream, x => _prideStream = x, (isPlayerControlled)? _defaultPrideStream : 0, 5f);
            _fundingPoints -= _testLevel * ((isUSA) ? 110 : 100) / 2;
            //_fundingStream += 1;
            _researchPoints += (isUSA) ? _researchValue / 2 : 0;
            if (isPlayerControlled) GameManager.instance.FlashStatLabelBack(GameManager.instance.researchBack, Color.green);
            //_researchValue = _researchValue;
            if (isPlayerControlled)
            {
                DOTween.To(() => GameManager.instance.testProgress.percent,
                    x => GameManager.instance.testProgress.percent = x,
                    GameManager.instance.testProgress.percent + (1f / 10f), 0.5f)
                    .SetEase(Ease.OutQuad)
                    .OnComplete(() => { GameManager.instance.levelIcons[_testLevel - 1].color = GameManager.instance.testProgress.effectColor; });
            }
            else
            {
                DOTween.To(() => GameManager.instance.aiTestProgress.percent,
                    x => GameManager.instance.aiTestProgress.percent = x,
                    GameManager.instance.aiTestProgress.percent + (1f / 10f), 0.5f)
                    .SetEase(Ease.OutQuad);
            }
        }
        else
        {
            /// TODO:   Play some animation
            //_nationalPride -= 100;
            _prideStreamTween.Pause();
            _prideStream = (isUSA) ? -7 : -5;
            _prideStreamTween = DOTween.To(() => _prideStream, x => _prideStream = x, (isPlayerControlled) ? _defaultPrideStream : 0, 7f);
            _fundingPoints -= (_testLevel + 1) * ((isUSA) ? 110 : 100) / 2;
            //_fundingStream = Mathf.Max(_fundingStream - 3, 0);
            //_researchPoints = _researchPoints;
            //_researchValue = _researchValue;
        }
    }

    /// <summary>
    /// Begins the test qte
    /// </summary>
    /// <param name="_pbool"></param>
    public void TestQTE(bool _pEmpty = false)
    {
        _isInQTE = true;
        _testQTE.Init();
    }

    /// <summary>
    /// Performs some animation whenever a research report is completed
    /// </summary>
    public void CompleteReport(bool _pSuccess = false)
    {
        _isInQTE = false;
        if (_pSuccess)
        {
            /// TODO:   Play some animation
            _reportQTE.FlaskAnim();
            //_nationalPride += 250;
            //_prideStream = _prideStream;
            //_fundingPoints -= fundingCosts[countryName][_level];
            _fundingStream += 1;
            _researchPoints += Mathf.FloorToInt((_researchValue * ((isPlayerControlled)?(3 - (int)GameManager.difficulty):2)) * ((isUSA) ? 1.1f : 1f));
            if (isPlayerControlled) GameManager.instance.FlashStatLabelBack(GameManager.instance.researchBack, Color.green);
            _researchValue += 1;
        }
        else
        {
            /// TODO:   Play some animation
            //_nationalPride -= 100;
            _prideStreamTween.Pause();
            _prideStream = (isUSA) ? -7 : -5;
            _prideStreamTween = DOTween.To(() => _prideStream, x => _prideStream = x, (isPlayerControlled) ? _defaultPrideStream : 0, 7f);
            //_fundingPoints -= fundingCosts[countryName][_level];
            //_fundingStream = Mathf.Max(_fundingStream - 3, 0);
            //_researchPoints = _researchPoints;
            //_researchValue = _researchValue;
        }
    }

    /// <summary>
    /// Begins the report qte
    /// </summary>
    /// <param name="_pbool"></param>
    public void ReportQTE(bool _pEmpty = false)
    {
        _isInQTE = true;
        _reportQTE.Init();
    }

    /// <summary>
    /// Performs some animation whenever propaganda is completed
    /// </summary>
    public void CompletePropaganda(bool _pSuccess = false)
    {
        _isInQTE = false;
        if (_pSuccess)
        {
            /// TODO:   Play some animation
            _propagandaQTE.DoneAnim();
            //_nationalPride += 250;
            _prideStreamTween?.Pause();
            _prideStream = (isUSA) ? 10 : 15;
            _prideStreamTween = DOTween.To(() => _prideStream, x => _prideStream = x, (isPlayerControlled) ? _defaultPrideStream : 0, (isUSA) ? 10f : 15f);
            //_fundingPoints -= fundingCosts[countryName][_level] / 4;
            _fundingStream += (isUSA) ? 3 : 2;
            //_researchPoints = _researchPoints;
            //_researchValue = _researchValue;
        }
        else
        {
            /// TODO:   Play some animation
            //_nationalPride -= 100;
            //_prideStream = _prideStream;
            _fundingPoints -= researchCosts[countryName][_level] / ((isUSA) ? 4 : 3);
            _fundingStream = Mathf.Max(_fundingStream - ((isUSA) ? 3 : 2), 0);
            //_researchPoints = _researchPoints;
            //_researchValue = _researchValue;
        }
    }

    /// <summary>
    /// Begins the propaganda qte
    /// </summary>
    /// <param name="_pbool"></param>
    public void PropagandaQTE(bool _pEmpty = false)
    {
        _isInQTE = true; 
        _propagandaQTE.Init();
    }

    /// <summary>
    /// Performs some animation whenever an expirement is completed and updates the research point total
    /// </summary>
    public void CompleteExperiment(bool _pSuccess = false)
    {
        _isInQTE = false;
        if (_pSuccess)
        {
            /// TODO:   Play some animation
            _experimentQTE.FlaskAnim();
            //_nationalPride += 250;
            //_prideStream = _prideStream;
            _fundingPoints -= researchCosts[countryName][_level] / ((isUSA) ? 10 : 8);
            _fundingStream += 2;
            _researchPoints += ((_researchValue * 2) * ((isPlayerControlled)?(3 - (int)GameManager.difficulty):2)) + ((isUSA) ? 0 : 2);
            if (isPlayerControlled) GameManager.instance.FlashStatLabelBack(GameManager.instance.researchBack, Color.green);
            //_researchValue = _researchValue;
        }
        else
        {
            /// TODO:   Play some animation
            //_nationalPride -= 100;
            //_prideStream = _prideStream;
            _fundingPoints -= researchCosts[countryName][_level] / 10;
            //_fundingStream = Mathf.Max(_fundingStream - 3, 0);
            _researchPoints += _researchValue;
            if (isPlayerControlled) GameManager.instance.FlashStatLabelBack(GameManager.instance.researchBack, Color.green);
            _researchValue = Mathf.Max(_researchValue - ((isUSA) ? 0 : 1), 1);
        }
    }

    /// <summary>
    /// Begins the experiment qte
    /// </summary>
    /// <param name="_pbool"></param>
    public void ExperimentQTE(bool _pEmpty = false)
    {
        _isInQTE = true;
        _experimentQTE.Init();
    }

    #endregion

    #region --------------------    Private Fields

    private int _testLevel = 0;
    private int _level = 0;

    [SerializeField] GameManager.CountryName _countryName = GameManager.CountryName.USA;

    private bool _isInQTE = false;

    [SerializeField] private int _nationalPride = 0;
    private int _defaultPride = 0;
    [SerializeField] private int _prideStream = 0;
    private int _defaultPrideStream = 0;
    private float _prideTimer = 0f;

    [SerializeField] private int _fundingPoints = 0;
    private int _defaultFunding = 0;
    [SerializeField] private int _fundingStream = 0;
    private int _defaultFundingStream = 0;
    private float _fundingTimer = 0f;

    [SerializeField] private int _researchPoints = 0;
    private int _defaultResearch = 0;
    [SerializeField] private int _researchValue = 0;
    private int _defaultResearchValue = 0;

    [SerializeField] private int _workforceTokens = 3;
    private int _tokenCost = 8;

    private float _ussrPropagandaTimer = 0f;
    private int _ussrPropagandaWaitRemaining = 25;

    [SerializeField] private RocketQTE _launchQTE = null;
    [SerializeField] private RocketQTE _testQTE = null;
    [SerializeField] private FlaskQTE _experimentQTE = null;
    [SerializeField] private FlaskQTE _reportQTE = null;
    [SerializeField] private PropagandaQTE _propagandaQTE = null;

    private DG.Tweening.Core.TweenerCore<int, int, DG.Tweening.Plugins.Options.NoOptions> _prideStreamTween = null;

    private float _aiActionTimer = 0f;

    private float[] _aiActionProbabilities = new float[] { 0.7f, 0.8f, 0.9f };

    /// <summary>
    /// Available Game Actions
    /// </summary>
    private Dictionary<ActionLabel.GameAction, System.Action<bool>> _aiInstantActions = new Dictionary<ActionLabel.GameAction, System.Action<bool>>() { };

    #endregion

    #region --------------------    Private Methods

    /// <summary>
    /// Document defaults
    /// </summary>
    private void Awake()
    {
        _defaultPride = _nationalPride;
        _defaultPrideStream = _prideStream;
        _defaultFunding = _fundingPoints;
        _defaultFundingStream = _fundingStream;
        _defaultResearch = _researchPoints;
        _defaultResearchValue = _researchValue;
    }

    /// <summary>
    /// Updates the stat labels
    /// </summary>
    private void Update()
    {
        if (GameManager.state != GameManager.GameState.Gameplay) return;
        if (!isPlayerControlled)
        {
            _aiActionTimer -= Time.deltaTime;
            if (_aiActionTimer <= 0f)
            {
                _aiActionTimer = Random.Range((3 - (float)GameManager.difficulty), 2f * (3 - (float)GameManager.difficulty));
                _ChooseAIAction();
            }
        }
        else
        {
            GameManager.instance.prideLabel.text = _nationalPride.ToString();
            GameManager.instance.prideStreamLabel.text = $"<sprite name={(_prideStream > 0 ? "Up" : "Down")}> {_prideStream}";
            GameManager.instance.fundingLabel.text = _fundingPoints.ToString();
            GameManager.instance.fundingStreamlabel.text = $"<sprite name={(_fundingStream > 0 ? "Up" : "Down")}> {_fundingStream}";
            GameManager.instance.researchLabel.text = _researchPoints.ToString();
            GameManager.instance.reserachValueLabel.text = $"<sprite name=ResearchValue> {_researchValue}";
            GameManager.instance.reserachValueLabel.text = $"<sprite name=ResearchValue> {_researchValue}";
        
            GameManager.instance.experimentResultLabel.text = $"<sprite name=Funding><color=red> {(_level + 1) * 100 / ((isUSA) ? 10 : 8)}</color>     " +
                $"<sprite name=FundingStream><color=green> 2</color>|<color=red>0</color>     " +
                $"<sprite name=Research><color=green> {((_researchValue * 2) * (3 - (int)GameManager.difficulty)) + ((isUSA) ? 0 : 2)}</color>|<color=green>{_researchValue}</color>     " +
                $"<sprite name=ResearchValue><color=green> 0</color>|<color=red>{((isUSA) ? 0 : 1)}</color>";

            GameManager.instance.propagandaResultLabel.text = $"<sprite name=PrideStream><color=green> {((isUSA) ? 10 : 15)}</color>|<color=red>0</color>     " +
                $"<sprite name=Funding><color=green> 0</color>|<color=red>{(_level + 1) * 100 / ((isUSA)? 4 : 3)}</color>     " +
                $"<sprite name=FundingStream><color=green> {((isUSA)? 3: 2)}</color>|<color=red>{((isUSA) ? 3 : 2)}</color>     ";

            GameManager.instance.reportResultLabel.text = $"<sprite name=PrideStream><color=green> 0</color>|<color=red>7</color>     " +
                $"<sprite name=FundingStream><color=green> 1</color>|<color=red>0</color>     " +
                $"<sprite name=Research><color=green> {Mathf.FloorToInt((_researchValue * (3 - (int)GameManager.difficulty)) * ((isUSA) ? 1.1f : 1f))}</color>|<color=red>0</color>     " +
                $"<sprite name=ResearchValue><color=green> 1</color>|<color=red>0</color>";

            GameManager.instance.testResultLabel.text = $"<sprite name=\"Level\" color=#{ColorUtility.ToHtmlStringRGBA(GameManager.instance.testProgress.effectColor)}><color=green> 1</color>|<color=red>0</color>     " +
                $"<sprite name=Pride><color=green> {((isUSA) ? 55 : 50)}</color>|<color=red>0</color>     " +
                $"<sprite name=PrideStream><color=green> 5</color>|<color=red>{((isUSA) ? 7 : 5)}</color>     " +
                $"<sprite name=Funding><color=red> {(_testLevel + 1) * ((isUSA) ? 110 : 100) / 2}</color>     " +
                $"<sprite name=Research><color=green> {((isUSA) ? _researchValue / 2 : 0)}</color>|<color=red>0</color>";

            GameManager.instance.launchResultLabel.text = $"<sprite name=\"Level\" color=#{ColorUtility.ToHtmlStringRGBA(GameManager.instance.launchProgress.effectColor)}><color=green> 1</color>|<color=red>0</color>     " +
                $"<sprite name=Pride><color=green> {((isUSA) ? 165 : 150)}</color>|<color=red>{((isUSA) ? 100 : 80)}</color>     " +
                $"<sprite name=Funding><color=red> {(_level + 1) * 100}</color>     " +
                $"<sprite name=FundingStream><color=green> {((isUSA) ? 2: 1)}</color>|<color=red>{((isUSA) ? 3 : 1)}</color>";
        }
    }

    /// <summary>
    /// Adds / Subtracts national pride and checks to see if pride has reached 0
    /// </summary>
    private void _StreamPride()
    {
        //  Breakout if game is not active
        if (GameManager.state != GameManager.GameState.Gameplay) return;

        //  Configure timer
        _prideTimer = 0f;
        DOTween.To(() => _prideTimer, x => _prideTimer = x, 1f, 1f)
            .OnComplete(_StreamPride);

        //  Perform Stream
        _nationalPride = Mathf.Max(_prideStream + _nationalPride, 0);

        /// TODO:   Play some animation
        if (_nationalPride < 100 && isPlayerControlled)  GameManager.instance.FlashStatLabelBack(GameManager.instance.prideBack, Color.red);

        //  Check for game end
        if (_nationalPride == 0 && isPlayerControlled) GameManager.instance.LoseGame(this, "\n(Your nation has lost its pride)");
        if (_nationalPride == 0 && !isPlayerControlled) GameManager.instance.WinGame(GameManager.playerCountry, "\n(Your opponent's nation has lost its pride)");
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
        DOTween.To(() => _fundingTimer, x => _fundingTimer = x, 1f, 1f)
            .OnComplete(_StreamFunding);

        //  Perform stream
        _fundingPoints = Mathf.Max(_fundingPoints + _fundingStream, 0);
        /// TODO:   Play some animation
    }

    /// <summary>
    /// Streams propaganda every 25 seconds for USSR
    /// </summary>
    private void _USSRStreamPropaganda()
    {
        //  Breakout if game is not active
        if (GameManager.state != GameManager.GameState.Gameplay || countryName != GameManager.CountryName.USSR) return;

        //  Configure timer
        _ussrPropagandaTimer = 0f;
        DOTween.To(() => _ussrPropagandaTimer, x => _ussrPropagandaTimer = x, 1f, 1f)
            .OnComplete(_USSRStreamPropaganda);

        //  Launch Propaganda
        _ussrPropagandaWaitRemaining--;
        if (_ussrPropagandaWaitRemaining == 0)
        {
            _ussrPropagandaWaitRemaining = 25;
            if (isPlayerControlled)
            {
                if (!_isInQTE)
                {
                    GameManager.instance.textInput.text = "";
                    if (UseWorkForceToken())
                    {
                        CompletePropaganda(true);
                    }
                    else
                    {
                        GameManager.instance.StartQTE(ActionLabel.instructions[ActionLabel.GameAction.Propaganda]);
                        PropagandaQTE();
                    }
                }
            }
            else
            {
                if (!_isInQTE)
                {
                    CompletePropaganda(UseWorkForceToken() || (Random.Range(0f, _aiActionProbabilities[(int)GameManager.difficulty]) > 0.5f));
                }
            }
        }
        if (isPlayerControlled) GameManager.instance.propagandaActionLabel.text = $"{_ussrPropagandaWaitRemaining.ToString()} Sekundy";
    }

    /// <summary>
    /// Chooses an action to perform and is randomly successful or not
    /// </summary>
    private void _ChooseAIAction()
    {
        //  TODO:   Maybe make the action choice more sophisticated than just random
        ActionLabel.GameAction _gameAction = (ActionLabel.GameAction)Random.Range(0, System.Enum.GetValues(typeof(ActionLabel.GameAction)).Length);
        _gameAction = (countryName == GameManager.CountryName.USSR && _gameAction == ActionLabel.GameAction.Propaganda) ? ActionLabel.GameAction.Experiment : _gameAction;
        _gameAction = (_nationalPride < 200) ? ActionLabel.GameAction.Launch : _gameAction;
        if (HasFunding(_gameAction))
        {
            if (_gameAction == ActionLabel.GameAction.Launch && !HasTestedLevel())
            {
                _gameAction = ActionLabel.GameAction.Test;
                if (!HasFunding(_gameAction))
                {
                    return;
                }
            }
            if ((_gameAction == ActionLabel.GameAction.Launch && !HasLaunchResearchPoints()) ||
                (_gameAction == ActionLabel.GameAction.Test && !HasTestResearchPoints()))
            {
                if (Random.Range(0f, 1f) > 0.5f)
                {
                    if (!HasFunding(ActionLabel.GameAction.Report))
                    {
                        return;
                    }
                    CompleteReport(UseWorkForceToken() || (Random.Range(0f, _aiActionProbabilities[(int)GameManager.difficulty]) > 0.5f));
                    return;
                }
                else
                {
                    if (!HasFunding(ActionLabel.GameAction.Experiment))
                    {
                        return;
                    }
                    CompleteExperiment(UseWorkForceToken() || (Random.Range(0f, _aiActionProbabilities[(int)GameManager.difficulty]) > 0.5f));
                    return;
                }
            }
            _aiInstantActions[_gameAction]?.Invoke(UseWorkForceToken() || (Random.Range(0f, _aiActionProbabilities[(int)GameManager.difficulty]) > 0.5f));
        }
        else
        {
            if (Random.Range(0f, 1f) > 0.5f)
            {
                if (!HasFunding(ActionLabel.GameAction.Report)) return;
                CompleteReport(UseWorkForceToken() || (Random.Range(0f, _aiActionProbabilities[(int)GameManager.difficulty]) > 0.5f));
            }
            else
            {
                if (!HasFunding(ActionLabel.GameAction.Experiment)) return;
                CompleteExperiment(UseWorkForceToken() || (Random.Range(0f, _aiActionProbabilities[(int)GameManager.difficulty]) > 0.5f));
            }
        }
    }

    #endregion

}