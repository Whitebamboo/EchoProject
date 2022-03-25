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
    List<TutorialLevel> tutorialLevels;

    [SerializeField]
    PlayerController playerPerfab;

    [HideInInspector]
    public bool IsConnectedAirconsole;

    public GameState State;

    //Start state variable
    int confirmedPlayer;

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
        //Debug.Log("message from " + fromDeviceID + ", data: " + data);
        if (data["action"] != null && data["action"].ToString().Equals("confirm"))
        {
            StartCanvas startCanvas = UIManager.instance.FindScreen<StartCanvas>();
            if (startCanvas != null)
            {
                confirmedPlayer++;
                startCanvas.SetConfirmedPlayer(confirmedPlayer);

                if(confirmedPlayer == GetActivePlayersNumber())
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
                canvas.Setup(tutorialLevels);
                break;
            case GameState.Fantasy:
                AirConsole.instance.Broadcast("Fantasy;Start");
                InitFantasyPhrase();
                break;
        }
    }

    void InitFantasyPhrase()
    {
        for(int i = 0; i < GetActivePlayersNumber(); i++)
        {
            PlayerController player = Instantiate(playerPerfab);
            player.SetupPlayerData(i);
            player.transform.position = new Vector3(i, 0.85f, 0);
        }

        CustomerManager.instance.StartLevel();
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
