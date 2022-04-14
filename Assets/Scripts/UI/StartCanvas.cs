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
    [SerializeField] GameObject[] playerNext;

    int currIndex;
    int confirmed;

    //Start state variable
    int confirmedPlayer;

    private void Awake()
    {
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
        TextMeshProUGUI nickName = playerReady[playerId-1].transform.Find("NickName").GetComponent<TextMeshProUGUI>();
        nickName.color = GameManager.instance.GetPlayerColor(playerId-1);
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
            SetNext(AirConsole.instance.ConvertDeviceIdToPlayerNumber(fromDeviceID));
            if (IsAllComfirmed())
            {
                ClearSelection();
                NextPage();
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


    public void SetNext(int playerId)
    {
        playerNext[playerId].transform.Find("Check").gameObject.SetActive(true);
        Image check = playerNext[playerId].transform.Find("Check").GetComponent<Image>();
        check.color = GameManager.instance.GetPlayerColor(playerId);
    }

    void ClearSelection()
    {
        for (int i = 0; i < playerNext.Length; i++)
        {
            playerNext[i].transform.Find("Check").gameObject.SetActive(false);
        }
    }


    bool IsAllComfirmed()
    {
        for (int i = 0; i < playerNext.Length; i++)
        {
            if (!playerNext[i].transform.Find("Check").gameObject.activeSelf)
            {
                return false;
            }
        }
        return true;
    }

    public void NextPage()
    {
        currIndex++;

        if (currIndex >= pages.Length)
        {
            GameManager.instance.SwitchState(GameState.Tutorial);
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
