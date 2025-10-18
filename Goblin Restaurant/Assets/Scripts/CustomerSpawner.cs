using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject customerPrefab; // ������ �� ������
    public Transform spawnPoint; // ���� ������ ��ġ

    public float spawnInterval = 5f; // ���� �õ� ����
    private float spawnTimer;


    private void Update()
    {
        if (GameManager.instance.currentState != GameManager.GameState.Open)
        {
            return;
        }

        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            TrySpawnCustomer();
        }
    }

    void TrySpawnCustomer()
    {
        Table emptyTable = FindEmptyTable();

        if (emptyTable != null)
        {
            Debug.Log("�� ���̺� �߰� �� ����");

            GameObject newCustomerObj = Instantiate(customerPrefab, spawnPoint.position, Quaternion.identity);
            Customer newCustomer = newCustomerObj.GetComponent<Customer>();

            RestaurantManager.instance.customers.Add(newCustomer);

            newCustomer.Initialize(emptyTable.transform, spawnPoint);
            emptyTable.isOccupied = true;
        }

        else
        {
            Debug.Log("��� ���̺��� �� á��");
        }
    }

    Table FindEmptyTable()
    {
        if (RestaurantManager.instance == null || RestaurantManager.instance.tables == null)
        {
            Debug.LogError("RestaurantManager �Ǵ� ���̺� ����Ʈ�� �����ϴ�!");
            return null;
        }

        foreach (Table table in RestaurantManager.instance.tables)
        {
            if (table != null && !table.isOccupied && !table.isDirty)
            {
                return table;
            }
        }
        return null;
    }
}