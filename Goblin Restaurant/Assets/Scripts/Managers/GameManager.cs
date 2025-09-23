using UnityEngine;
using System.Collections; // �ڷ�ƾ�� ����ϱ� ���� �ʿ��մϴ�.

// ����: ������ �ð�, ��, �� �� �������� ���¸� �����ϰ� �ٸ� �Ŵ����鿡�� ��ȣ�� �����ϴ�.
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("���� �ð� ����")]
    [Tooltip("���� �� �Ϸ簡 ���� �ð����� �� ������ �����մϴ�.")]
    public float secondsPerDay = 5f;

    [Header("���� ���� ����")]
    [Tooltip("���� ������ ��¥")]
    public int currentDay = 1;
    [Tooltip("���� �Ĵ��� ����. �� ���� �ٲ㼭 �׽�Ʈ�� �� �ֽ��ϴ�.")]
    public int currentFame = 100;

    [Header("���ΰ� ����")]
    [Tooltip("���ΰ����� ����� ������ ���赵(EmployeeData ����)�� �������ּ���.")]
    public EmployeeData mainCharacterTemplate;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        CreateMainCharacter();
        StartCoroutine(DayCycleCoroutine());
    }

    // ���ΰ� ĳ���͸� �����ؼ� ��� ��Ͽ� �߰��ϴ� �Լ�
    void CreateMainCharacter()
    {
        if (mainCharacterTemplate != null)
        {
            EmployeeInstance mainCharacter = new EmployeeInstance(mainCharacterTemplate);
            EmployeeManager.Instance.hiredEmployees.Add(mainCharacter);

            // [������ �κ�] employeeName -> firstName
            // EmployeeInstance�� ����� ���ΰ��� �̸�(firstName)�� ����մϴ�.
            Debug.Log($"���ΰ� '{mainCharacter.firstName}'��(��) �Ĵ翡 �շ��߽��ϴ�!");
        }
    }

    // ������ �ð����� �Ϸ縦 �����Ű�� �ڷ�ƾ
    IEnumerator DayCycleCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(secondsPerDay);
            currentDay++;
            Debug.Log($"{currentDay}���� ��ħ�� �Ǿ����ϴ�.");

            if ((currentDay - 1) % 7 == 0 && currentDay > 1)
            {
                EmployeeManager.Instance.GenerateApplicants(currentFame);
            }
        }
    }
}