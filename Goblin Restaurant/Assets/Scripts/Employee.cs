using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using System;

// �� ��ũ��Ʈ�� �ʿ� �����Ǵ� ���� ĳ���Ϳ� �پ� �ϲ� ������ �մϴ�.
public class Employee : MonoBehaviour
{
    // [�߰��� �ʵ�]
    [Tooltip("�� ������ ����� ������ �ν��Ͻ�")]
    private EmployeeInstance employeeData;

    public enum EmployeeState
    {
        Idle,
        MovingToCounterTop,
        Cooking,
        MovingToServe,
        MovingToIdle,
        CheckingTable,
        MovingToTable,
        Cleaning
    }

    public EmployeeState currentState;

    // [����: private���� public���� ����]
    [SerializeField]
    private float movespeed = 3f;
    public float cookingtime = 5f;

    /*[SerializeField]
    private int cookingskill = 1; // ���� ��� �߰� ����*/
    [SerializeField]
    private Customer targetCustomer;
    [SerializeField]
    private CounterTop targetCountertop;
    [SerializeField]
    private Table targetTable;
    [SerializeField]
    private Transform idlePosition; // ������ �� ���� ���� �� �� ���� ��ġ
    private KitchenOrder targetOrder;

    // [�߰��� Initialize �Լ�] - RestaurantManager���� ȣ���
    public void Initialize(EmployeeInstance data, Transform defaultIdlePosition)
    {
        this.employeeData = data;
        this.idlePosition = defaultIdlePosition;

        // EmployeeInstance �����Ϳ��� �ɷ�ġ�� ������ ����
        // �丮 �ð��� �ɷ�ġ�� �ݺ���Ͽ� ���� (�ּ� 1��)
        cookingtime = Mathf.Max(1f, 5f - (data.currentCookingStat * 0.1f));

        // �����
        Debug.Log($"{data.firstName} ���� �Ϸ�. Cooking Time: {cookingtime}s");

        currentState = EmployeeState.Idle;
    }


    void Start()
    {
        // ���� Initialize�� ȣ����� �ʾҴٸ� (������ �ʿ� �ִ� �����̶��)
        if (employeeData == null)
        {
            // �� ������ ���� �ʿ� �ִ� ���ΰ� ������ �� �����Ƿ�, �⺻ ���·� ����
            currentState = EmployeeState.Idle;
        }
    }

    void Update()
    {
        // ���¿� ���� �ٸ� �ൿ�� ����
        switch (currentState)
        {
            case EmployeeState.Idle:
                FindTask();
                break;
            case EmployeeState.MovingToIdle:
                if (idlePosition != null)
                {
                    MoveTo(idlePosition.position, () => { currentState = EmployeeState.Idle; });
                }
                else
                {
                    currentState = EmployeeState.Idle;
                }
                break;
            case EmployeeState.MovingToCounterTop:
                // MoveTo �Լ��� ���� �� CookFoodCoroutine ����
                MoveTo(targetCountertop.transform.position, () => { StartCoroutine(CookFoodCoroutine()); });
                break;
            case EmployeeState.Cooking:
                break;
            case EmployeeState.MovingToServe:
                MoveTo(targetCustomer.transform.position, ServeFood);
                break;
            case EmployeeState.CheckingTable:
                CheckTable();
                break;
            case EmployeeState.MovingToTable:
                // MoveTo �Լ��� ���� �� CleaningTable ����
                MoveTo(targetTable.transform.position, () => { StartCoroutine(CleaningTable()); });
                break;
            case EmployeeState.Cleaning:
                break;
        }
    }

