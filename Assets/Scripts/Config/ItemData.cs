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
    Blue,
    Navy_Blue,
    White,
    Red,
    Christmas_Red,
    Bright_Red,
    Orange,
    Orange_Coral,
    Lavender,
    Black,
    Grey
}

public enum ClothMaterial
{
    Cotton,
    Conventional_Cotton,
    Organic_Cotton,
    Recycled_Cotton,
    Modal,
    Polyester,
    Recycled_Polyester,
    Spandex,
    Nylon,
    Silk,
    Linen,
    Acrylic,
    Wool
}

[CreateAssetMenu(fileName = "Item", menuName = "Config/In game cloth")]
public class ItemData : ScriptableObject
{
    public GameObject prefab;
    public Sprite clothImage;
    public List<ClothColor> color;
    public string description;
    public int year;
    public int month;
    public List<MaterialProperty> properties;

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append("Color: ");
        foreach(ClothColor c in color)
        {
            sb.Append(c.ToString().Replace("_", " ") + ", ");
        }      
        sb.Append("<br>");

        if(properties.Count == 0)
        {
            sb.Append("No Label");
        }
        else
        {
            sb.Append("Material: ");
            foreach (MaterialProperty material in properties)
            {
                sb.Append(material.percentage.ToString());
                sb.Append("% ");
                sb.Append(material.material.ToString().Replace("_", " "));
                sb.Append(" <br>");
            }
        }

        if(!string.IsNullOrEmpty(description))
        {
            sb.Append(description);
            sb.Append("<br>");
        }

        sb.Append("Durability: ");
        if (year > 0)
        {
            sb.Append("Worn for " + year + " years");
        }
        else
        {
            sb.Append("Worn for less than 1 year.");
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
