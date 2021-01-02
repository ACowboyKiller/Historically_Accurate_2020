using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    #region --------------------    Public Enumerations

    /// <summary>
    /// The different states of the game
    /// </summary>
    public enum GameState { Splash, Setup, About, Gameplay, Results };

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
    /// Stores whether or not a country has been selected
    /// </summary>
    public bool isCountrySelected { get; set; } = false;

    /// <summary>
    /// Returns the player's country of the game
    /// </summary>
    public static Country playerCountry => instance._countries.Find(c => c.isPlayerControlled);

    /// <summary>
    /// Returns the ai's country of the game
    /// </summary>
    public static Country aiCountry => instance._countries.Find(c => !c.isPlayerControlled);

    /// <summary>
    /// Returns the text input for the game
    /// </summary>
    public InputField textInput => _textInput.textInput;

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

    /// <summary>
    /// Returns the canvas group for the gameplay background
    /// </summary>
    public CanvasGroup gameplayBackground => _gameplayBackground;

    /// <summary>
    /// Returns the pride back image
    /// </summary>
    public Image prideBack => _prideBack;

    /// <summary>
    /// Returns the funding back image
    /// </summary>
    public Image fundingBack => _fundingBack;

    /// <summary>
    /// Returns the research back image
    /// </summary>
    public Image researchBack => _researchBack;

    /// <summary>
    /// Returns the instructions text
    /// </summary>
    public TMP_Text instructionsText => _instructions;

    /// <summary>
    /// Returns the use token particles
    /// </summary>
    public ParticleSystem useTokenParticles => _useTokenParticles;

    /// <summary>
    /// Returns the gain token particles
    /// </summary>
    public ParticleSystem gainTokenParticles => _gainTokenParticles;

    /// <summary>
    /// Returns the resepective label
    /// </summary>
    public TMP_Text prideLabel => _pridelbl;
    public TMP_Text prideStreamLabel => _prideStreamlbl;
    public TMP_Text fundingLabel => _fundinglbl;
    public TMP_Text fundingStreamlabel => _fundingStreamlbl;
    public TMP_Text researchLabel => _researchlbl;
    public TMP_Text reserachValueLabel => _researchValuelbl;

    /// <summary>
    /// Returns the background image for the level progress bar
    /// </summary>
    public Image levelProgBack => _levelProgBack;

    /// <summary>
    /// Returns the custom progressbar for the launch progress
    /// </summary>
    public CustomProgressBar launchProgress => _launchProgress;

    /// <summary>
    /// Returns the custom progressbar for the test progress
    /// </summary>
    public CustomProgressBar testProgress => _testProgress;

    /// <summary>
    /// Returns the custom progressbar for the ai's launch progress
    /// </summary>
    public CustomProgressBar aiLaunchProgress => _aiLaunchPropgress;

    /// <summary>
    /// Returns the custom progressbar for the ai's public test progress
    /// </summary>
    public CustomProgressBar aiTestProgress => _aiTestProgress;

    /// <summary>
    /// Level icons for the ui
    /// </summary>
    public List<Image> levelIcons => _levelIcons;

    /// <summary>
    /// Returns the list of text labels for the research requirements for levels
    /// </summary>
    public List<TMP_Text> levelRequirements => _levelRequirements;

    /// <summary>
    /// Returns the respective result label
    /// </summary>
    public TMP_Text experimentResultLabel => _experiementResultLabel;
    public TMP_Text propagandaActionLabel => _propagandaActionLabel;
    public TMP_Text propagandaResultLabel => _propagandaResultLabel;
    public TMP_Text reportResultLabel => _reportResultLabel;
    public TMP_Text testResultLabel => _testResultLabel;
    public TMP_Text launchResultLabel => _launchResultLabel;

    public Image playerFlag => (playerCountry.countryName == CountryName.USA) ? _USAFlag : _USSRFlag;
    public Image aiFlag => (aiCountry.countryName == CountryName.USA) ? _USAFlag : _USSRFlag;


    #endregion

    #region --------------------    Public Methods

    /// <summary>
    /// Quit the game
    /// </summary>
    public void Close()
    {
        Application.Quit();
    }

    /// <summary>
    /// Moves to setup
    /// </summary>
    public void MoveToSetup(CanvasGroup _pPrevious) => _pPrevious.DOFade(0f, 0.5f)
        .SetEase(Ease.OutQuad)
        .OnComplete(() => {
            SetInteractable(_pPrevious);
            _setup.DOFade(1f, 0.5f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => {
                    SetInteractable(_setup, true);
                });
            state = GameState.Setup;
        });

    /// <summary>
    /// Moves to about
    /// </summary>
    public void MoveToAbout(CanvasGroup _pPrevious) => _pPrevious.DOFade(0f, 0.5f)
        .SetEase(Ease.OutQuad)
        .OnComplete(() => {
            SetInteractable(_pPrevious);
            _about.DOFade(1f, 0.5f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => {
                    SetInteractable(_about, true);
                });
            state = GameState.About;
        });

    /// <summary>
    /// Resets the canvas group
    /// </summary>
    public void SetInteractable(CanvasGroup _pGroup, bool _pInteractable = false)
    {
        _pGroup.blocksRaycasts = _pInteractable;
        _pGroup.interactable = _pInteractable;
    }

    /// <summary>
    /// Moves to gameplay
    /// </summary>
    public void MoveToGameplay(CanvasGroup _pPrevious) => _pPrevious.DOFade(0f, 0.5f)
        .SetEase(Ease.OutQuad)
        .OnComplete(() => { 
            SetInteractable(_pPrevious); 
            _gameplay.DOFade(1f, 0.5f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() => { 
                    SetInteractable(_gameplay, true); 
                    _textInput.textInput.Select(); 
                });
            state = GameState.Gameplay; 
            playerCountry.Initialize();
            aiCountry.Initialize();
        });

    /// <summary>
    /// Moves to results
    /// </summary>
    public void MoveToResults(CanvasGroup _pPrevious) => _pPrevious.DOFade(0f, 0.5f)
        .SetEase(Ease.OutQuad)
        .OnComplete(() => { 
            SetInteractable(_pPrevious);
            SetInteractable(_results, true);
            _results.DOFade(1f, 0.5f)
                .SetEase(Ease.OutQuad);
            state = GameState.Results;
        });

    /// <summary>
    /// Moves to splash
    /// </summary>
    public void MoveToSplash(CanvasGroup _pPrevious)
    {
        _pPrevious.DOFade(0f, 0.5f)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => {
                SetInteractable(_pPrevious);
                _title.material.DOFade(1f, 0.5f);
                _splash.DOFade(1f, 0.5f).SetEase(Ease.OutQuad); 
                state = GameState.Splash; 
            });
    }

    /// <summary>
    /// Selects the game language for gameplay
    /// </summary>
    /// <param name="_pCountry"></param>
    public void SelectCountry(int _pCountry)
    {
        isCountrySelected = true;
        if (playerCountry != null) playerCountry.isPlayerControlled = false;
        _countries.Find(c => c.countryName == (CountryName)_pCountry).isPlayerControlled = true;
        _labels.ForEach(l => l.Setup());
        _camTarget.DOMove(playerCountry.transform.position, 2f);
        float _countryX = playerCountry.transform.position.x;
        DOTween.To(() => _transposer.m_Heading.m_Bias, x => _transposer.m_Heading.m_Bias = x, (_countryX / -2f) * 90f, 2f);
    }

    /// <summary>
    /// Sets the game's difficulty
    /// </summary>
    /// <param name="_pDifficulty"></param>
    public void SetDifficulty(int _pDifficulty) => difficulty = (GameDifficulty)_pDifficulty;

    /// <summary>
    /// Removes the gameplay background to show the country better
    /// </summary>
    public void StartQTE(string _pInstructions)
    {
        _gameplayBackground.DOFade(0f, 0.25f);
        _instructions.text = _pInstructions;
    }

    /// <summary>
    /// Reset the gameplay background
    /// </summary>
    public void EndQTE()
    {
        _gameplayBackground.DOFade(1f, 0.25f);
        _instructions.text = "Type the name of an action to be performed";
        _textInput.textInput.Select();
    }

    /// <summary>
    /// Starts the game
    /// </summary>
    public void StartGame()
    {
        if (!isCountrySelected) return;
        MoveToGameplay(_setup);
        _textInput.Init();
    }

    /// <summary>
    /// Flashes the background of a stat label
    /// </summary>
    /// <param name="_pBack"></param>
    /// <param name="_pColor"></param>
    public void FlashStatLabelBack(Image _pBack, Color _pColor) => _pBack.DOColor(_pColor, 0.25f).OnComplete(() => { _pBack.color = new Color(0.22f, 0.22f, 0.22f); });

    /// <summary>
    /// Attempts to buy a workforce token
    /// </summary>
    public void BuyToken()
    {
        playerCountry.BuyWorkForceToken();
        _textInput.textInput.Select();
    }

    /// <summary>
    /// Updates the visibility of the work tokens
    /// </summary>
    public void UpdateTokens()
    {
        for (int i = 0; i < _workTokens.Count; i ++)
        {
            _workTokens[i].gameObject.SetActive(i + 1 <= playerCountry.workforceTokens);
        }
        _buyTokensButton.text = (playerCountry.workforceTokens < 3) ? $"<sprite name=Funding> { playerCountry.tokenCost }" : "<color=green>Full</color>";
    }

    /// <summary>
    /// Called when the player loses
    /// </summary>
    /// <param name="_pCountry"></param>
    public void LoseGame(Country _pCountry, string _pReason)
    {
        state = GameState.Results;
        MoveToResults(_gameplay);
        _resultTitle.text = $"<color=red>{((playerCountry.countryName == CountryName.USA) ? "Defeat" : "Porazheniye")} {_pReason}</color>";
        aiFlag.DOFade(1f, 2f);
    }

    /// <summary>
    /// Called when the player wins
    /// </summary>
    /// <param name="_pCountry"></param>
    public void WinGame(Country _pCountry, string _pReason)
    {
        state = GameState.Results;
        MoveToResults(_gameplay);
        _resultTitle.text = $"<color=green>{((playerCountry.countryName == CountryName.USA) ? "Victory" : "Pobeda")} {_pReason}</color>";
        playerFlag.DOFade(1f, 2f);
    }

    /// <summary>
    /// Resets all elements of the gameplay so another round of gameplay can begin
    /// </summary>
    public void ResetGameplay()
    {
        isCountrySelected = false;
        _camTarget.DOMove(Vector3.zero, 2f);
        DOTween.To(() => _transposer.m_Heading.m_Bias, x => _transposer.m_Heading.m_Bias = x, 0f, 2f);

        _textInput.textInput.text = "";
        timerMod = 1f;
        OnTimerEmptyEvent = null;
        progressMod = -1f;
        OnProgressFullEvent = null;
        _workTokens.ForEach(t => t.gameObject.SetActive(true));
        
        _levelIcons.ForEach(i => i.color = new Color(0.33f, 0.33f, 0.33f));
        _aiTestProgress.percent = 0f;
        _aiLaunchPropgress.percent = 0f;
        _testProgress.percent = 0f;
        _launchProgress.percent = 0f;

        _countries.ForEach(c => c.ResetCountry());
    }

    /// <summary>
    /// Resets the results screen
    /// </summary>
    public void ResetResults()
    {
        _resultTitle.text = "";
        aiFlag.DOFade(0f, 0f);
        playerFlag.DOFade(0f, 0f);
    }

    #endregion

    #region --------------------    Private Fields

    /// <summary>
    /// The different canvas groups
    /// </summary>
    [SerializeField] private MeshRenderer _title = null;
    [SerializeField] private CanvasGroup _splash = null;
    [SerializeField] private CanvasGroup _setup = null;
    [SerializeField] private CanvasGroup _about = null;
    [SerializeField] private CanvasGroup _gameplay = null;
    [SerializeField] private CanvasGroup _results = null;

    /// <summary>
    /// The available countries in the game
    /// </summary>
    [SerializeField] private List<Country> _countries = new List<Country>();

    /// <summary>
    /// The target for the camera
    /// </summary>
    [SerializeField] private Transform _camTarget = null;

    /// <summary>
    /// The virtual camera
    /// </summary>
    [SerializeField] private Cinemachine.CinemachineVirtualCamera _cam = null;

    /// <summary>
    /// The virtual camera's transposer
    /// </summary>
    private Cinemachine.CinemachineOrbitalTransposer _transposer = null;

    /// <summary>
    /// The timer for the game's QTEs
    /// </summary>
    [SerializeField] private CustomProgressBar _timer = null;

    /// <summary>
    /// The progress bar for the game's QTEs
    /// </summary>
    [SerializeField] private CustomProgressBar _progress = null;

    /// <summary>
    /// The text input for the game
    /// </summary>
    [SerializeField] private GameTextInput _textInput = null;

    /// <summary>
    /// The list of action labels
    /// </summary>
    [SerializeField] private List<ActionLabel> _labels = new List<ActionLabel>();

    /// <summary>
    /// The canvas group for gameplay background
    /// </summary>
    [SerializeField] private CanvasGroup _gameplayBackground = null;

    /// <summary>
    /// The instructions text
    /// </summary>
    [SerializeField] private TMP_Text _instructions = null;

    [SerializeField] private TMP_Text _pridelbl = null;
    [SerializeField] private Image _prideBack = null;
    [SerializeField] private TMP_Text _prideStreamlbl = null;

    [SerializeField] private TMP_Text _fundinglbl = null;
    [SerializeField] private Image _fundingBack = null;
    [SerializeField] private TMP_Text _fundingStreamlbl = null;

    [SerializeField] private TMP_Text _researchlbl = null;
    [SerializeField] private Image _researchBack = null;
    [SerializeField] private TMP_Text _researchValuelbl = null;

    [SerializeField] private TMP_Text _experiementResultLabel = null;
    [SerializeField] private TMP_Text _propagandaActionLabel = null;
    [SerializeField] private TMP_Text _propagandaResultLabel = null;
    [SerializeField] private TMP_Text _reportResultLabel = null;
    [SerializeField] private TMP_Text _testResultLabel = null;
    [SerializeField] private TMP_Text _launchResultLabel = null;

    /// <summary>
    /// The button used to buy more work force tokens
    /// </summary>
    [SerializeField] private TMP_Text _buyTokensButton = null;

    /// <summary>
    /// The list of token images
    /// </summary>
    [SerializeField] private List<Image> _workTokens = new List<Image>();

    /// <summary>
    /// The particle system for when a token is used
    /// </summary>
    [SerializeField] private ParticleSystem _useTokenParticles = null;

    /// <summary>
    /// The particle system for when a token is gained
    /// </summary>
    [SerializeField] private ParticleSystem _gainTokenParticles = null;

    /// <summary>
    /// The background image for the launch level bar
    /// </summary>
    [SerializeField] private Image _levelProgBack = null;

    /// <summary>
    /// The custom progress bar for the public test level
    /// </summary>
    [SerializeField] private CustomProgressBar _testProgress = null;

    /// <summary>
    /// The custom progress bar for the launch level
    /// </summary>
    [SerializeField] private CustomProgressBar _launchProgress = null;

    /// <summary>
    /// The custom progress bar for the ai public test level
    /// </summary>
    [SerializeField] private CustomProgressBar _aiTestProgress = null;

    /// <summary>
    /// The custom progress bar for the ai launch level
    /// </summary>
    [SerializeField] private CustomProgressBar _aiLaunchPropgress = null;

    /// <summary>
    /// The list of icons for the levels for the UI
    /// </summary>
    [SerializeField] private List<Image> _levelIcons = new List<Image>();

    /// <summary>
    /// This list of labels for level research point requirements
    /// </summary>
    [SerializeField] private List<TMP_Text> _levelRequirements = new List<TMP_Text>();

    /// <summary>
    /// The title of the result screen
    /// </summary>
    [SerializeField] private TMP_Text _resultTitle = null;

    [SerializeField] private Image _USAFlag = null;

    [SerializeField] private Image _USSRFlag = null;

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
    /// Sets up the camera transposer
    /// </summary>
    private void Start() => _transposer = _cam.GetCinemachineComponent<Cinemachine.CinemachineOrbitalTransposer>();

    /// <summary>
    /// Used for updating the progress bars
    /// </summary>
    private void Update()
    {
        /// Quit out on escape
        if (Input.GetKeyDown(KeyCode.Escape)) Close();

        /// Advances the state of the game from the splash screen
        if (state == GameState.Splash && Input.GetKeyDown(KeyCode.Return))
        {
            _title.material.DOFade(0f, 0.5f);
            MoveToSetup(_splash);
        }

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