using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using RedBlueGames.Tools.TextTyper;

public enum TutorialState
{
    Lesson,
    Play,
    Congrat
}

public class TutorialCanvas : UIScreenBase
{
    [SerializeField] TextMeshProUGUI levelTitle;
    [SerializeField] TextMeshProUGUI hintText;
    [SerializeField] TextMeshProUGUI lowExtentText;
    [SerializeField] TextMeshProUGUI highExtentText;
    [SerializeField] GameObject introPage;
    [SerializeField] TextMeshProUGUI lessonTitle;
    [SerializeField] TextTyper lessonDesc;
    [SerializeField] GameObject playPage;
    [SerializeField] GameObject progressPage;
    [SerializeField] TextMeshProUGUI progressTitle;
    [SerializeField] TextTyper progressDescription;
    [SerializeField] TutorialController[] players;
    [SerializeField] Image[] displayImageHolder;
    [SerializeField] Sprite[] colorSprites; //0 green 1 yellow 2 red 3 blue
    [SerializeField] GameObject[] selectionComfirm;
    [SerializeField] List<FantasyClothUI> clothUI;
    [SerializeField] List<GameObject> lessonComfirm;
    [SerializeField] List<GameObject> congratConfirm;
    [SerializeField] Animator congratAnimator;
    [SerializeField] GameObject clothAnimation;
    [SerializeField] AnimationClip clothIn;

    List<TutorialLevel> m_leves;

    int m_currLevelIndex;

    string[] m_selectList;

    WeightedRandomGenerator<int> m_playerRandom;

    Dictionary<int, string> m_clothAssignment = new Dictionary<int, string>();

    bool inLevelTransition;

    public TutorialState State;

    int missingPlayerId = -1;

    private void Awake()
    {
        AirConsole.instance.onMessage += OnMessage;
        AirConsole.instance.onConnect += OnConnect;
        AirConsole.instance.onDisconnect += OnDisconnect;
    }

    private void OnConnect(int deviceId)
    {
        if(missingPlayerId != -1)
        {
            Debug.Log("connect: " + missingPlayerId + ":" + deviceId);
            AirConsole.instance.SetPlayerIdBydeviceID(missingPlayerId, deviceId);
            AirConsole.instance.Message(deviceId, "Start;Controller;" + (AirConsole.instance.ConvertDeviceIdToPlayerNumber(deviceId) + 1));
            AirConsole.instance.Message(deviceId, "Tutorial;Controller;" + (AirConsole.instance.ConvertDeviceIdToPlayerNumber(deviceId) + 1));
            AirConsole.instance.Message(deviceId, "Tutorial;Start");
            players[missingPlayerId].SetupPlayer(missingPlayerId, displayImageHolder[missingPlayerId].GetComponent<RectTransform>(), OnConfirm);

            missingPlayerId = -1;
        }        
    }

    private void OnDisconnect(int deviceId)
    {
        Debug.Log("disconnt: " + missingPlayerId + ":" + deviceId);
        missingPlayerId = AirConsole.instance.ConvertDeviceIdToPlayerNumber(deviceId);
    }

    public void Setup(List<TutorialLevel> levels)
    {
        m_leves = levels;

        LoadLevel(0);

        for (int i = 0; i < players.Length; i++)
        {
            players[i].SetupPlayer(i, displayImageHolder[i].GetComponent<RectTransform>(), OnConfirm);
        }

        SwitchState(TutorialState.Lesson);
    }

    void SwitchState(TutorialState state)
    {
        State = state;

        switch (state)
        {
            case TutorialState.Lesson:
                ResetConfirmed(lessonComfirm);
                ResetConfirmed(congratConfirm);
                AssignColor(lessonComfirm);
                introPage.SetActive(true);
                playPage.SetActive(false);
                lessonTitle.text = m_leves[m_currLevelIndex].lessonName;
                lessonDesc.TypeText(m_leves[m_currLevelIndex].levelDescription);
                break;
            case TutorialState.Play:
                ResetConfirmed(lessonComfirm);
                introPage.SetActive(false);
                playPage.SetActive(true);
                ResetPlayers();
                break;
            case TutorialState.Congrat:
                ResetConfirmed(congratConfirm);
                playPage.SetActive(false);
                congratAnimator.Play("In");
                progressDescription.TypeText(m_leves[m_currLevelIndex].winDescription);
                AssignColor(congratConfirm);
                break;
        }
    }

