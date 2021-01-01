using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ActionLabel : MonoBehaviour
{

    #region --------------------    Public Enumerations

    /// <summary>
    /// The available actions to be taken by labels in the game
    /// </summary>
    public enum GameAction { Launch, Test, Report, Propaganda, Experiment };

    #endregion

    #region --------------------    Public Properties

    /// <summary>
    /// The different game instructions
    /// </summary>
    public static Dictionary<GameAction, string> instructions = new Dictionary<GameAction, string>
    {
        { GameAction.Launch, "Click & hold the rocket" },
        { GameAction.Test, "Click & hold the rocket" },
        { GameAction.Report, "Rapidly click the flask" },
        { GameAction.Experiment, "Rapidly click the flask" },
        { GameAction.Propaganda, "Click each of the towers" }
    };

    /// <summary>
    /// Available Game Actions
    /// </summary>
    public Dictionary<GameAction, System.Action<bool>> instantActions = new Dictionary<GameAction, System.Action<bool>>() { };

    /// <summary>
    /// Available Game Actions
    /// </summary>
    public Dictionary<GameAction, System.Action<bool>> qteActions = new Dictionary<GameAction, System.Action<bool>>() { };

    /// <summary>
    /// Returns the fill color for the label
    /// </summary>
    public Color fillColor => _fillColor;

    /// <summary>
    /// Returns the text field for the label
    /// </summary>
    public TMP_Text textField => _text;

    #endregion

    #region --------------------    Public Methods

    /// <summary>
    /// Sets up the label
    /// </summary>
    public void Setup() => _BuildActionsDictionary();

    /// <summary>
    /// Invokes the action of the label
    /// </summary>
    public void Invoke()
    {
        if (GameManager.playerCountry.HasFunding(_gameAction))
        {
            if ((_gameAction == GameAction.Launch || _gameAction == GameAction.Test) && !GameManager.playerCountry.HasResearchPoints())
            {
                //  TODO:   Unable to perform action.  Play sound / do UI animation
                return;
            }
            if (GameManager.playerCountry.UseWorkForceToken())
            {
                instantActions[_gameAction]?.Invoke(true);
            }
            else
            {
                GameManager.instance.StartQTE(instructions[_gameAction]);
                qteActions[_gameAction]?.Invoke(false);
            }
        }
    }

    #endregion

    #region --------------------    Private Fields

    /// <summary>
    /// The color used to fill in the text
    /// </summary>
    [SerializeField] private Color _fillColor = Color.red;

    /// <summary>
    /// The game action to perform
    /// </summary>
    [SerializeField] private GameAction _gameAction = GameAction.Experiment;

    /// <summary>
    /// The text field for the label
    /// </summary>
    private TMP_Text _text = null;

    #endregion

    #region --------------------    Private Methods

    /// <summary>
    /// Grabs the text label
    /// </summary>
    private void Start()
    {
        _text = GetComponent<TMP_Text>();
    }

    /// <summary>
    /// Builds the game actions dictionary
    /// </summary>
    private void _BuildActionsDictionary()
    {
        instantActions.Clear();
        qteActions.Clear();

        instantActions.Add(GameAction.Launch, GameManager.playerCountry.CompleteLaunch);
        instantActions.Add(GameAction.Test, GameManager.playerCountry.CompleteTest);
        instantActions.Add(GameAction.Report, GameManager.playerCountry.CompleteReport);
        instantActions.Add(GameAction.Propaganda, GameManager.playerCountry.CompletePropaganda);
        instantActions.Add(GameAction.Experiment, GameManager.playerCountry.CompleteExperiment);

        qteActions.Add(GameAction.Launch, GameManager.playerCountry.LaunchQTE);
        qteActions.Add(GameAction.Test, GameManager.playerCountry.TestQTE);
        qteActions.Add(GameAction.Report, GameManager.playerCountry.ReportQTE);
        qteActions.Add(GameAction.Propaganda, GameManager.playerCountry.PropagandaQTE);
        qteActions.Add(GameAction.Experiment, GameManager.playerCountry.ExperimentQTE);
    }

    #endregion

}