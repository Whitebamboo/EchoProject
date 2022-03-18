using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Start,
    Tutorial,
    Fantasy
}

public class GameManager : CSingletonMono<GameManager>
{ 
    [SerializeField]
    List<TutorialLevel> levels;

    [HideInInspector]
    public bool IsConnectedAirconsole;

    public GameState State;

    //Start state variable
    int comfirmedPlayer;

    public int GetActivePlayersNumber()
    {
        return AirConsole.instance.GetActivePlayerDeviceIds.Count;
    }

    protected override void Awake()
    {
        base.Awake();

        AirConsole.instance.onReady += OnReady;
        AirConsole.instance.onConnect += OnConnect;
        AirConsole.instance.onDisconnect += OnDisconnect;
        AirConsole.instance.onMessage += OnMessage;
    }

    //@param code airconsole connection code
    void OnReady(string code)
    {
        IsConnectedAirconsole = true;
    }

    void OnConnect(int device_id)
    {
        AirConsole.instance.SetActivePlayers();

        StartCanvas startCanvas = UIManager.instance.FindScreen<StartCanvas>();
        if(startCanvas!=null)
        {
            startCanvas.SetConnectedPlayer(GetActivePlayersNumber());
        }
    }

    void OnDisconnect(int device_id)
    {
        AirConsole.instance.SetActivePlayers();

        StartCanvas startCanvas = UIManager.instance.FindScreen<StartCanvas>();
        if (startCanvas != null)
        {
            startCanvas.SetConnectedPlayer(GetActivePlayersNumber());
        }
    }

    void OnMessage(int fromDeviceID, JToken data)
    {
        Debug.Log("message from " + fromDeviceID + ", data: " + data);
        if (data["action"] != null && data["action"].ToString().Equals("confirm"))
        {
            StartCanvas startCanvas = UIManager.instance.FindScreen<StartCanvas>();
            if (startCanvas != null)
            {
                comfirmedPlayer++;
                startCanvas.SetConfirmedPlayer(comfirmedPlayer);

                if(comfirmedPlayer == GetActivePlayersNumber())
                {
                    startCanvas.StartGame(() =>
                    {
                        startCanvas.CloseScreen();

                        SwitchState(GameState.Tutorial);
                        LoadTutorialLevel(0);
                    });
                }
            }
        }
    }

    void Start()
    {
        SwitchState(GameState.Start);
    }

    void SwitchState(GameState state)
    {
        State = state;

        switch (state)
        {
            case GameState.Start:
                UIManager.instance.CreateScreen<StartCanvas>();
                break;
            case GameState.Tutorial:
                AirConsole.instance.Broadcast("Tutorial;Start");
                UIManager.instance.CreateScreen<TutorialCanvas>();
                break;
        }
    }

    void LoadTutorialLevel(int level)
    {
        Debug.Log("Load level");
        if(level < 0 || (level + 1) > levels.Count)
        {
            return;
        }

        TutorialCanvas tutorialCanvas = UIManager.instance.FindScreen<TutorialCanvas>();
        if (tutorialCanvas != null)
        {
            Debug.Log("Load level");
            tutorialCanvas.LoadLevel(levels[level]);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        // unregister events
        if (AirConsole.instance != null)
        {
            AirConsole.instance.onReady -= OnReady;
        }
    }
}
