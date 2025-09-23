using UnityEngine;

public class Table : MonoBehaviour
{
    // �� ���̺��� ���� �մԿ� ���� ���ǰ� �ִ��� ����
    public bool isOccupied = false;

    // ���� �� ���̺� �ɾ��ִ� �մ� ������Ʈ�� ������ ����
    public GameObject currentCustomer;

    public void Occupy(GameObject customer)
    {
        isOccupied = true;
        currentCustomer = customer;
    }

    public void Vacate()
    {
        isOccupied = false;
        currentCustomer = null;
    }
}
