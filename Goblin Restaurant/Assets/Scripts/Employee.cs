using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Employee : MonoBehaviour
{
    public enum EmployeeState
    {
        Idle,
        MovingToCustomer,
        TakingOrder,
        MovingToCounterTop,
        Cooking,
        MovingToServe,
        MovingToIdle,
        CheckingTable,
        MovingToTable,
        Cleaning
    }

    public EmployeeState currentState;
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

    void Start()
    {
        currentState = EmployeeState.Idle;
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
            case EmployeeState.MovingToCustomer:
                MoveTo(targetCustomer.transform.position, () => { currentState = EmployeeState.TakingOrder; });
                break;
            case EmployeeState.TakingOrder:
                TakeOrder();
                break;
            case EmployeeState.MovingToCounterTop:
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
                MoveTo(targetTable.transform.position, () => { StartCoroutine(CleaningTable()); });
                break;
            case EmployeeState.Cleaning:
                break;
        }
    }

    // �� �� ã��
    void FindTask()
    {
        targetCustomer = RestaurantManager.instance.customers.FirstOrDefault(c => c.currentState == Customer.CustomerState.WaitingForOrder);
        if (targetCustomer != null)
        {
            currentState = EmployeeState.MovingToCustomer;
            return;
        }

        targetTable = RestaurantManager.instance.tables.FirstOrDefault(t => !t.isOccupied && t.isDirty);
        if (targetTable != null)
        {
            currentState = EmployeeState.MovingToTable;
            return;
        }

        if (Vector2.Distance(transform.position, idlePosition.position) > 0.1f)
        {
            currentState = EmployeeState.MovingToIdle;
        }
    }

    // �ֹ� ����
    void TakeOrder()
    {
        Debug.Log("�ֹ� ����");

        // �̻�� ȭ�� ã��
        targetCountertop = RestaurantManager.instance.counterTops.FirstOrDefault(s => !s.isBeingUsed);

        if (targetCountertop != null)
        {
            targetCountertop.isBeingUsed = true; // ȭ���� ��� ���·� ����
            currentState = EmployeeState.MovingToCounterTop;
        }

        else
        {
            // ���� ��� ȭ���� ��� ���̶�� ��� ��� (Idle ���·� ���ư� �ٽ� Ž��)
            currentState = EmployeeState.Idle;
        }
    }

    // �丮 �ڷ�ƾ
    IEnumerator CookFoodCoroutine()
    {
        currentState = EmployeeState.Cooking;
        Debug.Log("�丮 ����");

        yield return new WaitForSeconds(cookingtime);

        Debug.Log("�丮 �ϼ�");
        currentState = EmployeeState.MovingToServe;
    }

    // ���� ����
    void ServeFood()
    {
        Debug.Log("���� �Ϸ�");

        targetCustomer.ReceiveFood(); // �մԿ��� ������ ����

        // ����ߴ� �ڿ����� �ʱ�ȭ
        targetCountertop.isBeingUsed = false;
        targetCustomer = null;
        targetCountertop = null;

        currentState = EmployeeState.MovingToIdle;
    }

    void CheckTable()
    {
        targetTable = RestaurantManager.instance.tables.FirstOrDefault(t => !t.isOccupied && t.isDirty);

        if (targetTable.isDirty && targetTable.isOccupied)
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
        Debug.Log("���̺� û�� ����");

        yield return new WaitForSeconds(1f); // û�� �ð�

        Debug.Log("���̺� û�� �Ϸ�");
        if (targetTable != null)
        {
            targetTable.isDirty = false;
        }
        targetTable = null;
        currentState = EmployeeState.MovingToIdle;
    }

    // ��ǥ �������� �̵��ϰ�, �����ϸ� ������ �ൿ(Action)�� �����ϴ� �Լ�
    void MoveTo(Vector3 destination, System.Action onArrived)
    {
        transform.position = Vector2.MoveTowards(transform.position, destination, movespeed * Time.deltaTime);

        if (Vector2.Distance(transform.position, destination) < 0.1f)
        {
            onArrived?.Invoke();
        }
    }
}
