using UnityEngine;
using System.Collections;
using System.Linq;
using System.Drawing;

public class Customer : MonoBehaviour
{
    public enum CustomerState { MovingToTable, DecidingMenu, WaitingForFood, Eating, Leaving }
    public CustomerState currentState;
    public GameObject orderIconPrefab; 
    public Transform iconSpawnPoint;   // �������� ǥ�õ� �Ӹ� �� ��ġ
    private GameObject currentOrderIcon; // ���� �� �ִ� �������� ������ ����

    private Transform targetTable;
    [SerializeField]
    private float speed = 3f;
    private PlayerRecipe myOrderedRecipe;
    private Transform exitPoint; // ���� �� �̵��� ��ǥ ����

    public void Initialize(Transform table, Transform exit)
    {
        targetTable = table;
        exitPoint = exit;
        currentState = CustomerState.MovingToTable;
    }

    void Update()
    {
        switch (currentState)
        {
            case CustomerState.MovingToTable:
                // ���̺�� �̵�
                transform.position = Vector2.MoveTowards(transform.position, targetTable.position, speed * Time.deltaTime);
                if (Vector2.Distance(transform.position, targetTable.position) < 0.1f)
                {
                    currentState = CustomerState.DecidingMenu;
                    targetTable.GetComponent<Table>().Occupy(gameObject); // ���̺� ����

                    StartCoroutine(DecideMenuCoroutine()); // �޴� ���� �ڷ�ƾ ����
                }
                break;
            case CustomerState.Leaving:
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(0, -5), speed * Time.deltaTime);
                Destroy(gameObject, 2f);
                break;
        }
    }

    IEnumerator DecideMenuCoroutine()
    {
        Debug.Log("�մ��� �޴��� ���� ��...");
        yield return new WaitForSeconds(2f);

        var availableMenu = MenuPlanner.instance.dailyMenu.Where(r => r != null).ToList();

        if (availableMenu.Count > 0)
        {
            int randomIndex = Random.Range(0, availableMenu.Count);
            myOrderedRecipe = availableMenu[randomIndex];

            Debug.Log($"{myOrderedRecipe.data.recipeName} ����! �ֹ濡 �ֹ��� �ֽ��ϴ�.");

            if (orderIconPrefab != null && iconSpawnPoint != null)
            {
                currentOrderIcon = Instantiate(orderIconPrefab, iconSpawnPoint.position, Quaternion.identity);
                currentOrderIcon.transform.SetParent(iconSpawnPoint);
                OrderIconUI iconUI = currentOrderIcon.GetComponent<OrderIconUI>();
                if (iconUI != null)
                {
                    iconUI.SetIcon(myOrderedRecipe.data.icon);
                }
            }

            KitchenOrder newOrder = new KitchenOrder(this, myOrderedRecipe, null); // foodObject�� ���߿� �߰�
            RestaurantManager.instance.OrderQueue.Add(newOrder);

            currentState = CustomerState.WaitingForFood;
        }
        else
        {
            Debug.LogError("�մ��� �ֹ��� �޴��� ������ �޴��� �ϳ��� ���Ǿ� ���� �ʽ��ϴ�!");
        }
    }

    public void ReceiveFood()
    {
        if (currentOrderIcon != null)
        {
            Destroy(currentOrderIcon);
        }

        currentState = CustomerState.Eating;
        StartCoroutine(EatAndLeave());
    }

    public void SetTable(Transform table)
    {
        targetTable = table;
        currentState = CustomerState.MovingToTable;
    }


    System.Collections.IEnumerator EatAndLeave()
    {
        Debug.Log("�Ļ� ����");
        yield return new WaitForSeconds(2f); // 2�ʰ� �Ļ�
        Debug.Log("�Ļ� �Ϸ�");
        if (myOrderedRecipe != null)
        {
            int price = myOrderedRecipe.GetCurrentPrice();
            GameManager.instance.AddGold(price);
            Debug.Log($"�մ��� {myOrderedRecipe.data.recipeName}��(��) �԰� {price}���� �����߽��ϴ�.");
        }
        else
        {
            Debug.LogError("�ֹ��� ������ ������ �����ϴ�!");
        }
        GameManager.instance.AddCustomerCount();
        targetTable.GetComponent<Table>().Vacate(); // ���̺� ����
        targetTable.GetComponent<Table>().isDirty = true; // ���̺� ������ ���·� ����
        currentState = CustomerState.Leaving;
        RestaurantManager.instance.customers.Remove(this);
    }
}
