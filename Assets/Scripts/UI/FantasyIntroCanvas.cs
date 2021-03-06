using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FantasyIntroCanvas : UIScreenBase
{
    [SerializeField] GameObject[] pages;
    [SerializeField] GameObject[] playerNext;
    [SerializeField] TextMeshProUGUI confirmedText;
    [SerializeField] Animator clothAnimation;

    int currIndex;

    int confirmed;

    private void Awake()
    {
        AirConsole.instance.onMessage += OnMessage;
    }

    void OnMessage(int fromDeviceID, JToken data)
    {
        if (data["action"] != null && data["action"].ToString().Equals("next"))
        {
            SetNext(AirConsole.instance.ConvertDeviceIdToPlayerNumber(fromDeviceID));
            if (IsAllComfirmed())
            {
                ClearSelection();
                NextPage();
            }    
        }
    }

    public void NextPage()
    {
        currIndex++;

        if(currIndex >= pages.Length)
        {
            clothAnimation.Play("Out");
            pages[0].transform.parent.gameObject.SetActive(false);
            AirConsole.instance.Broadcast("Fantasy;Start");
            CustomerManager.instance.StartLevel();
            return;
        }

        foreach(GameObject obj in pages)
        {
            obj.SetActive(false);
        }

        pages[currIndex].SetActive(true);
    }

    public void SetNext(int playerId)
    {
        if (!playerNext[playerId].transform.Find("Check").gameObject.activeSelf)
        {
            MusicManager.instance.Play_confirm_pick_up();
        }
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

    void OnDestroy()
    {
        // unregister events
        if (AirConsole.instance != null)
        {
            AirConsole.instance.onMessage -= OnMessage;
        }
    }
}
