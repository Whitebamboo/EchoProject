using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class StartCanvas : UIScreenBase
{
    [SerializeField]
    GameObject connectPage;

    [SerializeField]
    TextMeshProUGUI connectedPlayerText;

    [SerializeField]
    TextMeshProUGUI confirmedPlayerText;

    public TextMeshProUGUI introductionText;

    private void Awake()
    {
        SetConnectedPlayer(0);
        SetConfirmedPlayer(0);
    }

    public void SetConnectedPlayer(int number)
    {
        connectedPlayerText.text = "Connected Players: " + number;
    }

    public void SetConfirmedPlayer(int number)
    {
        confirmedPlayerText.text = "Confirmed Players: " + number;
    }

    public void StartGame(Action callback)
    {
        connectPage.SetActive(false);
        introductionText.gameObject.SetActive(true);
        introductionText.DOFade(0, 1f).From().OnComplete(() => introductionText.DOFade(0f, 1f).SetDelay(3f).OnComplete(() => callback.Invoke()));
    }
}
