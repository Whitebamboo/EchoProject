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
    [SerializeField] GameObject connectPage;
    [SerializeField] GameObject introPage;
    [SerializeField] TextMeshProUGUI connectedPlayerText;
    [SerializeField] TextMeshProUGUI confirmedPlayerText;
    [SerializeField] TextMeshProUGUI confirmedText;
    [SerializeField] GameObject[] pages;
    [SerializeField] GameObject[] playerReady;

    int currIndex;
    int confirmed;

    //Start state variable
    int confirmedPlayer;

    private void Awake()
    {
        //SetConnectedPlayer(0);
        //SetConfirmedPlayer(0);

        AirConsole.instance.onReady += OnReady;
        AirConsole.instance.onConnect += OnConnect;
        AirConsole.instance.onDisconnect += OnDisconnect;
    }

    void OnReady(string code)
    {
        AirConsole.instance.onMessage += OnMessage;
    }

    void OnConnect(int device_id)
    {
        AirConsole.instance.SetActivePlayers();
        SetConnectedPlayer(AirConsole.instance.GetActivePlayerDeviceIds.Count);
    }

    void OnDisconnect(int device_id)
    {
        AirConsole.instance.SetActivePlayers();
        SetConnectedPlayer(AirConsole.instance.GetActivePlayerDeviceIds.Count);
    }

    public void SetConnectedPlayer(int playerId)
    {
        playerReady[playerId-1].SetActive(true);
        Image icon = playerReady[playerId-1].transform.Find("Icon").GetComponent<Image>();
        icon.color = GameManager.instance.GetPlayerColor(playerId - 1);
    }

    public void SetConfirmedPlayer(int playerId)
    {
        playerReady[playerId].transform.Find("Check").gameObject.SetActive(true);
        Image check = playerReady[playerId].transform.Find("Check").GetComponent<Image>();
        check.color = GameManager.instance.GetPlayerColor(playerId);
    }

    public void StartGame()
    {
        AirConsole.instance.Broadcast("Fantasy;Intro");
        connectPage.SetActive(false);
        introPage.SetActive(true);
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

        //Game ready to start
        if (data["action"] != null && data["action"].ToString().Equals("confirm"))
        {
            confirmedPlayer++;
            SetConfirmedPlayer(AirConsole.instance.ConvertDeviceIdToPlayerNumber(fromDeviceID));

            if (confirmedPlayer == 4)
            {
                StartGame();
            }
        }
    }

    public void NextPage()
    {
        currIndex++;

        if (currIndex >= pages.Length)
        {
            GameManager.instance.SwitchState(GameState.Fantasy);
            CloseScreen();
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
