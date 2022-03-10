using NDream.AirConsole;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
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
    }

    void Start()
    {
        SwitchState(GameState.Tutorial);        
    }

    void SwitchState(GameState state)
    {
        State = state;

        switch (state)
        {
            case GameState.Tutorial:
                UIManager.instance.CreateScreen<TutorialCanvas>();
                break;
        }
    }


    //@param code airconsole connection code
    void OnReady(string code)
    {
        IsConnectedAirconsole = true;
    }

    void OnConnect(int device_id)
    {
        AirConsole.instance.SetActivePlayers();
    }

    void OnDisconnect(int device_id)
    {
        AirConsole.instance.SetActivePlayers();
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
