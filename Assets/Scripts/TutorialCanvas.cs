using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialCanvas : MonoBehaviour
{
    public Button startButton;

    public Image[] images;
    public Sprite[] sprites;

    private void Awake()
    {
        AirConsole.instance.onMessage += OnMessage;
    }

    public void StartGame()
    {
        startButton.gameObject.SetActive(false);

        int playersNumber = GameManager.instance.GetActivePlayersNumber();

        for(int i = 0; i < playersNumber; i++)
        {
            AirConsole.instance.Message(AirConsole.instance.ConvertPlayerNumberToDeviceId(i), i + 1);
        }
    }

    void OnMessage(int fromDeviceID, JToken data)
    {
        Debug.Log("message from " + fromDeviceID + ", data: " + data);
        if (data["action"] != null)
        {
            int input = int.Parse(data["action"].ToString());

            images[input-1].sprite = sprites[AirConsole.instance.ConvertDeviceIdToPlayerNumber(fromDeviceID)];
        }
    }

    private void OnDestroy()
    {
        if (AirConsole.instance != null)
        {
            AirConsole.instance.onMessage -= OnMessage;
        }
    }
}
