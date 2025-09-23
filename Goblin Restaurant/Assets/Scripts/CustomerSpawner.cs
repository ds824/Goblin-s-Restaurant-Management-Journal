using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class CustomerSpawner : MonoBehaviour
{
    public GameObject customerPrefab; // ������ �� ������
    public List<Table> tables; // ���� �� ���̺� ����Ʈ
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

            newCustomer.SetTable(emptyTable.transform);
            emptyTable.isOccupied = true;
        }

        else
        {
            Debug.Log("��� ���̺��� �� á��");
        }
    }

    Table FindEmptyTable()
    {
        foreach (Table table in tables)
        {
            if (!table.isOccupied)
            {
                return table;
            }
        }
        return null; // ��� ���̺��� �� á��
    }
}
