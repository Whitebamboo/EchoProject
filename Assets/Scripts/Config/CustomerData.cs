using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Customer", menuName = "Config/Customer data")]
public class CustomerData : ScriptableObject
{
    public Sprite image;
    public string request;
    public List<FulfillItem> items;
    public List<FulfillItem> closeItems;
    public string notMatchResponse;
}

[System.Serializable]
public class FulfillItem
{
    public ItemData data;
    public float score;
    public string matchResponse;
}
