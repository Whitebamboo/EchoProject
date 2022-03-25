using System.Collections;
using System.Collections.Generic;
using System.Text;
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

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("Color: ");
        sb.Append(color.ToString());
        sb.Append("<br>");

        if(properties.Count == 0)
        {
            sb.Append("No Label");
        }
        else
        {
            sb.Append("Material:");
            foreach (MaterialProperty material in properties)
            {
                sb.Append(material.material.ToString());
                sb.Append(" ");
                sb.Append(material.percentage.ToString());
                sb.Append("%<br>");
            }
        }

        if(year != 0)
        {
            sb.Append("Durability: ");
            sb.Append("This cloth can be worn for " + year + " years");
        }

        return sb.ToString();
    }
}

[System.Serializable]
public class MaterialProperty
{
    public ClothMaterial material;
    public float percentage;
}
