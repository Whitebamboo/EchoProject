using NDream.AirConsole;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : CSingletonMono<CustomerManager>
{
    [SerializeField] Vector3 spawnPoint;
    [SerializeField] Vector3 enterPoint;
    [SerializeField] Vector3 waitPoint;
    [SerializeField] Vector3 exitPoint;
    [SerializeField] CustomerController customerPrefab;
    [SerializeField] List<FantasyLevel> fantasyLevels;

    public Vector3 EnterPoint => enterPoint;
    public Vector3 WaitPoint => waitPoint;
    public Vector3 ExitPoint => exitPoint;

    List<ItemHolder> holders = new List<ItemHolder>();

    Queue<CustomerData> customerQuene = new Queue<CustomerData>();

    int currLevel;
    int currCustomerIndex;

    public void AddHolders(ItemHolder holder)
    {
        holders.Add(holder);
    }

    protected override void Awake()
    {
        base.Awake();

        EventBus.AddListener(EventTypes.CustomerLeft, OnCustomerLeft);
    }

    public void StartLevel()
    {
        currLevel = 0;
        SetupLevel(currLevel);
    }

    public void SetupLevel(int level)
    {
        if(level >= fantasyLevels.Count)
        {
            Debug.Log("Game finished");
            UIManager.instance.CreateScreen<EndCanvas>();
            AirConsole.instance.Broadcast("Fantasy;Intro");
            return;
        }

        customerQuene = new Queue<CustomerData>(fantasyLevels[level].customers);
        RefreshItems(fantasyLevels[level].cloths);
        if(customerQuene.Count == 0)
        {
            Debug.LogError("Zero customer");
            return;
        }

        CustomerData customer = customerQuene.Dequeue();
        GenerateCustomer(customer);
    }

    void OnCustomerLeft()
    {
        if(customerQuene.Count == 0)
        {
            currLevel += 1;
            SetupLevel(currLevel);
            return;
        }

        currCustomerIndex += 1;
        CustomerData customer = customerQuene.Dequeue();
        GenerateCustomer(customer);
    }

    public void GenerateCustomer(CustomerData data)
    {
        CustomerController newCustomer = Instantiate(customerPrefab);
        newCustomer.transform.position = spawnPoint;
        newCustomer.GenerateModel(currCustomerIndex);
        newCustomer.SetupData(data);
    }

    public void RefreshItems(List<ItemData> items)
    {
        if(items.Count > holders.Count)
        {
            Debug.LogError("There are more items for the holders in the scene. Please check the config");
        }

        for(int i = 0; i < holders.Count; i++)
        {
            if(i >= items.Count)
            {
                break;
            }

            holders[i].AddItemData(items[i]);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(spawnPoint, 1);
        Gizmos.DrawWireSphere(enterPoint, 1);
        Gizmos.DrawWireSphere(waitPoint, 1);
        Gizmos.DrawWireSphere(exitPoint, 1);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        // unregister events
        EventBus.RemoveListener(EventTypes.CustomerLeft, OnCustomerLeft);
    }
}
