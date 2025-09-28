using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState { Preparing, Open, Closing }
    public GameState _currentState;
    public GameState currentState
    {
        get { return _currentState; }
        set
        {
            _currentState = value;
            UpdateButtonUI();

            if (_currentState == GameState.Closing)
            {
                ShowSettlementPanal(); // ���� ���� �г� ǥ��
            }
        }
    }

    public float dayDurationInSeconds = 600f; // ���� �Ϸ� ���� (10��)
    public int totalGoldAmount = 0; // �� ��� ���� �߰�
    private int todaysGold = 0; // ���� �� ���
    private int todaysCustomers = 0; // ���� �湮�� �� ��
    private float currentTimeOfDay;
    private int DayCount = 1; // ��ĥ°���� ���� ���� �߰�
    private float timeScale; // ���� �� �ð� �帧 �ӵ�

    public TextMeshProUGUI timeText; // ȭ�鿡 �ð��� ǥ���� UI �ؽ�Ʈ
    public TextMeshProUGUI dayText; // ȭ�鿡 ��¥�� ǥ���� UI �ؽ�Ʈ
    public TextMeshProUGUI totalGold; // ȭ�鿡 �� ��带 ǥ���� UI �ؽ�Ʈ

    public GameObject OpenButton; // ���� ��ư ui
    public GameObject NextDayButton; // ���� �� ��ư ui

    public GameObject settlementPanel;
    public GameObject CheckButton;
    public TextMeshProUGUI todaysGoldText;
    public TextMeshProUGUI totalGoldText;
    public TextMeshProUGUI customerCountText;

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
    }

    private void OnDisable()
    {
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
            todaysGold = 0; // ���� �� ��� �ʱ�ȭ
            todaysCustomers = 0; // ���� �湮�� �� �ʱ�ȭ
        }
    }

    private void UpdateButtonUI()
    {
        // �� ���¿� �´� ��ư�� Ȱ��ȭ(true)�ϰ� �������� ��Ȱ��ȭ(false)
        OpenButton.SetActive(currentState == GameState.Preparing);
        NextDayButton.SetActive(currentState == GameState.Closing);
    }

    public void OpenTheStore()
    {
        if (currentState == GameState.Preparing)
        {
            currentState = GameState.Open;
            Debug.Log("���� ����");
        }
    }

    public void MoveToNextDay()
    {
        if (currentState == GameState.Closing)
        {
            currentState = GameState.Preparing;
            currentTimeOfDay = 9 * 3600; // ���� �� ���� 9�÷� �ʱ�ȭ
            DayCount += 1; // ��ĥ°���� ����
            Debug.Log("���� �� �غ� �����մϴ�.");
        }
    }

    public void closePanal()
    {
        settlementPanel.SetActive(false); // ���� ���� �г� �ݱ�
        CheckButton.SetActive(false); // Ȯ�� ��ư �ݱ�
    }

    private void ShowSettlementPanal()
    {
        todaysGoldText.text = $"���� Ȯ���� ��差: {todaysGold}";
        totalGoldText.text = $"�� ���� ���: {totalGoldAmount}";
        customerCountText.text = $"���� �湮�� ��: {todaysCustomers}";

        settlementPanel.SetActive(true); // ���� ���� �г� ����
        CheckButton.SetActive(true); // Ȯ�� ��ư ����
    }

    public void AddGold(int amount)
    {
        totalGoldAmount += amount; // �� ��忡 �߰�
        todaysGold += amount; // ���� �� ��忡 �߰�
        totalGold.text = "Gold: " + totalGoldAmount; // UI ������Ʈ
    }

    public void AddCustomerCount()
    {
        todaysCustomers += 1; // ���� �湮�� �� �� ����
    }
}
