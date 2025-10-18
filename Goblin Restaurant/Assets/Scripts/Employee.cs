using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Employee : MonoBehaviour
{
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

    void FindTask()
    {
        targetOrder = RestaurantManager.instance.OrderQueue.FirstOrDefault(o => o.status == OrderStatus.Pending);

        if (targetOrder != null)
        {
            Debug.Log("�丮�� �ֹ� �߰�");
            targetCountertop = RestaurantManager.instance.counterTops.FirstOrDefault(s => !s.isBeingUsed);

            if (targetCountertop != null)
            {
                targetOrder.status = OrderStatus.Cooking;
                targetCountertop.isBeingUsed = true;
                currentState = EmployeeState.MovingToCounterTop;
            }


            return;
        }

        targetTable = RestaurantManager.instance.tables.FirstOrDefault(t => t != null && !t.isOccupied && t.isDirty && !t.isBeingUsedForCleaning);
        if (targetTable != null)
        {
            targetTable.isBeingUsedForCleaning = true;
            currentState = EmployeeState.MovingToTable;
            return; 
        }


        if (idlePosition != null && Vector2.Distance(transform.position, idlePosition.position) > 0.1f)
        {
            currentState = EmployeeState.MovingToIdle;
        }
    }

    // �丮 �ڷ�ƾ
    IEnumerator CookFoodCoroutine()
    {
        currentState = EmployeeState.Cooking;
        Debug.Log($"{targetOrder.recipe.data.recipeName} �丮 ����");

        if (targetOrder.foodObject != null)
        {
            targetOrder.foodObject.transform.position = targetCountertop.transform.position;
            targetOrder.foodObject.SetActive(true);
        }

        yield return new WaitForSeconds(targetOrder.recipe.data.baseCookTime);

        Debug.Log("�丮 �ϼ�");

        targetOrder.status = OrderStatus.ReadyToServe;
        targetCustomer = targetOrder.customer;
        currentState = EmployeeState.MovingToServe;

        if (targetOrder.foodObject != null)
        {
            targetOrder.foodObject.transform.SetParent(this.transform);
            targetOrder.foodObject.transform.localPosition = new Vector3(0, 1.2f, 0);
        }
    }

    // ���� ����
    void ServeFood()
    {
        Debug.Log("���� �Ϸ�");
        targetCustomer.ReceiveFood();

        if (targetOrder.foodObject != null)
        {
            Destroy(targetOrder.foodObject);
        }

        RestaurantManager.instance.OrderQueue.Remove(targetOrder);

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
            targetTable.isBeingUsedForCleaning = false;
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
