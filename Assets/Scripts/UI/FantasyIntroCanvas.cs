using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FantasyIntroCanvas : UIScreenBase
{
    [SerializeField] GameObject[] pages;
    [SerializeField] TextMeshProUGUI confirmedText;

    int currIndex;

    int confirmed;

    private void Awake()
    {
        AirConsole.instance.onMessage += OnMessage;
    }

    void OnMessage(int fromDeviceID, JToken data)
    {
        Debug.Log("message from " + fromDeviceID + ", data: " + data);

        if (data["action"] != null && data["action"].ToString().Equals("next"))
        {
            confirmed++;
            confirmedText.text = "Confirmed: " + confirmed + "/" + GameManager.instance.GetActivePlayersNumber().ToString();

            if(confirmed == GameManager.instance.GetActivePlayersNumber())
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

    void OnDestroy()
    {
        // unregister events
        if (AirConsole.instance != null)
        {
            AirConsole.instance.onMessage -= OnMessage;
        }
    }
}
