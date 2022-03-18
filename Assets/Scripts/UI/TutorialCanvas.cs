using NDream.AirConsole;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialCanvas : UIScreenBase
{
    [SerializeField]
    TextMeshProUGUI levelDescription;

    [SerializeField] 
    Image[] images;

    List<TutorialLevel> m_leves;

    int m_currLevelIndex;

    string[] m_selectList;

    WeightedRandomGenerator<int> m_playerRandom;

    Dictionary<int, string> m_clothAssignment = new Dictionary<int, string>();

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
        ResetPlayerRandom(playersNumber);

        m_currLevelIndex = levelIndex;
        TutorialLevel levelData = m_leves[m_currLevelIndex];
        levelDescription.text = levelData.levelDescription;
        for (int i = 0; i < playersNumber; i++)
        {
            int playerIndex = m_playerRandom.GetRandomEntry();
            AirConsole.instance.Message(AirConsole.instance.ConvertPlayerNumberToDeviceId(playerIndex),
                string.Format("Tutorial;Cloth;{0};{1};{2}", levelIndex + 1, levelData.clothes[i].image.name, levelData.clothes[i].clothDescription));
            m_clothAssignment.Add(AirConsole.instance.ConvertPlayerNumberToDeviceId(playerIndex), levelData.clothes[i].image.name);
        }
    }

    void OnMessage(int fromDeviceID, JToken data)
    {
        Debug.Log("message from " + fromDeviceID + ", data: " + data);
        if (data["action"] != null)
        {
            int input = int.Parse(data["action"].ToString());
            RemoveSprite(m_clothAssignment[fromDeviceID]);
            m_selectList[input-1] = m_clothAssignment[fromDeviceID];
            UpdateSprite(m_leves[m_currLevelIndex]);

            if(IsFilled())
            {
                if(IsCorrect(m_leves[m_currLevelIndex]))
                {
                    m_currLevelIndex++;
                    if(m_currLevelIndex == m_leves.Count)
                    {
                        GameManager.instance.FinishTutorial();
                        CloseScreen();
                    }
                    else
                    {
                        LoadLevel(++m_currLevelIndex);
                    }
                }
                else
                {
                    ClearSprite();
                }
            }
        }
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
                images[i].sprite = null;
            }
        }
    }

    void UpdateSprite(TutorialLevel level)
    {
        for(int i = 0; i < m_selectList.Length; i++)
        {
            for(int j = 0; j < level.clothes.Count; j++)
            {
                if (m_selectList[i] == level.clothes[j].image.name)
                {
                    images[i].sprite = level.clothes[j].image;
                }
            }
        }
    }

    void ClearSprite()
    {
        for (int i = 0; i < m_selectList.Length; i++)
        {
            m_selectList[i] = "";
            images[i].sprite = null;
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
