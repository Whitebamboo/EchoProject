using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using System;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using UnityEngine.Video;

public class StartCanvas : UIScreenBase
{
    [SerializeField] GameObject connectPage;
    [SerializeField] GameObject introPage;
    [SerializeField] GameObject[] pages;
    [SerializeField] GameObject[] playerReady;
    [SerializeField] GameObject[] playerNext;
    [SerializeField] VideoPlayer video;

    [SerializeField] AnimationClip bubbleAnimation;

    int currIndex;
    int confirmedPlayer;
    bool isPlayingVideo;

    private void Awake()
    {
        AirConsole.instance.onReady += OnReady;
        AirConsole.instance.onConnect += OnConnect;
        AirConsole.instance.onDisconnect += OnDisconnect;
        video.loopPointReached += OnFinishVideo;
        video.prepareCompleted += OnPrepareVideo;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            video.frame = (long)video.frameCount - 10;
        }
    }

    void OnFinishVideo(VideoPlayer vp)
    {
        isPlayingVideo = false;
        AirConsole.instance.Broadcast("Fantasy;Intro");
        introPage.SetActive(true);
        video.transform.parent.gameObject.SetActive(false);
        StartCoroutine(Showtext());
    }

    void OnPrepareVideo(VideoPlayer vp)
    {
        isPlayingVideo = true;
        video.Play();
        AirConsole.instance.Broadcast("Video;Start");
    }

    void OnReady(string code)
    {
        AirConsole.instance.onMessage += OnMessage;
    }

    void OnConnect(int device_id)
    {
        AirConsole.instance.SetActivePlayers();
        SetConnectedPlayer(AirConsole.instance.GetActivePlayerDeviceIds.Count);
        CheckStatus();
        AirConsole.instance.Message(device_id, "Start;Controller;" + (AirConsole.instance.ConvertDeviceIdToPlayerNumber(device_id) + 1));
    }

    void OnDisconnect(int device_id)
    {
        AirConsole.instance.SetActivePlayers();
        SetConnectedPlayer(AirConsole.instance.GetActivePlayerDeviceIds.Count);
    }

    public void SetConnectedPlayer(int playerId)
    {
        playerReady[playerId-1].SetActive(true);
        TextMeshProUGUI nickName = playerReady[playerId-1].transform.Find("NickName").GetComponent<TextMeshProUGUI>();
        nickName.color = GameManager.instance.GetPlayerColor(playerId-1);
        nickName.text = AirConsole.instance.GetNickname(AirConsole.instance.ConvertPlayerNumberToDeviceId(playerId - 1));
    }

    public void SetConfirmedPlayer(int playerId)
    {
        if (!playerReady[playerId].transform.Find("Check").gameObject.activeSelf)
        {
            MusicManager.instance.Play_confirm_pick_up();
        }

        playerReady[playerId].transform.Find("Check").gameObject.SetActive(true);
        Image check = playerReady[playerId].transform.Find("Check").GetComponent<Image>();
        check.color = GameManager.instance.GetPlayerColor(playerId);
    }

    void CheckStatus()
    {
        for(int i = 0; i < playerReady.Length; i++)
        {
            if(!playerReady[i].activeSelf && i < AirConsole.instance.GetActivePlayerDeviceIds.Count - 1)
            {
                playerReady[i].SetActive(true);
                TextMeshProUGUI nickName = playerReady[i].transform.Find("NickName").GetComponent<TextMeshProUGUI>();
                nickName.color = GameManager.instance.GetPlayerColor(i);
                nickName.text = AirConsole.instance.GetNickname(AirConsole.instance.ConvertPlayerNumberToDeviceId(i));
            }
        }
    }

    public void StartGame()
    {
        StartCoroutine(ShowVideo());
    }

    IEnumerator ShowVideo()
    {
        yield return new WaitForSeconds(1);
        connectPage.SetActive(false);
        video.transform.parent.gameObject.SetActive(true);

        if (currIndex == 0)
        {
            video.url = System.IO.Path.Combine(Application.streamingAssetsPath, "Final_videoGuide.mp4");
            video.Prepare();
        }
    }

    IEnumerator Showtext()
    {
        yield return new WaitForSeconds(bubbleAnimation.length);
        pages[0].gameObject.SetActive(true);
        MusicManager.instance.Play_manager_talk();
    }

    void OnMessage(int fromDeviceID, JToken data)
    {
        Debug.Log("message from " + fromDeviceID + ", data: " + data);

        if(isPlayingVideo)
        {
            return;
        }

        if (data["action"] != null && data["action"].ToString().Equals("next"))
        {
            SetNext(AirConsole.instance.ConvertDeviceIdToPlayerNumber(fromDeviceID));
            if (IsAllComfirmed())
            {
                ClearSelection();
                NextPage();
            }
        }

        //Game ready to start
        if (data["action"] != null && data["action"].ToString().Equals("confirm"))
        {
            confirmedPlayer++;
            SetConfirmedPlayer(AirConsole.instance.ConvertDeviceIdToPlayerNumber(fromDeviceID));

            if (confirmedPlayer == 4)
            {
                StartGame();
            }
        }
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

    public void NextPage()
    {
        currIndex++;

        if (currIndex >= pages.Length)
        {
            GameManager.instance.SwitchState(GameState.Tutorial);
            CloseScreen();
            return;
        }

        foreach (GameObject obj in pages)
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
            AirConsole.instance.onConnect -= OnConnect;
            AirConsole.instance.onDisconnect -= OnDisconnect;
        }
    }
}