    public void LoadLevel(int levelIndex)
    {
        Debug.Assert(levelIndex >= 0 && levelIndex < m_leves.Count);

        SwitchState(TutorialState.Lesson);

        int playersNumber = GameManager.instance.GetActivePlayersNumber();
        m_selectList = new string[playersNumber];
        m_clothAssignment.Clear();

        ClearSelection();
        ResetPlayerRandom(playersNumber);

        m_currLevelIndex = levelIndex;
        TutorialLevel levelData = m_leves[m_currLevelIndex];
        hintText.text = m_leves[m_currLevelIndex].command;
        levelTitle.text = levelData.levelTitle;
        lowExtentText.text = levelData.lowExtentTitle;
        highExtentText.text = levelData.highExtentTitle;
        for (int i = 0; i < playersNumber; i++)
        {
            int clothIndex = m_playerRandom.GetRandomEntry();
            m_clothAssignment.Add(AirConsole.instance.ConvertPlayerNumberToDeviceId(i), levelData.clothes[clothIndex].image.name);
            clothUI[i].SetDesc(Color.white, levelData.clothes[clothIndex].image, levelData.clothes[clothIndex].clothDescription);
            displayImageHolder[i].transform.Find("Confirm").GetComponent<Image>().color = GameManager.instance.GetPlayerColor(i);
        }
    }

    void OnConfirm()
    {
        if (inLevelTransition)
        {
            return;
        }

        if (IsAllComfirmed())
        {
            if (!IsCorrect(m_leves[m_currLevelIndex]))
            {
                WrongSelection();
            }
            else
            {
                SwitchState(TutorialState.Congrat);
            }
        }

        return;
    }

    void OnMessage(int fromDeviceID, JToken data)
    {
        Debug.Log("message from " + fromDeviceID + ", data: " + data);

        if (data["action"] != null && data["action"].ToString().Equals("confirm"))
        {
            if(State == TutorialState.Lesson)
            {
                SetLessonConfirmedPlayer(AirConsole.instance.ConvertDeviceIdToPlayerNumber(fromDeviceID));
                if(AllConfirmed(lessonComfirm))
                {
                    StartCoroutine(ToPlay());
                }
            }
            else if(State == TutorialState.Congrat)
            {
                SetCongartConfirmedPlayer(AirConsole.instance.ConvertDeviceIdToPlayerNumber(fromDeviceID));
                if (AllConfirmed(congratConfirm))
                {
                    CorrectSelection();
                }
            }
        }
    }

    IEnumerator ToPlay()
    {
        yield return null;
        SwitchState(TutorialState.Play);
    }

    public void SetLessonConfirmedPlayer(int playerId)
    {
        lessonComfirm[playerId].transform.Find("Check").gameObject.SetActive(true);
        Image check = lessonComfirm[playerId].transform.Find("Check").GetComponent<Image>();
        check.color = GameManager.instance.GetPlayerColor(playerId);
    }

    public void SetCongartConfirmedPlayer(int playerId)
    {
        congratConfirm[playerId].transform.Find("Check").gameObject.SetActive(true);
        Image check = congratConfirm[playerId].transform.Find("Check").GetComponent<Image>();
        check.color = GameManager.instance.GetPlayerColor(playerId);
    }

    void WrongSelection()
    {
        ClearSelection();

        ResetPlayers();

        hintText.text = m_leves[m_currLevelIndex].loseDescription;
    }

    void CorrectSelection()
    {
        m_currLevelIndex++;
        if (m_currLevelIndex == m_leves.Count)
        {
            clothAnimation.SetActive(true);
            StartCoroutine(FinishTutorial());
        }
        else
        {
            congratAnimator.Play("Out");
            LoadLevel(m_currLevelIndex);
            SwitchState(TutorialState.Lesson);
        }
        ResetPlayers();
    }

    IEnumerator FinishTutorial()
    {
        yield return new WaitForSeconds(clothIn.length);
        GameManager.instance.FinishTutorial();
        CloseScreen();
    }

    void ResetPlayerRandom(int playerNum)
    {
        m_playerRandom = new WeightedRandomGenerator<int>("UniqueRandom");
        for(int i = 0; i < playerNum; i++)
        {
            m_playerRandom.AddEntry(i, 1d / playerNum);
        }
    }

    void ClearSelection()
    {
        for(int i = 0; i < selectionComfirm.Length; i++)
        {
            selectionComfirm[i].SetActive(false);
        }
    }


    bool IsAllComfirmed()
    {
        for (int i = 0; i < selectionComfirm.Length; i++)
        {
            if(!selectionComfirm[i].activeSelf)
            {
                return false;
            }
        }
        return true;
    }

    bool IsCorrect(TutorialLevel level)
    {
        List<TutorialController> display = new List<TutorialController>(players);
        List<TutorialController> sortedList = display.OrderBy(x => x.GetXpos()).ToList();

        for (int i = 0; i < sortedList.Count; i++)
        {
            if (m_clothAssignment[sortedList[i].GetDeviceId()] != level.clothes[i].image.name)
            {
                return false;
            }
        }

        return true;
    }

    void ResetPlayers()
    {
        for (int i = 0; i < players.Length; i++)
        {
            players[i].ResetPlayer();
        }
    }

    void AssignColor(List<GameObject> players)
    {

    }

    bool AllConfirmed(List<GameObject> players)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (!players[i].transform.Find("Check").gameObject.activeSelf)
            {
                return false;
            }
        }

        return true;
    }

    void ResetConfirmed(List<GameObject> players)
    {
        for (int i = 0; i < players.Count; i++)
        {
            players[i].transform.Find("Check").gameObject.SetActive(false);
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
