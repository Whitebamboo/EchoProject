using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHolder : MonoBehaviour, IIteractable
{
    Item m_item;
    public Item GetItem() => m_item;

    public GameObject holderRoot;

    public bool IsHoldingTtem => m_item != null;

    private GameObject popupHint;

    void Start()
    {
        CustomerManager.instance.AddHolders(this);
    }

    public void OnInteract(PlayerController player, Item item)
    {
        Debug.Log("Interact");
        
        if(m_item != null && item != null)
        {
            return;
        }

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

    public void OnShowHint(GameObject hintObj)
    {
        if (m_item == null || popupHint != null)
        {
            return;
        }

        popupHint = Instantiate(hintObj, transform);
        popupHint.transform.position = transform.position + new Vector3(0, -0.7f, 0);
    }

    public void OnHideHint(GameObject hintObj)
    {
        if (hintObj != null)
        {
            Destroy(popupHint);
        }
    }

    public void AddItemData(ItemData data)
    {
        if(holderRoot.transform.childCount == 1)
        {
            Destroy(holderRoot.transform.GetChild(0).gameObject);
        }

        if (data != null)
        {
            GameObject newItem = new GameObject();
            Cloth c = newItem.AddComponent<Cloth>();
            c.SetupData(data);
            AssignItem(c);
        }
    }
    
    void AssignItem(Item item)
    {
        m_item = item;
        item.transform.parent = holderRoot.transform;
        item.transform.localPosition = Vector3.zero;
        item.transform.localEulerAngles = Vector3.zero;
        item.transform.localScale = Vector3.one;
    }

}
