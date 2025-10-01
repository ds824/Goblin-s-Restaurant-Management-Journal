using UnityEngine;

public class PlaceObjectButton : MonoBehaviour
{
    public int tablePrice = 100; // ���̺� ����\
    public GameObject UpgradeTableButton; // ���̺� ���׷��̵� ��ư
    public GameObject UpgradeTablePannal; // ���̺� ���׷��̵� �г�

    public void OnButtonClick()
    {
        if(GameManager.instance.totalGoldAmount >= tablePrice)
        {
            GameManager.instance.AddTable(this.transform, tablePrice);

            gameObject.SetActive(false); // ��ư ��Ȱ��ȭ
            UpgradeTableButton.SetActive(false); // ���̺� ���׷��̵� ��ư ��Ȱ��ȭ
            UpgradeTablePannal.SetActive(false); // ���̺� ���׷��̵� �г� ��Ȱ��ȭ


        }
        else
        {
            Debug.Log("��尡 �����մϴ�!");
        }
    }
}