    void FindTask()
    {
        // 1. �丮 �ֹ� ã��
        if (RestaurantManager.instance != null)
        {
            targetOrder = RestaurantManager.instance.OrderQueue.FirstOrDefault(o => o != null && o.status == OrderStatus.Pending);
        }

        if (targetOrder != null)
        {
            Debug.Log($"{employeeData?.firstName ?? "Worker"} �丮�� �ֹ� �߰�");
            targetCountertop = RestaurantManager.instance.counterTops.FirstOrDefault(s => s != null && !s.isBeingUsed);

            if (targetCountertop != null)
            {
                // **���� ���� ���� ���� �ʿ�:** �۾� �Ҵ� �� ��� ���� ���� �� ����Ʈ ������Ʈ
                targetOrder.status = OrderStatus.Cooking;
                targetCountertop.isBeingUsed = true;
                currentState = EmployeeState.MovingToCounterTop;
            }
            // �۾� �Ҵ翡 ���������Ƿ� ����
            return;
        }

        // 2. û���� ���̺� ã�� (������, ��� ������ �ʰ�, û�� ������ ���� ���̺�)
        if (RestaurantManager.instance != null)
        {
            targetTable = RestaurantManager.instance.tables.FirstOrDefault(t =>
                t != null && t.isDirty && !t.isBeingUsedForCleaning); // isOccupied�� û�ҿ� ����
        }

        if (targetTable != null)
        {
            targetTable.isBeingUsedForCleaning = true;
            currentState = EmployeeState.MovingToTable;
            // �۾� �Ҵ翡 ���������Ƿ� ����
            return;
        }


        // 3. �� �� ������ ��� ��ġ�� �̵�
        if (idlePosition != null && Vector2.Distance(transform.position, idlePosition.position) > 0.1f)
        {
            currentState = EmployeeState.MovingToIdle;
        }
    }

    // �丮 �ڷ�ƾ
    IEnumerator CookFoodCoroutine()
    {
        currentState = EmployeeState.Cooking;
        Debug.Log($"{employeeData?.firstName ?? "Worker"} {targetOrder.recipe.data.recipeName} �丮 ����");

        if (targetOrder.foodObject != null)
        {
            targetOrder.foodObject.transform.position = targetCountertop.transform.position;
            targetOrder.foodObject.SetActive(true);
        }

        // ���� �ɷ�ġ(cookingtime) �ݿ�
        yield return new WaitForSeconds(cookingtime);

        Debug.Log($"{employeeData?.firstName ?? "Worker"} �丮 �ϼ�");

        targetOrder.status = OrderStatus.ReadyToServe;
        targetCustomer = targetOrder.customer;
        currentState = EmployeeState.MovingToServe;

        if (targetOrder.foodObject != null)
        {
            targetOrder.foodObject.transform.SetParent(this.transform);
            targetOrder.foodObject.transform.localPosition = new Vector3(0, 1.2f, 0);
        }

        // ���� �������� �̵�
    }

    // ���� ����
    void ServeFood()
    {
        Debug.Log($"{employeeData?.firstName ?? "Worker"} ���� �Ϸ�");
        if (targetCustomer != null)
        {
            targetCustomer.ReceiveFood();
        }

        if (targetOrder.foodObject != null)
        {
            Destroy(targetOrder.foodObject);
        }

        if (RestaurantManager.instance != null && targetOrder != null)
        {
            RestaurantManager.instance.OrderQueue.Remove(targetOrder);
        }

        // ����ߴ� �ڿ����� �ʱ�ȭ
        if (targetCountertop != null) targetCountertop.isBeingUsed = false;
        targetCustomer = null;
        targetCountertop = null;
        targetOrder = null;

        currentState = EmployeeState.MovingToIdle;
    }

    void CheckTable()
    {
        // FindTask �������� û���� ���̺��� ã�Ҵٰ� ����
        if (targetTable != null && targetTable.isDirty)
        {
            currentState = EmployeeState.MovingToTable;
        }
        else
        {
            currentState = EmployeeState.Idle;
        }
    }

    // ���̺� û��
    IEnumerator CleaningTable()
    {
        currentState = EmployeeState.Cleaning;
        Debug.Log($"{employeeData?.firstName ?? "Worker"} ���̺� û�� ����");

        // ���� �ɷ�ġ(û�� �ɷ�) �ݿ��� �ʿ�
        yield return new WaitForSeconds(1f); // û�� �ð� ���� (���߿� �ɷ�ġ�� ����)

        Debug.Log($"{employeeData?.firstName ?? "Worker"} ���̺� û�� �Ϸ�");
        if (targetTable != null)
        {
            targetTable.isDirty = false;
            targetTable.isBeingUsedForCleaning = false;
        }
        targetTable = null;
        currentState = EmployeeState.MovingToIdle;
    }

    // ��ǥ �������� �̵��ϰ�, �����ϸ� ������ �ൿ(Action)�� �����ϴ� �Լ�
    void MoveTo(Vector3 destination, Action onArrived)
    {
        transform.position = Vector2.MoveTowards(transform.position, destination, movespeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, destination) < 0.1f)
        {
            onArrived?.Invoke();
        }
    }
}