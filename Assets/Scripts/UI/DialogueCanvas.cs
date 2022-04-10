using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RedBlueGames.Tools.TextTyper;
using System;

public class DialogueCanvas : UIScreenBase
{
    [SerializeField] TextTyper typer;
    [SerializeField] float printSpeed = 0.1f;

    string currText;
    Action action;

    private void Start()
    {
        typer.PrintCompleted.AddListener(OnPrintingComplete);
    }

    void OnPrintingComplete()
    {
        if(action != null)
        {
            action.Invoke();
        }
    }

    public void SetUp(string text, Action action = null)
    {
        if(string.Equals(text, currText))
        {
            return;
        }

        this.action = action;
        currText = text;
        typer.TypeText(text, printSpeed);
    }
}
