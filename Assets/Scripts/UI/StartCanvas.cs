using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class StartCanvas : UIScreenBase
{
    [SerializeField]
    GameObject connectPage;

    [SerializeField]
    TextMeshProUGUI connectedPlayerText;

    [SerializeField]
    TextMeshProUGUI confirmedPlayerText;

    public TextMeshProUGUI introductionText;

    [SerializeField] GameObject[] pages;
    [SerializeField] TextMeshProUGUI confirmedText;
    [SerializeField] GameObject introPage;

    int currIndex;

    int confirmed;

    Action m_callback;

    private void Awake()
    {
        SetConnectedPlayer(0);
        SetConfirmedPlayer(0);

        AirConsole.instance.onReady += OnReady;      
    }

    void OnReady(string code)
    {
        AirConsole.instance.onMessage += OnMessage;
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
        introPage.SetActive(true);
        m_callback = callback;
        AirConsole.instance.Broadcast("Fantasy;Intro");
        //introductionText.gameObject.SetActive(true);
        //introductionText.DOFade(0, 1f).From().OnComplete(() => introductionText.DOFade(0f, 1f).SetDelay(3f).OnComplete(() => callback.Invoke()));
    }

    void OnMessage(int fromDeviceID, JToken data)
    {
        Debug.Log("message from " + fromDeviceID + ", data: " + data);

        if (data["action"] != null && data["action"].ToString().Equals("next"))
        {
            confirmed++;
            confirmedText.text = "Confirmed: " + confirmed + "/" + GameManager.instance.GetActivePlayersNumber().ToString();

            if (confirmed == GameManager.instance.GetActivePlayersNumber())
            {
                NextPage();
                confirmed = 0;
                confirmedText.text = "Confirmed: " + confirmed + "/" + GameManager.instance.GetActivePlayersNumber().ToString();
            }
        }
    }

    public void NextPage()
    {
        currIndex++;

        if (currIndex >= pages.Length)
        {
            CloseScreen();
            m_callback.Invoke();
            return;
        }

        foreach (GameObject obj in pages)
        {
            obj.SetActive(false);
        }

        pages[currIndex].SetActive(true);
    }

    void OnDestroy()
    {
        // unregister events
        if (AirConsole.instance != null)
        {
            AirConsole.instance.onMessage -= OnMessage;
        }
    }
}
