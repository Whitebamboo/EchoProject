using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum ClothColor
{
    Bady_Yellow,
    Sunflower_Yellow,
    Medallion_Yellow,
    Yellow,
    Pink,
    Purple,
}

public enum ClothMaterial
{
    Cotton,
    Organic_Cotton,
    Recycled_Cotton,
    Modal,
    Polyester,
    Spandex,
    Nylon,
    Silk,
    Line,

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
                sb.Append(material.material.ToString().Replace("_"," "));
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

    public string GetPopupDescription()
    {
        StringBuilder sb = new StringBuilder();

        if (properties.Count == 0)
        {
            sb.Append("No Label\n");
        }
        else
        {
            sb.Append("Material:\n");
            foreach (MaterialProperty material in properties)
            {
                sb.Append(material.material.ToString().Replace("_", " "));
                sb.Append(" ");
                sb.Append(material.percentage.ToString() + "%" + "\n");
            }
        }

        if (year != 0)
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
