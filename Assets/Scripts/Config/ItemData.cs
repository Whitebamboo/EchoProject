using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Config/Item")]
public class ItemData : ScriptableObject
{
    public GameObject prefab;

    public Sprite clothImage;

    public string description;
}
