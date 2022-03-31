using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ClothPopup : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_clothText;
    [SerializeField] Image clothImage;

    public void SetImage(Sprite image)
    {
        clothImage.sprite = image;
    }

    public void SetText(string desc)
    {
        m_clothText.text = desc;
    }
}
