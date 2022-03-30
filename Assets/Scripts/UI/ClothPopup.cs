using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ClothPopup : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_clothText;

    public void SetText(string desc)
    {
        m_clothText.text = desc;
    }
}
