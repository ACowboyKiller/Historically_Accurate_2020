using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// A locked text input field that won't lose focus on Enter
/// </summary>
public class LockedInputField : InputField
{

    #region --------------------    Public Methods

    public void OnEndEdit(string _pValue)
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            ActivateInputField();
            Select();
        }
    }

    #endregion

    #region --------------------    Protected Methods

    /// <summary>
    /// Prevents copy/paste
    /// </summary>
    /// <param name="input"></param>
    protected override void Append(string input) { }

    #endregion

    #region --------------------    Private Methods

    private new void Awake()
    {
        base.Awake();
        onEndEdit.AddListener(OnEndEdit);
    }

    #endregion

}