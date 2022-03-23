using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ClothColor
{
    Yellow,
    Pink,
    Purple
}

public enum ClothMaterial
{
    Contton,
    Organic_Contton,
    Recycled_Contton,
    Polyester,
    Spandex,
    Nylon,
    Silk,
}

[CreateAssetMenu(fileName = "Item", menuName = "Config/In game cloth")]
public class ItemData : ScriptableObject
{
    public GameObject prefab;
    public Sprite clothImage;
    public ClothColor color;
    public string description;
    public int year;
    public int month;
    public List<MaterialProperty> properties;
}

[System.Serializable]
public class MaterialProperty
{
    public ClothMaterial material;
    public float percentage;
}
