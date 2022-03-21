using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour, IIteractable
{
    Item m_item;
    public Item GetItem() => m_item;

    public GameObject holderRoot;

    public ItemData data;

    public bool IsHoldingTtem => m_item != null;

    void Start()
    {
        if(data != null)
        {
            GameObject newItem = new GameObject();
            Cloth c = newItem.AddComponent<Cloth>();
            c.SetupData(data);
            AssignItem(c);
        }
    }

    public void OnInteract(PlayerController player, Item item)
    {
        Debug.Log("Interact");     
        if(item != null)
        {
            AssignItem(item);
            player.SetItem(null);
        }
        else
        {
            player.SetItem(m_item);
            m_item = null;
        }
    }

    void AssignItem(Item item)
    {
        m_item = item;
        item.transform.parent = holderRoot.transform;
        item.transform.localPosition = Vector3.zero;
    }
}
