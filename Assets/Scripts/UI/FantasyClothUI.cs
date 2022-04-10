using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FantasyClothUI : MonoBehaviour
{
    [SerializeField] Image bgImage;
    [SerializeField] Image clothImage;
    [SerializeField] TextMeshProUGUI clothDesc;

    public void SetDesc(Color c, Sprite img, string desc)
    {
        bgImage.color = c;
        clothImage.sprite = img;
        clothDesc.text = desc;
    }
}
