using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CustomerController : MonoBehaviour, IIteractable
{
    public GameObject holderRoot;

    CustomerData m_data;

    CustomerPopup m_popup;

    public void SetupData(CustomerData data)
    {
        m_data = data;
        GotoEnterPoint();
    }

    void GotoEnterPoint()
    {
        transform.DOMove(CustomerManager.instance.EnterPoint, 2f).OnComplete(()=> {
            //DialogueCanvas canvas = UIManager.instance.CreateScreen<DialogueCanvas>();
            //canvas.SetUp(m_data.request);
            GotoWaitPoint();
        });
    }

    void GotoWaitPoint()
    {
        transform.DOLookAt(CustomerManager.instance.WaitPoint, 0.5f);
        transform.DOMove(CustomerManager.instance.WaitPoint, 5f).OnComplete(()=> {
            transform.DORotate(Vector3.zero, 0.5f);
            FollowUICanvas canvas = UIManager.instance.FindScreen<FollowUICanvas>();
            m_popup = canvas.GenerateCustomerPopup(transform);
        });
    }

    void GotExitPoint()
    {
        if (m_popup != null)
        {
            Destroy(m_popup.gameObject);
        }
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

        DialogueCanvas canvas = UIManager.instance.CreateScreen<DialogueCanvas>();
        foreach (FulfillItem fulfillItem in m_data.closeItems)
        {
            if (fulfillItem.data == item)
            {
                matchedItem = fulfillItem;              
                canvas.SetUp(fulfillItem.matchResponse);

                return false;
            }
        }

        canvas = UIManager.instance.CreateScreen<DialogueCanvas>();
        canvas.SetUp(m_data.notMatchResponse);

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
                canvas.SetUp(matchedItem.matchResponse);
                AssignItem(item);
            }
        }else
        {
            DialogueCanvas canvas = UIManager.instance.CreateScreen<DialogueCanvas>();
            canvas.SetUp(m_data.request);
        }
    }

    void AssignItem(Item item)
    {
        item.transform.parent = holderRoot.transform;
        item.transform.localPosition = Vector3.zero;
    }

    public void OnShowHint()
    {

    }

    public void OnHideHint()
    {

    }
}
