using UnityEngine;

public class Customer : MonoBehaviour
{
    public enum CustomerState { MovingToTable, WaitingForOrder, Eating, Leaving }
    public CustomerState currentState;

    private Transform targetTable;
    [SerializeField]
    private float speed = 3f;

    void Update()
    {
        switch (currentState)
        {
            case CustomerState.MovingToTable:
                // ���̺�� �̵�
                transform.position = Vector2.MoveTowards(transform.position, targetTable.position, speed * Time.deltaTime);
                if (Vector2.Distance(transform.position, targetTable.position) < 0.1f)
                {
                    currentState = CustomerState.WaitingForOrder;
                    targetTable.GetComponent<Table>().Occupy(gameObject); // ���̺� ����
                    Debug.Log("�մ� ����, �ֹ� ��� ��");
                }
                break;
            case CustomerState.Leaving:
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(0, -5), speed * Time.deltaTime);
                Destroy(gameObject, 2f);
                break;
        }
    }

    public void SetTable(Transform table)
    {
        targetTable = table;
        currentState = CustomerState.MovingToTable;
    }

    // ������ ������ ȣ��� �Լ�
    public void ReceiveFood()
    {
        currentState = CustomerState.Eating;
        StartCoroutine(EatAndLeave());
    }

    System.Collections.IEnumerator EatAndLeave()
    {
        Debug.Log("�Ļ� ����");
        yield return new WaitForSeconds(2f); // 2�ʰ� �Ļ�
        Debug.Log("�Ļ� �Ϸ�");
        GameManager.instance.AddGold(100);
        GameManager.instance.AddCustomerCount();
        targetTable.GetComponent<Table>().Vacate(); // ���̺� ����
        targetTable.GetComponent<Table>().isDirty = true; // ���̺� ������ ���·� ����
        currentState = CustomerState.Leaving;
        RestaurantManager.instance.customers.Remove(this);
    }
}
