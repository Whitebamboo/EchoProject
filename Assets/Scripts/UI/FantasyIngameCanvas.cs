using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FantasyIngameCanvas : UIScreenBase
{
    [SerializeField] List<FantasyClothUI> clothUI;

    float disablePos;
    float showPos;
    float height;

    void Start()
    {
        RectTransform rTrans = clothUI[0].GetComponent<RectTransform>();
        float startPoint = rTrans.anchoredPosition.y;
        height = rTrans.rect.height;
        disablePos = startPoint;
        showPos = startPoint - height;
    }

    public void ShowClothDesc(int playerIndex, ItemData data)
    {
        clothUI[playerIndex].SetDesc(Color.white, data.clothImage, data.ToString());

        RectTransform rTrans = clothUI[playerIndex].GetComponent<RectTransform>();
        rTrans.anchoredPosition = new Vector2(rTrans.anchoredPosition.x, showPos);
    }

    public void DisableClothDesc(int playerIndex)
    {
        RectTransform rTrans = clothUI[playerIndex].GetComponent<RectTransform>();
        //rTrans.DOAnchorPosY(disablePos, 1f);
        rTrans.anchoredPosition = new Vector2(rTrans.anchoredPosition.x, disablePos);
    }
}
