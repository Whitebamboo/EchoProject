using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueCanvas : UIScreenBase
{
    [SerializeField] TextMeshProUGUI dialogText;

    public float displayTime = 10f;

    float timer;

    private void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            CloseScreen();
        }
    }

    public void SetUp(string text)
    {
        dialogText.text = text;
        timer = displayTime;
    }
}
