using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "level", menuName = "Config/Fantasy Level")]
public class FantasyLevel : ScriptableObject
{
    public List<CustomerData> customers;
    public List<ItemData> cloths;
}
