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

    public void FinishTutorial()
    {
        SwitchState(GameState.Fantasy);
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
                TutorialCanvas canvas = UIManager.instance.CreateScreen<TutorialCanvas>();
                canvas.Setup(levels);
                break;
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
