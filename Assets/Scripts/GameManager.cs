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

    [SerializeField]
    List<Vector3> playerPositions;

    [SerializeField]
    List<Color> playerColor;

    [HideInInspector]
    public bool IsConnectedAirconsole;

    public GameState State;


    public int GetActivePlayersNumber()
    {
        return AirConsole.instance.GetActivePlayerDeviceIds.Count;
    }

    public Color GetPlayerColor(int index)
    {
        if(index < 0 || index > GetActivePlayersNumber()-1)
        {
            Debug.LogError("Index out of bound");
            return Color.white;
        }

        return playerColor[index];
    }

    public void FinishTutorial()
    {
        SwitchState(GameState.Fantasy);
    }

    protected override void Awake()
    {
        base.Awake();

        AirConsole.instance.onReady += OnReady;
        AirConsole.instance.onMessage += OnMessage;
    }

    //@param code airconsole connection code
    void OnReady(string code)
    {
        IsConnectedAirconsole = true;       
    }

    void OnMessage(int fromDeviceID, JToken data)
    {

    }

    void Start()
    {
        SwitchState(GameState.Start);
    }

    public void SwitchState(GameState state)
    {
        State = state;

        switch (state)
        {
            case GameState.Start:
                UIManager.instance.CreateScreen<StartCanvas>();
                break;
            case GameState.Tutorial:

                TutorialCanvas canvas = UIManager.instance.CreateScreen<TutorialCanvas>();
                canvas.Setup(tutorialLevels);
                for(int i = 0; i < GetActivePlayersNumber(); i++)
                {
                    AirConsole.instance.Message(AirConsole.instance.ConvertPlayerNumberToDeviceId(i), "Tutorial;Controller;" + (i+1));
                }
                break;
            case GameState.Fantasy:
                UIManager.instance.CreateScreen<FollowUICanvas>();
                UIManager.instance.CreateScreen<FantasyIntroCanvas>();
                UIManager.instance.CreateScreen<FantasyIngameCanvas>();
                AirConsole.instance.Broadcast("Fantasy;Intro");
                MusicManager.instance.PlayPhrase2();
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
            player.transform.position = playerPositions[i];
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        // unregister events
        if (AirConsole.instance != null)
        {
            AirConsole.instance.onReady -= OnReady;
            AirConsole.instance.onMessage -= OnMessage;
        }
    }

    private void OnDrawGizmos()
    {
        foreach(Vector3 position in playerPositions)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(position, 1);
        }
    }
}
