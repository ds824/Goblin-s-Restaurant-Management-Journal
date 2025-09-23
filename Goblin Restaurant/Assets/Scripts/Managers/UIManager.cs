using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("���� UI �г�")]
    public GameObject managementUIParent;

    [Header("�� UI ���")]
    public Button hireTabButton;
    public Button manageTabButton;

    [Header("������ �г�")]
    public GameObject applicantListPanel;
    public GameObject manageEmployeePanel;

    [Header("������ ��� UI")]
    public Transform applicantCardParent;
    public GameObject applicantCardPrefab;

    [Header("���̾ƿ� ����")]
    [Tooltip("������ ����� �⺻ ���� ����")]
    public int normalLeftPadding = 20;
    [Tooltip("�����ڰ� 18���� �� ����� ���� ���� ����")]
    public int narrowLeftPadding = 5;
    [Tooltip("ȭ�鿡 ǥ���� �ִ� ������ ��")]
    public int maxApplicantsToShow = 18;

    [Header("�� �ð� ȿ��")]
    public Color normalTabColor = Color.white;
    public Color activeTabColor = new Color(0.8f, 0.9f, 1f);

    private List<GameObject> spawnedApplicantCards = new List<GameObject>();
    private bool isUIVisible = false;
    private GridLayoutGroup applicantGrid; // �׸��� ���̾ƿ� ������Ʈ�� ������ ����

    void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(gameObject); }

        // applicantCardParent���� GridLayoutGroup ������Ʈ�� �̸� ã�ƵӴϴ�.
        if (applicantCardParent != null)
        {
            applicantGrid = applicantCardParent.GetComponent<GridLayoutGroup>();
        }
    }

    void Start()
    {
        if (hireTabButton != null) hireTabButton.onClick.AddListener(() => OpenTab(applicantListPanel, hireTabButton));
        if (manageTabButton != null) manageTabButton.onClick.AddListener(() => OpenTab(manageEmployeePanel, manageTabButton));

        // ���� �� UI�� ������ �� ���·� �����մϴ�.
        isUIVisible = false;
        if (managementUIParent != null) managementUIParent.SetActive(isUIVisible);
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
        {
            isUIVisible = !isUIVisible;
            if (managementUIParent != null) managementUIParent.SetActive(isUIVisible);

            // UI�� �� ��, ��� ������ �г��� ���� �� ������ �ʱ�ȭ�մϴ�.
            if (isUIVisible)
            {
                if (applicantListPanel != null) applicantListPanel.SetActive(false);
                if (manageEmployeePanel != null) manageEmployeePanel.SetActive(false);
                if (hireTabButton != null) hireTabButton.GetComponent<Image>().color = normalTabColor;
                if (manageTabButton != null) manageTabButton.GetComponent<Image>().color = normalTabColor;
            }

            Debug.Log($"Tab Ű ����. UI ǥ�� ����: {isUIVisible}");
        }
    }

    void OpenTab(GameObject panelToShow, Button clickedButton)
    {
        if (applicantListPanel != null) applicantListPanel.SetActive(false);
        if (manageEmployeePanel != null) manageEmployeePanel.SetActive(false);
        if (panelToShow != null) panelToShow.SetActive(true);

        if (hireTabButton != null) hireTabButton.GetComponent<Image>().color = normalTabColor;
        if (manageTabButton != null) manageTabButton.GetComponent<Image>().color = normalTabColor;
        if (clickedButton != null) clickedButton.GetComponent<Image>().color = activeTabColor;
    }

    public void UpdateApplicantListUI(List<GeneratedApplicant> applicants)
    {
        // ������ ���� ���� ���� ������ �����մϴ�.
        if (applicantGrid != null)
        {
            // Take(maxApplicantsToShow).Count()�� ����� ���� ǥ�õ� ������ ���� �������� �Ǵ��մϴ�.
            if (applicants.Take(maxApplicantsToShow).Count() >= maxApplicantsToShow)
            {
                applicantGrid.padding.left = narrowLeftPadding;
            }
            else
            {
                applicantGrid.padding.left = normalLeftPadding;
            }
        }

        foreach (GameObject card in spawnedApplicantCards) { Destroy(card); }
        spawnedApplicantCards.Clear();

        // applicants ����Ʈ���� �ִ� maxApplicantsToShow ���������� �����ͼ� UI�� �����մϴ�.
        foreach (GeneratedApplicant applicant in applicants.Take(maxApplicantsToShow))
        {
            GameObject newCard = Instantiate(applicantCardPrefab, applicantCardParent);
            UpdateCardUI(newCard, applicant);
            spawnedApplicantCards.Add(newCard);
        }
    }

    private void UpdateCardUI(GameObject card, GeneratedApplicant applicant)
    {
        Image portraitImage = card.transform.Find("PortraitImage")?.GetComponent<Image>();
        TextMeshProUGUI nameText = card.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI statsText = card.transform.Find("StatsText")?.GetComponent<TextMeshProUGUI>();
        Button hireButton = card.transform.Find("HireButton")?.GetComponent<Button>();

        if (portraitImage != null) portraitImage.sprite = applicant.BaseSpeciesData.portrait;
        if (nameText != null) nameText.text = $"{applicant.GeneratedFirstName}\n<size=20>({applicant.BaseSpeciesData.speciesName})</size>";

        if (statsText != null)
        {
            var statsBuilder = new System.Text.StringBuilder();
            statsBuilder.AppendLine($"�丮: {applicant.GeneratedCookingStat}");
            statsBuilder.AppendLine($"����: {applicant.GeneratedServingStat}");
            statsBuilder.AppendLine($"����: {applicant.GeneratedCleaningStat}");
            if (applicant.GeneratedTraits.Any()) { statsBuilder.AppendLine($"\nƯ��: <color=yellow>{applicant.GeneratedTraits[0].traitName}</color>"); }
            statsText.text = statsBuilder.ToString();
        }
        if (hireButton != null)
        {
            hireButton.onClick.RemoveAllListeners();
            hireButton.onClick.AddListener(() => EmployeeManager.Instance.HireEmployee(applicant));
        }
    }
}
