using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    public TutorialUIController uiController;

    [Header("설정")]
    public bool isFirstRun = true; // 저장 데이터와 연동 필요

    void Awake()
    {
        if (Instance == null) Instance = this;
    }

    void Start()
    {
        
        Debug.Log("[TutorialManager] 1. Start 함수 시작");
        OpenTutorial();
        // PlayerPrefs.SetInt("TutorialSeen", 1);
        // PlayerPrefs.Save();
        // isFirstRun = false;
    }

    // 외부(일시정지 메뉴 버튼 등)에서 호출
    public void OpenTutorial()
    {
        // 게임 일시 정지
        Time.timeScale = 0f;

        // 만약 일시정지 상태에서 튜토리얼을 열었다면, 일시정지 패널을 끈다.
        if (GameManager.instance != null && GameManager.instance.isPaused)
        {
            if (GameManager.instance.pausePanel != null)
            {
                GameManager.instance.pausePanel.SetActive(false);
            }
        }
        
        if (uiController != null)
        {
            uiController.OpenTutorialUI();
        }
    }

    // 튜토리얼 UI가 닫힐 때 호출됨
    public void OnTutorialClosed()
    {
        // ▼▼▼ [수정] 닫힐 때 게임 상태에 따라 분기 처리 ▼▼▼
        
        if (GameManager.instance != null)
        {
            // 튜토리얼 UI가 사용하던 PopupManager는 꺼준다.
            if (GameManager.instance.PopupManager != null)
                 GameManager.instance.PopupManager.SetActive(false);

            // 1. 게임이 일시 정지(Pause) 상태였다면 -> 다시 일시 정지 패널을 엽니다.
            if (GameManager.instance.isPaused)
            {
                if (GameManager.instance.pausePanel != null)
                    GameManager.instance.pausePanel.SetActive(true);
                
                // 일시정지 메뉴는 자체 블로커를 활성화하므로, 블로커를 켜둔다.
                if (GameManager.instance.panelBlocker != null)
                    GameManager.instance.panelBlocker.SetActive(true);
                
                // 시간은 여전히 멈춤 상태(0) 유지
            }
            // 2. 그냥 게임 도중 열었던 거라면 -> 게임 재개
            else
            {
                Time.timeScale = 1f;
                // 블로커 끄기
                if (GameManager.instance.panelBlocker != null)
                    GameManager.instance.panelBlocker.SetActive(false);
            }
        }
    }
}