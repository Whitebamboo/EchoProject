using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialCanvas : UIScreenBase
{
    [SerializeField] TextMeshProUGUI levelTitle;
    [SerializeField] TextMeshProUGUI levelDescription;
    [SerializeField] TextMeshProUGUI lowExtentText;
    [SerializeField] TextMeshProUGUI highExtentText;
    [SerializeField] Image[] displayImageHolder;
    [SerializeField] Sprite[] colorSprites; //0 green 1 yellow 2 red 3 blue
    [SerializeField] GameObject[] selectionComfirm;
    [SerializeField] GameObject progressPage;
    [SerializeField] TextMeshProUGUI progressTitle;
    [SerializeField] TextMeshProUGUI progressDescription;

    List<TutorialLevel> m_leves;

    int m_currLevelIndex;

    string[] m_selectList;

    WeightedRandomGenerator<int> m_playerRandom;

    Dictionary<int, string> m_clothAssignment = new Dictionary<int, string>();

    bool inLevelTransition;

    private void Awake()
    {
        AirConsole.instance.onMessage += OnMessage;
    }

    public void Setup(List<TutorialLevel> levels)
    {
        m_leves = levels;

        LoadLevel(0);
    }

    public void LoadLevel(int levelIndex)
    {
        Debug.Assert(levelIndex >= 0 && levelIndex < m_leves.Count);

        int playersNumber = GameManager.instance.GetActivePlayersNumber();
        m_selectList = new string[playersNumber];
        m_clothAssignment.Clear();
        ClearSprite();
        ClearSelection();
        ResetPlayerRandom(playersNumber);

        m_currLevelIndex = levelIndex;
        TutorialLevel levelData = m_leves[m_currLevelIndex];
        levelDescription.text = levelData.levelDescription;
        levelTitle.text = levelData.levelTitle;
        lowExtentText.text = levelData.lowExtentTitle;
        highExtentText.text = levelData.highExtentTitle;
        for (int i = 0; i < playersNumber; i++)
        {
            int playerIndex = m_playerRandom.GetRandomEntry();
            AirConsole.instance.Message(AirConsole.instance.ConvertPlayerNumberToDeviceId(playerIndex),
                string.Format("Tutorial;Cloth;{0};{1};{2};{3};{4};{5}", levelIndex + 1, levelData.clothes[i].image.name, levelData.clothes[i].clothDescription, 
                lowExtentText.text, highExtentText.text, levelData.levelTitle));
            m_clothAssignment.Add(AirConsole.instance.ConvertPlayerNumberToDeviceId(playerIndex), levelData.clothes[i].image.name);
        }
    }

    void OnMessage(int fromDeviceID, JToken data)
    {
        Debug.Log("message from " + fromDeviceID + ", data: " + data);

        if(data["action"] != null && data["action"].ToString().Equals("confirm"))
        {
            if(inLevelTransition)
            {
                return;
            }

            int index = GetSelectionIndex(fromDeviceID);
            if(index == -1)
            {
                return;
            }

            selectionComfirm[index].SetActive(true);

            if (IsFilled() && IsAllComfirmed())
            {
                if (IsCorrect(m_leves[m_currLevelIndex]))
                {
                    StartCoroutine(CorrectSelection());
                }
                else
                {
                    StartCoroutine(WrongSelection());
                }
            }

            return;
        }

        if (data["action"] != null)
        {
            int index = GetSelectionIndex(fromDeviceID);
            int input = int.Parse(data["action"].ToString());

            if (index != -1 && IsConfirmed(index))
            {
                return;
            }

            if (IsConfirmed(input-1))
            {
                return;
            }

            RemoveSprite(m_clothAssignment[fromDeviceID]);
            m_selectList[input-1] = m_clothAssignment[fromDeviceID];
            UpdateSprite();
        }
    }

    IEnumerator WrongSelection()
    {
        ClearSprite();
        ClearSelection();
        progressPage.SetActive(true);
        progressTitle.text = "Try Again!";
        progressDescription.text = m_leves[m_currLevelIndex].loseDescription;
        inLevelTransition = true;

        yield return new WaitForSeconds(5f);

        AirConsole.instance.Broadcast("Tutorial;Hint;"+ m_leves[m_currLevelIndex].loseDescription);

        progressPage.SetActive(false);
        inLevelTransition = false;
    }

    IEnumerator CorrectSelection()
    {
        progressPage.SetActive(true);
        progressTitle.text = "Congratulations!\nYou put the garments\nin the correct order!";
        progressDescription.text = m_leves[m_currLevelIndex].winDescription;
        inLevelTransition = true;

        yield return new WaitForSeconds(8f);
        m_currLevelIndex++;
        if (m_currLevelIndex == m_leves.Count)
        {
            GameManager.instance.FinishTutorial();
            CloseScreen();
        }
        else
        {
            progressPage.SetActive(false);
            LoadLevel(m_currLevelIndex);
        }
        inLevelTransition = false;
    }

    int GetSelectionIndex(int fromDeviceID)
    {
        string assignedCloth = m_clothAssignment[fromDeviceID];

        for(int i = 0; i < m_selectList.Length; i++)
        {
            if(m_selectList[i] == assignedCloth)
            {
                return i;
            }
        }

        return -1;
    }

    bool IsConfirmed(int index)
    {
        return selectionComfirm[index].activeSelf;
    }

    void ResetPlayerRandom(int playerNum)
    {
        m_playerRandom = new WeightedRandomGenerator<int>("UniqueRandom");
        for(int i = 0; i < playerNum; i++)
        {
            m_playerRandom.AddEntry(i, 1d / playerNum);
        }
    }

    void RemoveSprite(string name)
    {
        for (int i = 0; i < m_selectList.Length; i++)
        {
            if (m_selectList[i] == name)
            {
                m_selectList[i] = "";
                displayImageHolder[i].sprite = null;
            }
        }
    }

    void UpdateSprite()
    {
        for(int i = 0; i < m_selectList.Length; i++)
        {
            displayImageHolder[i].gameObject.SetActive(true);
            if (m_selectList[i].Contains("green"))
            {
                displayImageHolder[i].sprite = colorSprites[0];
            }
            else if(m_selectList[i].Contains("yellow"))
            {
                displayImageHolder[i].sprite = colorSprites[1];
            }
            else if (m_selectList[i].Contains("red"))
            {
                displayImageHolder[i].sprite = colorSprites[2];
            }
            else if (m_selectList[i].Contains("blue"))
            {
                displayImageHolder[i].sprite = colorSprites[3];
            }
            else
            {
                displayImageHolder[i].gameObject.SetActive(false);
            }       
        }
    }

    void ClearSprite()
    {
        for (int i = 0; i < m_selectList.Length; i++)
        {
            m_selectList[i] = "";
            displayImageHolder[i].sprite = null;
            displayImageHolder[i].gameObject.SetActive(false);
        }
    }

    void ClearSelection()
    {
        for(int i = 0; i < selectionComfirm.Length; i++)
        {
            selectionComfirm[i].SetActive(false);
        }
    }

    bool IsFilled()
    {
        for (int i = 0; i < m_selectList.Length; i++)
        {
            if (string.IsNullOrEmpty(m_selectList[i]))
            {
                return false;
            }
        }

        return true;
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
        for (int i = 0; i < m_selectList.Length; i++)
        {
            if (m_selectList[i] != level.clothes[i].image.name)
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
