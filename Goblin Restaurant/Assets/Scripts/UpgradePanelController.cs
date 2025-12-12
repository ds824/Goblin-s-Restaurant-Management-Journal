using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradePanelController : MonoBehaviour
{
    public static UpgradePanelController instance;

    [Header("UI ")]
    public GameObject panel;
    public TextMeshProUGUI messageText;
    public Button confirmButton;
    public Button cancelButton;

    //      (   vs  )
    private enum UpgradeType { Table, Stove }
    private UpgradeType currentType;

    //  
    private PlaceObjectButton currentTableButton; //   
    private PlaceObjectButton currentStoveButton; //   

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        confirmButton.onClick.AddListener(OnConfirm);
        cancelButton.onClick.AddListener(OnCancel);
        panel.SetActive(false);
    }

    // 1. []      
    public void ShowTablePanel(PlaceObjectButton button)
    {
        currentType = UpgradeType.Table;
        currentTableButton = button; 

        messageText.text = $"테이블을 추가하시겠습니까?\n(가격: {currentTableButton.GetPrice()} G)";

        OpenPanel();
    }

    // 2. [신규] 화구 구매 창 표시
    public void ShowStovePanel(PlaceObjectButton button)
    {
        currentType = UpgradeType.Stove;
        currentStoveButton = button;

        messageText.text = $"화구를 추가하시겠습니까?\n(가격: {currentStoveButton.GetPrice()} G)";

        OpenPanel();
    }

    private void OpenPanel()
    {
        panel.SetActive(true);
        panel.transform.SetAsLastSibling();
        if (GameManager.instance.panelBlocker != null) 
            GameManager.instance.panelBlocker.SetActive(true);
    }

    private void OnConfirm()
    {
        // 상황에 따라 다른 로직 실행
        if (currentType == UpgradeType.Table)
        {
            ConfirmTablePurchase();
        }
        else if (currentType == UpgradeType.Stove)
        {
            ConfirmStovePurchase();
        }
    }

    // 테이블 구매 확정 (기존 코드)
    private void ConfirmTablePurchase()
    {
        if (GameManager.instance.totalGoldAmount >= currentTableButton.GetPrice())
        {
            GameManager.instance.SpendGold(currentTableButton.GetPrice());
            GameManager.instance.AddTable(currentTableButton.transform);
            currentTableButton.SetPurchased();
            HidePanel();
        }
        else
        {
            OnInsufficientGold();
        }
    }

    // 화구 구매 확정 (신규)
    private void ConfirmStovePurchase()
    {
        if (GameManager.instance.totalGoldAmount >= currentStoveButton.GetPrice())
        {
            GameManager.instance.SpendGold(currentStoveButton.GetPrice());
            RestaurantManager.instance.AddStove(currentStoveButton.transform);
            currentStoveButton.SetPurchased();
            HidePanel();
        }
        else
        {
            OnInsufficientGold();
        }
    }

    private void OnInsufficientGold()
    {
        Debug.Log("골드가 부족합니다!");
        HidePanel();
        if (NotificationController.instance != null)
            NotificationController.instance.ShowNotification("골드가 부족합니다!");
    }

    public void OnCancel()
    {
        HidePanel();
    }

    private void HidePanel()
    {
        panel.SetActive(false);
        if (GameManager.instance.panelBlocker != null) 
            GameManager.instance.panelBlocker.SetActive(false);
    }
}
