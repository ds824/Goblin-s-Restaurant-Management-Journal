using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class ClosePopupInput : MonoBehaviour, IPointerClickHandler
{
    [Header("Panels")]
    public GameObject RecipeBookPanel;
    public GameObject ShopPanel;
    public GameObject InventoryPanel;
    public GameObject RecipeSelection;
    public GameObject MenuPlanner;
    public GameObject RecipeIngredientsPanel;
    public GameObject centralUpgradePanel; // 중앙 업그레이드 패널
    public GameObject QuantityPopupPanel;
    public GameObject QuestPanel;
    public GameObject EmployeePanel;
    public GameObject SettlementPanel;

    public GameObject PanelBlocker;
    public GameObject PopupManager;
    
    private InputSystem_Actions inputActions;

    private void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        inputActions.UI.Enable();
        
        // 닫기 키 연결
        inputActions.UI.ClosePopup.performed += OnClosePopup;
        
        // ESC(Pause) 키 연결
        var pauseAction = inputActions.UI.Get().FindAction("Pause");
        if (pauseAction != null)
        {
            pauseAction.performed += OnPauseInput;
        }
    }

    private void OnDisable()
    {
        inputActions.UI.ClosePopup.performed -= OnClosePopup;
        
        var pauseAction = inputActions.UI.Get().FindAction("Pause");
        if (pauseAction != null)
        {
            pauseAction.performed -= OnPauseInput;
        }
        
        inputActions.UI.Disable();
    }

    // 닫기 버튼용
    private void OnClosePopup(InputAction.CallbackContext context)
    {
        TryCloseTopPopup(); 
    }

    // 마우스 우클릭용
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            TryCloseTopPopup();
        }
    }

    // ESC 키 입력용
    public void OnPauseInput(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        // 로직이 TryCloseTopPopup에 통합되었으므로 호출만 합니다.
        TryCloseTopPopup();
    }

    // ▼▼▼ [통합된 로직] 튜토리얼 -> 팝업 -> 일시정지 순서로 처리 ▼▼▼
    public void TryCloseTopPopup()
    {
        // 1. [최우선] 튜토리얼 패널 확인
        if (TutorialUIController.instance != null && 
            TutorialUIController.instance.tutorialRootPanel.activeSelf)
        {
            TutorialUIController.instance.CloseTutorial();
            return; // 닫았으니 종료
        }

        // 2. [차순위] 일반 팝업 및 업그레이드 패널 확인
        if (QuantityPopupPanel != null && QuantityPopupPanel.activeSelf)
        {
            QuantityPopupPanel.SetActive(false);
            return;
        }

        if (SettlementPanel != null && SettlementPanel.activeSelf)
        {
            SettlementPanel.SetActive(false);
            PanelBlocker.SetActive(false);
            return;
        }

        if (MenuPlanner != null && MenuPlanner.activeSelf)
        {
            MenuPlanner.SetActive(false);
            RecipeSelection.SetActive(false);
            PanelBlocker.SetActive(false);
            return;
        }

        if (RecipeBookPanel != null && RecipeBookPanel.activeSelf)
        {
            RecipeBookPanel.SetActive(false);
            PanelBlocker.SetActive(false);
            return;
        }

        if (QuestPanel != null && QuestPanel.activeSelf)
        {
            QuestPanel.SetActive(false);
            PanelBlocker.SetActive(false);
            return;
        }

        if (EmployeePanel != null && EmployeePanel.activeSelf)
        {
            EmployeePanel.SetActive(false);
            return;
        }

        // 업그레이드 패널 (컨트롤러 경유)
        if (centralUpgradePanel != null && centralUpgradePanel.activeSelf)
        {
            var controller = centralUpgradePanel.GetComponent<UpgradePanelController>();
            if (controller != null)
            {
                controller.OnCancel();
            }
            else
            {
                centralUpgradePanel.SetActive(false);
                if(GameManager.instance.panelBlocker != null) 
                    GameManager.instance.panelBlocker.SetActive(false);
            }
            return;
        }

        if (ShopPanel != null && ShopPanel.activeSelf)
        {
            ShopPanel.SetActive(false);
            PanelBlocker.SetActive(false);
            return;
        }

        if (InventoryPanel != null && InventoryPanel.activeSelf)
        {
            InventoryPanel.SetActive(false);
            PanelBlocker.SetActive(false);
            return;
        }
        
        if (RecipeIngredientsPanel != null && RecipeIngredientsPanel.activeSelf)
        {
            RecipeIngredientsPanel.SetActive(false);
            return;
        }

        if (PopupManager != null && PopupManager.activeSelf)
        {
            PopupManager.SetActive(false);
            return;
        }

        // 3. [마지막] 아무것도 닫을 게 없었다면 -> 일시정지 토글
        if (GameManager.instance != null)
        {
            GameManager.instance.TogglePause();
        }
    }
}