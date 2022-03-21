using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    public ItemData data;

    GameObject prefab; //versual

    public void SetupData(ItemData data)
    {
        this.data = data;

        prefab = Instantiate(data.prefab, transform.position, transform.rotation, transform);
    }
}
