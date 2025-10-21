using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RestaurantManager : MonoBehaviour
{
    public static RestaurantManager instance;

    // [���� ������ ���� �߰� �ʵ�]
    [Header("Employee Spawning")]
    [Tooltip("�ʿ� ������ų ���� ĳ���� �⺻ ������ (Employee.cs�� �پ��־�� ��)")]
    public GameObject employeePrefab; // �⺻ ������
    [Tooltip("�������� ó�� ��Ÿ�� ��ġ")]
    public Transform spawnPoint;

    // ���� �ʵ�
    public List<Customer> customers;
    public List<Table> tables;
    public List<CounterTop> counterTops;
    [SerializeField]
    private List<KitchenOrder> orderQueue;

    public List<KitchenOrder> OrderQueue => orderQueue;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);

        customers = new List<Customer>();
        orderQueue = new List<KitchenOrder>();
    }

    /// <summary>
    /// GameManager�� Start()���� ȣ��Ǿ�, �⺻ �������� ����Ͽ� ���� �������� �ʿ� �����մϴ�.
    /// (���� ���� ȣȯ�� ������ ���� ���ܵξ�����, �� �Լ��� ȣ��� ���Դϴ�.)
    /// </summary>
    public void SpawnHiredEmployees(List<EmployeeInstance> hiredEmployees)
    {
        // �� �Լ��� GameManager���� SpawnWorkersWithPrefabs�� ��ü�մϴ�.

        if (employeePrefab == null || spawnPoint == null)
        {
            Debug.LogError("RestaurantManager ERROR: Employee Prefab or Spawn Point is not set in Inspector!");
            return;
        }

        // ���� ���������� �����ϴ� ���� (�⺻ ����)
        List<(EmployeeInstance, GameObject)> workersToSpawn = hiredEmployees
            .Select(data => (data, employeePrefab))
            .ToList();

        SpawnWorkersWithPrefabs(workersToSpawn);
    }

    /// <summary>
    /// GameManager���� ���޵�, �����Ϳ� ������ ������ ���� ����Ͽ� �������� �����մϴ�.
    /// </summary>
    public void SpawnWorkersWithPrefabs(List<(EmployeeInstance data, GameObject prefab)> workersToSpawn)
    {
        if (spawnPoint == null)
        {
            Debug.LogError("RestaurantManager ERROR: Spawn Point is not set in Inspector! Cannot spawn workers.");
            return;
        }

        foreach (var (employeeData, employeePrefab) in workersToSpawn)
        {
            if (employeePrefab == null)
            {
                Debug.LogWarning($"Skipping spawn for {employeeData.firstName}: Prefab is missing for this employee!");
                continue;
            }

            // Worker ������Ʈ ����
            // transform�� Parent�� �����Ͽ� Hierarchy���� RestaurantManager�� �ڽ����� ���̰� �մϴ�.
            GameObject workerObject = Instantiate(employeePrefab, spawnPoint.position, Quaternion.identity, this.transform);

            // Employee.cs ��ũ��Ʈ ��������
            Employee workerComponent = workerObject.GetComponent<Employee>();

            if (workerComponent != null)
            {
                // ������ ���� ������Ʈ�� EmployeeInstance �����͸� �Ҵ��ϰ� �ʱ�ȭ�մϴ�.
                workerComponent.Initialize(employeeData, spawnPoint);
                workerObject.name = $"Worker - {employeeData.firstName} ({employeeData.BaseData.speciesName})";
            }
            else
            {
                Debug.LogError($"Employee Prefab�� Employee.cs ��ũ��Ʈ�� �����ϴ�: {workerObject.name}");
                Destroy(workerObject);
            }
        }
    }
}
