using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IIteractable
{
    void OnInteract(PlayerController player, Item item = null);

    void OnShowHint(GameObject hintObj);

    void OnHideHint(GameObject hintObj);
}
