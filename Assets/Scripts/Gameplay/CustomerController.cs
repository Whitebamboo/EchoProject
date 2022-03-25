using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CustomerController : MonoBehaviour, IIteractable
{
    public GameObject holderRoot;

    CustomerData m_data;

    public void SetupData(CustomerData data)
    {
        m_data = data;
        GotoEnterPoint();
    }

    void GotoEnterPoint()
    {
        transform.DOMove(CustomerManager.instance.EnterPoint, 2f).OnComplete(()=> {
            DialogueCanvas canvas = UIManager.instance.CreateScreen<DialogueCanvas>();
            canvas.SetUp(m_data.request);
            GotoWaitPoint();
        });
    }

    void GotoWaitPoint()
    {
        transform.DOLookAt(CustomerManager.instance.WaitPoint, 0.5f);
        transform.DOMove(CustomerManager.instance.WaitPoint, 5f).OnComplete(()=> {
            transform.DORotate(Vector3.zero, 0.5f);
        });
    }

    void GotExitPoint()
    {
        transform.DOLookAt(CustomerManager.instance.ExitPoint, 0.5f);
        transform.DOMove(CustomerManager.instance.ExitPoint, 2f).OnComplete(()=> {
            EventBus.Broadcast(EventTypes.CustomerLeft);
            Destroy(this.gameObject);
        });
    }

    bool ItemMatch(ItemData item, out FulfillItem matchedItem)
    {
        foreach(FulfillItem fulfillItem in m_data.items)
        {
            if(fulfillItem.data == item)
            {
                matchedItem = fulfillItem;
                return true;
            }
        }

        matchedItem = null;
        return false;
    }

    public void OnInteract(PlayerController player, Item item = null)
    {
        Debug.Log("Interact");
        if (item != null)
        {
            if (ItemMatch(item.data, out FulfillItem matchedItem))
            {
                GotExitPoint();
                player.SetItem(null);

                DialogueCanvas canvas = UIManager.instance.CreateScreen<DialogueCanvas>();
                canvas.SetUp(matchedItem.response);
                AssignItem(item);
            }
            else
            {
                DialogueCanvas canvas = UIManager.instance.CreateScreen<DialogueCanvas>();
                canvas.SetUp("This is not I want!");
            }
        }
    }

    void AssignItem(Item item)
    {
        item.transform.parent = holderRoot.transform;
        item.transform.localPosition = Vector3.zero;
    }
}
