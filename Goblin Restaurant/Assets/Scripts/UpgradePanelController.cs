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

        messageText.text = $"    ?\n(: {currentTableButton.GetPrice()} G)";

        OpenPanel();
    }

    // 2. [ű]      
    public void ShowStovePanel(PlaceObjectButton button)
    {
        currentType = UpgradeType.Stove;
        currentStoveButton = button;

        messageText.text = $"     ?\n(: {currentStoveButton.GetPrice()} G)";

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
        //    
        if (currentType == UpgradeType.Table)
        {
            ConfirmTablePurchase();
        }
        else if (currentType == UpgradeType.Stove)
        {
            ConfirmStovePurchase();
        }
    }

    //     (  )
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

    //     (ű)
    private void ConfirmStovePurchase()
    {
        if (GameManager.instance.totalGoldAmount >= currentStoveButton.GetPrice())
        {
            // GameManager         
            GameManager.instance.SpendGold(currentStoveButton.GetPrice());
            RestaurantManager.instance.UnlockNextStove(); 
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
        Debug.Log("   !");
        HidePanel();
        if (NotificationController.instance != null)
            NotificationController.instance.ShowNotification("   !");
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
