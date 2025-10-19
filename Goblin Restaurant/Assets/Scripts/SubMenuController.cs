using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SubMenuController : MonoBehaviour
{
    [Header("���� ��ư��")]
    public Button buttonStore;
    public Button buttonRecipe;
    public Button buttonInterior;
    public Button buttonEmployee;

    [Header("���� �޴� �гε�")]
    public GameObject subMenuStore;
    public GameObject subMenuRecipe;
    public GameObject subMenuInterior;
    public GameObject subMenuEmployee;

    [Header("���� �޴� �ݱ� ��ư�� (�� �г� ����)")]
    public Button closeButtonStore;
    public Button closeButtonRecipe;
    public Button closeButtonInterior;
    public Button closeButtonEmployee;

    [Header("ȭ�� ��Ӱ� ���� ���Ŀ")]
    public GameObject blocker;

    private GameObject currentActiveSubMenu = null;

    private void Start()
    {
        // ���� ��ư Ŭ�� �� ������ �޴� ����
        buttonStore.onClick.AddListener(() => ShowSubMenu(subMenuStore));
        buttonRecipe.onClick.AddListener(() => ShowSubMenu(subMenuRecipe));
        buttonInterior.onClick.AddListener(() => ShowSubMenu(subMenuInterior));
        buttonEmployee.onClick.AddListener(() => ShowSubMenu(subMenuEmployee));

        // �ݱ� ��ư �̺�Ʈ
        closeButtonStore.onClick.AddListener(HideAllSubMenus);
        closeButtonRecipe.onClick.AddListener(HideAllSubMenus);
        closeButtonInterior.onClick.AddListener(HideAllSubMenus);
        closeButtonEmployee.onClick.AddListener(HideAllSubMenus);

        // ���Ŀ Ŭ�� �� �ݱ�
        if (blocker.TryGetComponent<Button>(out var blockerBtn))
            blockerBtn.onClick.AddListener(HideAllSubMenus);

        // �ʱ� ����
        HideAllSubMenus();
    }

    private void Update()
    {
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
        bool isSameMenu = (currentActiveSubMenu == targetMenu);

        HideAllSubMenus();

        if (!isSameMenu)
        {
            targetMenu.SetActive(true);
            currentActiveSubMenu = targetMenu;
            blocker.SetActive(true); // ���Ŀ �ѱ�
        }
    }

    public void HideAllSubMenus()
    {
        subMenuStore.SetActive(false);
        subMenuRecipe.SetActive(false);
        subMenuInterior.SetActive(false);
        subMenuEmployee.SetActive(false);

        blocker.SetActive(false);
        currentActiveSubMenu = null;
    }
}
