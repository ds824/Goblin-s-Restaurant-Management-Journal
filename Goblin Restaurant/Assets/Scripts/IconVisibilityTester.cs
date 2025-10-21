// ���� �̸�: IconVisibilityTester.cs
using UnityEngine;

public class IconVisibilityTester : MonoBehaviour
{
    // �׽�Ʈ�� ������ ������
    public GameObject orderIconPrefab;

    // �������� ������ ���� ĵ����
    public Canvas worldCanvas;

    void Awake()
    {
        Debug.Log("'T' Ű �Է�! ������ ���� ���� �׽�Ʈ�� �����մϴ�.");

        if (orderIconPrefab == null || worldCanvas == null)
        {
            Debug.LogError("�׽�Ʈ ��ũ��Ʈ�� ������ �Ǵ� ĵ������ ������� �ʾҽ��ϴ�!");
            return;
        }

        // ĵ������ ���߾� ��ġ�� �������� ������ ����
        GameObject iconInstance = Instantiate(orderIconPrefab, worldCanvas.transform.position, Quaternion.identity);

        // ĵ������ �ڽ����� ����� �������ǵ��� ��
        iconInstance.transform.SetParent(worldCanvas.transform, false);

        Debug.Log($"������ ���� �Ϸ�! ���� ��ġ: {iconInstance.transform.position}");
    }
}