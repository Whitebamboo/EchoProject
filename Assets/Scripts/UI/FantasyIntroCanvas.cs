using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FantasyIntroCanvas : UIScreenBase
{
    [SerializeField] GameObject[] pages;

    int currIndex;

    private void Awake()
    {
        AirConsole.instance.onMessage += OnMessage;
    }

    void OnMessage(int fromDeviceID, JToken data)
    {
        Debug.Log("message from " + fromDeviceID + ", data: " + data);

        if (data["action"] != null && data["action"].ToString().Equals("next"))
        {
            NextPage();
        }
    }

    public void NextPage()
    {
        currIndex++;

        if(currIndex >= pages.Length)
        {
            CloseScreen();
            AirConsole.instance.Broadcast("Fantasy;Start");
            return;
        }

        foreach(GameObject obj in pages)
        {
            obj.SetActive(false);
        }

        pages[currIndex].SetActive(true);
    }
}
