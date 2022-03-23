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
    [SerializeField] CustomerData data;

    public Vector3 EnterPoint => enterPoint;

    public Vector3 WaitPoint => waitPoint;

    public Vector3 ExitPoint => exitPoint;

    public void StartGame()
    {
        GenerateCustomer();
    }

    public void GenerateCustomer()
    {
        CustomerController newCustomer = Instantiate(customerPrefab);
        newCustomer.transform.position = spawnPoint;
        newCustomer.SetupData(data);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(spawnPoint, 1);
        Gizmos.DrawWireSphere(enterPoint, 1);
        Gizmos.DrawWireSphere(waitPoint, 1);
        Gizmos.DrawWireSphere(exitPoint, 1);
    }
}
