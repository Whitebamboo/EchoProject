using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CustomerController : MonoBehaviour, IIteractable
{
    [SerializeField] GameObject holderRoot;
    [SerializeField] List<GameObject> models;

    CustomerData m_data;

    //CustomerPopup m_popup;

    Animator animator;

    bool interacble;

    public void SetupData(CustomerData data)
    {
        m_data = data;
        GotoEnterPoint();
    }

    void GotoEnterPoint()
    {
        animator.SetBool("Walking", true);
        transform.DOMove(CustomerManager.instance.EnterPoint, 2f).OnComplete(()=> {
            GotoWaitPoint();
        });
    }

    void GotoWaitPoint()
    {
        transform.DOLookAt(CustomerManager.instance.WaitPoint, 0.5f);
        transform.DOMove(CustomerManager.instance.WaitPoint, 5f).OnComplete(()=> {
            transform.DORotate(Vector3.zero, 0.5f);
            DialogueCanvas canvas = UIManager.instance.CreateScreen<DialogueCanvas>();
            animator.SetBool("Walking", false);
            MusicManager.instance.Play_exclamation();
            interacble = true;
        });
    }

    void GotExitPoint()
    {
        animator.SetBool("Walking", true);
        transform.DOLookAt(CustomerManager.instance.ExitPoint, 0.5f);
        transform.DOMove(CustomerManager.instance.ExitPoint, 2f).OnComplete(()=> {
            EventBus.Broadcast(EventTypes.CustomerLeft);
            Destroy(this.gameObject);
        });

        DialogueCanvas canvas = UIManager.instance.FindScreen<DialogueCanvas>();
        canvas.CloseScreen();
    }

    bool ItemMatch(ItemData item, out FulfillItem matchedItem)
    {
        foreach(FulfillItem fulfillItem in m_data.items)
        {
            if(fulfillItem.data == item)
            {
                MusicManager.instance.Play_congrats();
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
        if(!interacble)
        {
            return;
        }

        Debug.Log("Interact");
        if (item != null)
        {
            if (ItemMatch(item.data, out FulfillItem matchedItem))
            {          
                player.SetItem(null);

                DialogueCanvas canvas = UIManager.instance.CreateScreen<DialogueCanvas>();
                canvas.SetUp(matchedItem.matchResponse, GotExitPoint);
                AssignItem(item);
                interacble = false;
            }
            else
            {
                MusicManager.instance.Play_Incorrect();
            }
        }else
        {
            DialogueCanvas canvas = UIManager.instance.CreateScreen<DialogueCanvas>();
            canvas.SetUp(m_data.request);
            //m_popup.SetText(m_data.request);
        }
    }

    void AssignItem(Item item)
    {
        item.transform.parent = holderRoot.transform;
        item.transform.localPosition = Vector3.zero;
    }

    public void GenerateModel(int index)
    {
        GameObject modelView = Instantiate(models[index], transform);
        modelView.transform.localPosition = new Vector3(0, -0.936f, 0);
        animator = modelView.GetComponent<Animator>();
    }

    public void OnShowHint(GameObject hintObj)
    {

    }

    public void OnHideHint(GameObject hintObj)
    {

    }
}
