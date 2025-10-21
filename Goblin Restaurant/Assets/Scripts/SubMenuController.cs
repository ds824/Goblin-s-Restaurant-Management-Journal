using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SubMenuController : MonoBehaviour
{
    // [Header("���� ��ư��")] - �ּ��� �����մϴ�.
    [Header("Main Buttons")]
    public Button buttonStore;
    public Button buttonRecipe;
    public Button buttonInterior;
    public Button buttonEmployee;

    // [Header("���� �޴� �гε�")]
    [Header("Sub Menu Panels")]
    public GameObject subMenuStore;
    public GameObject subMenuRecipe;
    public GameObject subMenuInterior;
    public GameObject subMenuEmployee;

    // [Header("���� �޴� �ݱ� ��ư�� (�� �г� ����)")]
    [Header("Close Buttons (Inside Panels)")]
    public Button closeButtonStore;
    public Button closeButtonRecipe;
    public Button closeButtonInterior;
    public Button closeButtonEmployee;

    // [Header("ȭ�� ��Ӱ� ���� ���Ŀ")]
    [Header("Screen Blocker")]
    public GameObject blocker;

    private GameObject currentActiveSubMenu = null;

    // Null üũ�� ���� ��ư ��� ���� (Start()������ ���)
    private List<Button> allCloseButtons = new List<Button>();
    private List<Button> allMainButtons = new List<Button>();


    private void Awake()
    {
        // ��� �ݱ� ��ư�� ����Ʈ�� �߰��մϴ�. (Null�� �ƴ� ��ư��)
        if (closeButtonStore != null) allCloseButtons.Add(closeButtonStore);
        if (closeButtonRecipe != null) allCloseButtons.Add(closeButtonRecipe);
        if (closeButtonInterior != null) allCloseButtons.Add(closeButtonInterior);
        if (closeButtonEmployee != null) allCloseButtons.Add(closeButtonEmployee);

        // ��� ���� ��ư�� ����Ʈ�� �߰��մϴ�. (Null�� �ƴ� ��ư��)
        if (buttonStore != null) allMainButtons.Add(buttonStore);
        if (buttonRecipe != null) allMainButtons.Add(buttonRecipe);
        if (buttonInterior != null) allMainButtons.Add(buttonInterior);
        if (buttonEmployee != null) allMainButtons.Add(buttonEmployee);
    }


    private void Start()
    {
        // --- ���� ��ư Ŭ�� �̺�Ʈ �Ҵ� ---

        // Null üũ�� ���� ���� ����
        if (buttonStore != null)
            buttonStore.onClick.AddListener(() => ShowSubMenu(subMenuStore));
        if (buttonRecipe != null)
            buttonRecipe.onClick.AddListener(() => ShowSubMenu(subMenuRecipe));
        if (buttonInterior != null)
            buttonInterior.onClick.AddListener(() => ShowSubMenu(subMenuInterior));
        if (buttonEmployee != null)
            buttonEmployee.onClick.AddListener(() => ShowSubMenu(subMenuEmployee));

        // --- �ݱ� ��ư �̺�Ʈ �Ҵ� (NullReferenceException �ذ�) ---
        // Awake���� ������ ����Ʈ�� ����Ͽ� ���������� �̺�Ʈ ����
        foreach (Button closeBtn in allCloseButtons)
        {
            closeBtn.onClick.AddListener(HideAllSubMenus);
        }

        // --- ���Ŀ Ŭ�� �� �ݱ� (Null üũ ��ȭ) ---
        // TryGetComponent<Button> ��� GetComponent<Button>()�� ����ص� ������, TryGetComponent�� �� �����մϴ�.
        if (blocker != null && blocker.TryGetComponent<Button>(out var blockerBtn))
            blockerBtn.onClick.AddListener(HideAllSubMenus);

        // �ʱ� ���� ����
        HideAllSubMenus();
    }

    private void Update()
    {
        // InvalidOperationException �ذ��� ���� Input Settings�� 'Old Input Manager'�� �����ؾ� �մϴ�.
        // ESC Ű�� �ݱ�
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // �޴��� �������� ���� �ݱ�
            if (currentActiveSubMenu != null)
                HideAllSubMenus();
        }
    }

    private void ShowSubMenu(GameObject targetMenu)
    {
        // Ÿ�� �޴��� Null�̸� �������� ���� (������ ��ȭ)
        if (targetMenu == null) return;

        bool isSameMenu = (currentActiveSubMenu == targetMenu);

        HideAllSubMenus();

        if (!isSameMenu)
        {
            targetMenu.SetActive(true);
            currentActiveSubMenu = targetMenu;

            // ���Ŀ�� Null�� �ƴ� ��쿡�� �ѱ� (������ ��ȭ)
            if (blocker != null)
                blocker.SetActive(true);
        }
    }

    public void HideAllSubMenus()
    {
        // �� ���� �޴� �г��� Null�� �ƴ� ��쿡�� ��Ȱ��ȭ (������ ��ȭ)
        if (subMenuStore != null) subMenuStore.SetActive(false);
        if (subMenuRecipe != null) subMenuRecipe.SetActive(false);
        if (subMenuInterior != null) subMenuInterior.SetActive(false);
        if (subMenuEmployee != null) subMenuEmployee.SetActive(false);

        // ���Ŀ�� Null�� �ƴ� ��쿡�� ��Ȱ��ȭ (������ ��ȭ)
        if (blocker != null) blocker.SetActive(false);

        currentActiveSubMenu = null;
    }
}