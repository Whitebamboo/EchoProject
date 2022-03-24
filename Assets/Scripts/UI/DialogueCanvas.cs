using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueCanvas : UIScreenBase
{
    [SerializeField] TextMeshProUGUI dialogText;

    public void SetUp(string text)
    {
        dialogText.text = text;
        StartCoroutine(Close());
    }

    IEnumerator Close()
    {
        yield return new WaitForSeconds(5f);

        CloseScreen();
    }
}
