using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CustomerPopup : MonoBehaviour
{
    [SerializeField] GameObject interactHint;
    [SerializeField] TextMeshProUGUI dialogText;
    [SerializeField] UI_Follow3D uiFollow;

    public void SetText(string text)
    {
        interactHint.SetActive(false);
        //dialogText.transform.parent.gameObject.SetActive(true);
        //uiFollow.offset3D = new Vector3(3f, 0, 0);
        //dialogText.text = text;
    }
}
