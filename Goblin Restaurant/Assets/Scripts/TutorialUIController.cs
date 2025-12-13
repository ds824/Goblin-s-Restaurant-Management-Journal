using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TutorialUIController : MonoBehaviour
{
    public static TutorialUIController instance;

    void Awake()
    {
        if (instance == null) instance = this;
    }

    [System.Serializable]
    public class TutorialCategory
    {
        public string categoryName;
        public Button categoryTabButton;
        public List<TutorialData> topics;
    }

    [Header("카테고리 설정")]
    public List<TutorialCategory> categories;

    [Header("Category Tab Style")]
    public Sprite tabNormalSprite;      
    public Sprite tabSelectedSprite;    
    public Color tabNormalColor = Color.white;
    public Color tabSelectedColor = new Color(0.7f, 0.7f, 0.7f, 1f); 

    [Header("UI 연결 - 네비게이션")]
    public Transform subTopicButtonParent;
    public GameObject subTopicButtonPrefab;

    [Header("UI 연결 - 콘텐츠 표시")]
    public GameObject contentPanel; 
    public TextMeshProUGUI titleText; 
    public TextMeshProUGUI descText; 
    public Image contentImage; 
    public Button nextButton; 
    public Button prevButton; 
    public Button closeButton; 

    [Header("UI 연결 - 전체")]
    public GameObject tutorialRootPanel; 

    // ▼▼▼ [추가] 블로커와 팝업 매니저 연결 변수 ▼▼▼
    public GameObject panelBlocker;
    public GameObject popupManager;
    // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

    [Header("Button Style")]
    public Sprite buttonNormalSprite;   
    public Sprite buttonSelectedSprite; 
    public Color buttonNormalColor = Color.white; 
    public Color buttonSelectedColor = new Color(0.7f, 0.7f, 0.7f, 1f); 

    private Image currentSelectedTabImage; 
    private Image currentSelectedBtnImage; 

    private TutorialData currentTopic;
    private int currentStepIndex = 0;

    void Start()
    {
        for (int i = 0; i < categories.Count; i++)
        {
            int index = i; 
            if (categories[i].categoryTabButton != null)
            {
                categories[i].categoryTabButton.onClick.AddListener(() => OnCategoryClicked(index));
            }
        }

        if (nextButton) nextButton.onClick.AddListener(OnNextStep);
        if (prevButton) prevButton.onClick.AddListener(OnPrevStep);
        if (closeButton) closeButton.onClick.AddListener(CloseTutorial);
    }

    // ▼▼▼ [수정] 튜토리얼 열 때 블로커/매니저 켜기 ▼▼▼ 
    public void OpenTutorialUI()
    {
        tutorialRootPanel.SetActive(true);
        
        if (panelBlocker != null) panelBlocker.SetActive(true);
        if (popupManager != null) popupManager.SetActive(true);

        if (categories.Count > 0) OnCategoryClicked(0);
    }
    // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

    // ▼▼▼ [수정] 튜토리얼 닫을 때 블로커/매니저 끄기 ▼▼▼
    public void CloseTutorial()
    {
        tutorialRootPanel.SetActive(false);

        // 주의: 일시정지 상태로 돌아갈 때는 TutorialManager가 알아서 처리하므로
        // 여기서는 기본적으로 끄는 동작을 수행합니다.
        // if (panelBlocker != null) panelBlocker.SetActive(false);
        // if (popupManager != null) popupManager.SetActive(false);

        if (TutorialManager.Instance != null) 
            TutorialManager.Instance.OnTutorialClosed();
    }
    // ▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲▲

    void OnCategoryClicked(int categoryIndex)
    {
        // 1. 탭 버튼 스타일 갱신
        if (categories[categoryIndex].categoryTabButton != null)
        {
            Image clickedTabImage = categories[categoryIndex].categoryTabButton.GetComponent<Image>();
            
            if (currentSelectedTabImage != null && currentSelectedTabImage != clickedTabImage)
            {
                UpdateCategoryButtonVisual(currentSelectedTabImage, false);
            }

            UpdateCategoryButtonVisual(clickedTabImage, true);
            currentSelectedTabImage = clickedTabImage;
        }

        // 2. 서브 토픽 버튼 생성
        foreach (Transform child in subTopicButtonParent) Destroy(child.gameObject);
        currentSelectedBtnImage = null;

        var selectedCategory = categories[categoryIndex];

        for (int i = 0; i < selectedCategory.topics.Count; i++)
        {
            var topic = selectedCategory.topics[i];
            
            GameObject btnObj = Instantiate(subTopicButtonPrefab, subTopicButtonParent);
            var btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null) btnText.text = topic.topicTitle;

            Image btnImage = btnObj.GetComponent<Image>();
            if (btnImage != null)
            {
                btnImage.sprite = buttonNormalSprite;
                btnImage.color = buttonNormalColor;
            }

            btnObj.GetComponent<Button>().onClick.AddListener(() => 
            {
                StartTopic(topic);
                SetSelectedButtonVisual(btnImage);
            });

            if (i == 0)
            {
                StartTopic(topic);
                SetSelectedButtonVisual(btnImage);
            }
        }

        if (selectedCategory.topics.Count == 0)
        {
            contentPanel.SetActive(false);
        }
    }

    void UpdateCategoryButtonVisual(Image targetImage, bool isSelected)
    {
        if (targetImage == null) return;

        if (isSelected)
        {
            if (tabSelectedSprite != null) targetImage.sprite = tabSelectedSprite;
            targetImage.color = tabSelectedColor;
        }
        else
        {
            if (tabNormalSprite != null) targetImage.sprite = tabNormalSprite;
            targetImage.color = tabNormalColor;
        }
    }

    void SetSelectedButtonVisual(Image newBtnImage)
    {
        if (newBtnImage == null) return;

        if (currentSelectedBtnImage != null)
        {
            if (buttonNormalSprite != null) 
                currentSelectedBtnImage.sprite = buttonNormalSprite;
            
            currentSelectedBtnImage.color = buttonNormalColor;
        }

        currentSelectedBtnImage = newBtnImage;

        if (buttonSelectedSprite != null) 
            currentSelectedBtnImage.sprite = buttonSelectedSprite;
        
        currentSelectedBtnImage.color = buttonSelectedColor; 
    }

    void StartTopic(TutorialData topic)
    {
        currentTopic = topic;
        currentStepIndex = 0;
        contentPanel.SetActive(true); 
        UpdateContentDisplay();
    }

    void UpdateContentDisplay()
    {
        if (currentTopic == null || currentTopic.steps.Count == 0) return;

        TutorialStep step = currentTopic.steps[currentStepIndex];

        if (titleText) titleText.text = currentTopic.topicTitle;
        if (descText) descText.text = step.description;

        if (contentImage)
        {
            if (step.contentImage != null)
            {
                contentImage.sprite = step.contentImage;
                contentImage.preserveAspect = true; 
                contentImage.gameObject.SetActive(true);
            }
            else
            {
                contentImage.gameObject.SetActive(false);
            }
        }

        if (prevButton) prevButton.interactable = (currentStepIndex > 0);
        if (nextButton) nextButton.interactable = (currentStepIndex < currentTopic.steps.Count - 1);
    }

    void OnNextStep()
    {
        if (currentTopic != null && currentStepIndex < currentTopic.steps.Count - 1)
        {
            currentStepIndex++;
            UpdateContentDisplay();
        }
    }

    void OnPrevStep()
    {
        if (currentStepIndex > 0)
        {
            currentStepIndex--;
            UpdateContentDisplay();
        }
    }
}