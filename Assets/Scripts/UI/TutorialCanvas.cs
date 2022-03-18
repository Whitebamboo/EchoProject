using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialCanvas : UIScreenBase
{
    [SerializeField]
    TextMeshProUGUI levelDescription;

    [SerializeField] 
    Image[] images;

    TutorialLevel currLevel;

    string[] selectList;

    Dictionary<int, string> clothAssignment = new Dictionary<int, string>();

    private void Awake()
    {
        AirConsole.instance.onMessage += OnMessage;
    }

    public void LoadLevel(TutorialLevel level)
    {
        currLevel = level;
        levelDescription.text = level.levelDescription;

        int playersNumber = GameManager.instance.GetActivePlayersNumber();
        selectList = new string[4];
        for (int i = 0; i < playersNumber; i++)
        {
            AirConsole.instance.Message(AirConsole.instance.ConvertPlayerNumberToDeviceId(i), 
                "Tutorial;Cloth;" + currLevel.clothes[i].image.name + ";" + currLevel.clothes[i].clothDescription);
            Debug.Log(AirConsole.instance.ConvertPlayerNumberToDeviceId(i));
            clothAssignment.Add(AirConsole.instance.ConvertPlayerNumberToDeviceId(i), currLevel.clothes[i].image.name);
        }
    }

    void OnMessage(int fromDeviceID, JToken data)
    {
        Debug.Log("message from " + fromDeviceID + ", data: " + data);
        if (data["action"] != null)
        {
            int input = int.Parse(data["action"].ToString());
            RemoveSprite(clothAssignment[fromDeviceID]);
            selectList[input-1] = clothAssignment[fromDeviceID];
            UpdateSprite();

            if(IsFilled())
            {
                if(IsCorrect())
                {
                    Debug.Log("Correct");
                }
                else
                {
                    for (int i = 0; i < selectList.Length; i++)
                    {
                        selectList[i] = "";
                        images[i].sprite = null;
                    }
                }
            }
        }
    }

    void RemoveSprite(string name)
    {
        for (int i = 0; i < selectList.Length; i++)
        {
            if (selectList[i] == name)
            {
                selectList[i] = "";
                images[i].sprite = null;
            }
        }
    }

    void UpdateSprite()
    {
        for(int i = 0; i < selectList.Length; i++)
        {
            for(int j = 0; j < currLevel.clothes.Count; j++)
            {
                if (selectList[i] == currLevel.clothes[j].image.name)
                {
                    images[i].sprite = currLevel.clothes[j].image;
                }
            }
        }
    }

    bool IsFilled()
    {
        for (int i = 0; i < selectList.Length; i++)
        {
            if (string.IsNullOrEmpty(selectList[i]))
            {
                return false;
            }
        }

        return true;
    }

    bool IsCorrect()
    {
        for (int i = 0; i < selectList.Length; i++)
        {
            if (selectList[i] != currLevel.clothes[i].image.name)
            {
                return false;
            }
        }

        return true;
    }

    private void OnDestroy()
    {
        if (AirConsole.instance != null)
        {
            AirConsole.instance.onMessage -= OnMessage;
        }
    }
}
