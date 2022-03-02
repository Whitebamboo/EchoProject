using NDream.AirConsole;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : CSingletonMono<GameManager>
{
    public List<TutorialLevel> levels;

    [HideInInspector]
    public bool IsConnectedAirconsole;

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
        print("OnDisconnect: " + device_id);
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
