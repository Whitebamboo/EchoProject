using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RedBlueGames.Tools.TextTyper;
using System;

public class DialogueCanvas : UIScreenBase
{
    [SerializeField] GameObject bg;
    [SerializeField] GameObject hint;
    [SerializeField] TextTyper typer;
    [SerializeField] float printSpeed = 0.1f;

    string currText;
    Action action;

    private void Start()
    {
        typer.PrintCompleted.AddListener(OnPrintingComplete);
        hint.SetActive(true);
        bg.SetActive(false);
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

        bg.SetActive(true);
        hint.SetActive(false);
        this.action = action;
        currText = text;
        typer.TypeText(text, printSpeed);
    }
}
