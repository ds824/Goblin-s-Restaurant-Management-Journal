using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState { Preparing, Open, Closing }
    public GameState currentState;

    public float dayDurationInSeconds = 600f; // ���� �Ϸ� ���� (10��)
    public int totalGoldAmount = 0; // �� ��� ���� �߰�
    private float currentTimeOfDay;
    private int DayCount = 1; // ��ĥ°���� ���� ���� �߰�
    private float timeScale; // ���� �� �ð� �帧 �ӵ�

    public TextMeshProUGUI timeText; // ȭ�鿡 �ð��� ǥ���� UI �ؽ�Ʈ
    public TextMeshProUGUI dayText; // ȭ�鿡 ��¥�� ǥ���� UI �ؽ�Ʈ
    public TextMeshProUGUI totalGold; // ȭ�鿡 �� ��带 ǥ���� UI �ؽ�Ʈ

    private InputSystem_Actions inputActions; // ������ Input Action C# Ŭ����

    private void Awake()
    {
        inputActions = new InputSystem_Actions();

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.GameManager.OpenStore.performed += OpenTheStore;
        inputActions.GameManager.NextDay.performed += MoveToNextDay; //�� ���� �ؽ�Ʈ���̸� ó �߰��ؾ��µ� �̺��Ű��� ����Ƽ�� �ν��� ���ϴ°Ŵ�???? ������� �����ݾ� ������� ������
    }

    private void OnDisable()
    {
        inputActions.GameManager.OpenStore.performed -= OpenTheStore;
        inputActions.GameManager.NextDay.performed -= MoveToNextDay;
        inputActions.Disable();
    }

    void Start()
    {
        currentState = GameState.Preparing;
        // ���� �� 9�ð�(09~18��)�� ���� 10������ ���
        timeScale = (9 * 60 * 60) / dayDurationInSeconds;
        currentTimeOfDay = 9 * 3600; // ���� 9�ÿ��� ���� (�� ����)
        timeText.text = "09:00";
        dayText.text = "Day " + DayCount;
        totalGold.text = "Gold: " + totalGoldAmount; // �� ��� �ʱ�ȭ
        Debug.Log("���� �غ� �ð��Դϴ�.");
    }

    void Update()
    {
        // ���� � ������ ���� �ð��� �帧
        if (currentState == GameState.Open)
        {
            currentTimeOfDay += Time.deltaTime * timeScale;

            // �ð� UI ������Ʈ (��: 13:30)
            int hours = (int)(currentTimeOfDay / 3600);
            int minutes = (int)((currentTimeOfDay % 3600) / 60);
            timeText.text = string.Format("{0:D2}:{1:D2}", hours, minutes);
            dayText.text = "Day " + DayCount;

            // 18�ð� �Ǹ� ���� ���·� ����
            if (currentTimeOfDay >= 18 * 3600)
            {
                currentState = GameState.Closing;
                Debug.Log("���� ����");
            }
        }

        if (currentState == GameState.Preparing)
        {
            timeText.text = "09:00";
            dayText.text = "Day " + DayCount;
        }
    }

    private void OpenTheStore(InputAction.CallbackContext context)
    {
        if (currentState == GameState.Preparing)
        {
            currentState = GameState.Open;
            Debug.Log("���� ����");
        }
    }

    private void MoveToNextDay(InputAction.CallbackContext context)
    {
        if (currentState == GameState.Closing)
        {
            currentState = GameState.Preparing;
            currentTimeOfDay = 9 * 3600; // ���� �� ���� 9�÷� �ʱ�ȭ
            DayCount += 1; // ��ĥ°���� ����
            Debug.Log("���� �� �غ� �����մϴ�.");
        }
    }

    public void AddGold(int amount)
    {
        totalGoldAmount += amount;
        totalGold.text = "Gold: " + totalGoldAmount; // UI ������Ʈ
    }
}
