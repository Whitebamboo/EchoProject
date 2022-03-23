using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Customer", menuName = "Config/Customer data")]
public class CustomerData : ScriptableObject
{
    public string customerName;
    public string request;
    public List<FulfillItem> items;
}

[System.Serializable]
public class FulfillItem
{
    public ItemData data;
    public float score;
    public string response;
}
