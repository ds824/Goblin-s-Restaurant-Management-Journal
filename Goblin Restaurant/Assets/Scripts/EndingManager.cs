using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EndingCutscene
{
    public Sprite cutsceneImage;   
    [TextArea(3, 5)]
    public string dialogue;        
}

public class EndingManager : MonoBehaviour
{
    [Header("UI Components")]
    public Image displayImage;       
    public TextMeshProUGUI displayText; 
    public Button nextButton;        
    public CanvasGroup contentCanvasGroup; 
    public CanvasGroup textCanvasGroup;    

    [Header("Data")]
    public List<EndingCutscene> cutscenes; 
    public string titleSceneName = "TitleScene"; 

    private int currentIndex = 0;
    private bool isTextShown = false; 
    private bool isAnimating = false; 

    void OnEnable()
    {
        currentIndex = 0;
        isTextShown = false;
        isAnimating = false;

        if (contentCanvasGroup != null) contentCanvasGroup.alpha = 0f;
        if (textCanvasGroup != null) textCanvasGroup.alpha = 0f;
        
        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(OnNextButtonClicked);
        }

        if (cutscenes.Count > 0)
        {
            StartCoroutine(ShowImageRoutine());
        }
    }

    void OnNextButtonClicked()
    {
        if (isAnimating) return; 

        if (!isTextShown)
        {
            StartCoroutine(ShowTextRoutine());
        }
        else
        {
            if (currentIndex < cutscenes.Count - 1)
            {
                StartCoroutine(TransitionToNextRoutine());
            }
            else
            {
                StartCoroutine(EndSequenceRoutine());
            }
        }
    }

    IEnumerator ShowImageRoutine()
    {
        isAnimating = true;
        isTextShown = false;

        SetCutsceneData(currentIndex); 
        
        if (textCanvasGroup != null) textCanvasGroup.alpha = 0f;

        float timer = 0f;
        while (timer < 1f)
        {
            // ▼▼▼ [수정] 현실 시간(Unscaled) 사용 ▼▼▼
            timer += Time.unscaledDeltaTime; 
            if(contentCanvasGroup != null) contentCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer);
            yield return null;
        }
        if(contentCanvasGroup != null) contentCanvasGroup.alpha = 1f;
        
        isAnimating = false;
    }

    IEnumerator ShowTextRoutine()
    {
        isAnimating = true;
        
        float timer = 0f;
        while (timer < 1f)
        {
            // ▼▼▼ [수정] 현실 시간 사용 ▼▼▼
            timer += Time.unscaledDeltaTime * 2f; 
            if(textCanvasGroup != null) textCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer);
            yield return null;
        }
        if(textCanvasGroup != null) textCanvasGroup.alpha = 1f;
        
        isTextShown = true; 
        isAnimating = false;
    }

    IEnumerator TransitionToNextRoutine()
    {
        isAnimating = true;

        Sprite currentSprite = cutscenes[currentIndex].cutsceneImage;
        Sprite nextSprite = cutscenes[currentIndex + 1].cutsceneImage;
        bool isSameImage = (currentSprite == nextSprite);

        // 1. 텍스트 페이드 아웃
        float timer = 0f;
        while (timer < 1f)
        {
            // ▼▼▼ [수정] 현실 시간 사용 ▼▼▼
            timer += Time.unscaledDeltaTime * 3f; 
            if(textCanvasGroup != null) textCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer);
            yield return null;
        }
        if(textCanvasGroup != null) textCanvasGroup.alpha = 0f;


        // 2. 이미지가 다르면 페이드 아웃
        if (!isSameImage)
        {
            timer = 0f;
            while (timer < 1f)
            {
                // ▼▼▼ [수정] 현실 시간 사용 ▼▼▼
                timer += Time.unscaledDeltaTime;
                if(contentCanvasGroup != null) contentCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer);
                yield return null;
            }
            if(contentCanvasGroup != null) contentCanvasGroup.alpha = 0f;
        }

        currentIndex++;
        SetCutsceneData(currentIndex);

        // 3. 이미지가 다르면 페이드 인
        if (!isSameImage)
        {
            timer = 0f;
            while (timer < 1f)
            {
                // ▼▼▼ [수정] 현실 시간 사용 ▼▼▼
                timer += Time.unscaledDeltaTime;
                if(contentCanvasGroup != null) contentCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer);
                yield return null;
            }
            if(contentCanvasGroup != null) contentCanvasGroup.alpha = 1f;
        }

        isTextShown = false; 
        isAnimating = false;
    }

    void SetCutsceneData(int index)
    {
        if (displayImage != null)
        {
            displayImage.sprite = cutscenes[index].cutsceneImage;
            displayImage.preserveAspect = true;
        }
        if (displayText != null) displayText.text = cutscenes[index].dialogue;
    }

    IEnumerator EndSequenceRoutine()
    {
        isAnimating = true;

        float timer = 0f;
        while (timer < 1f)
        {
            // ▼▼▼ [수정] 현실 시간 사용 ▼▼▼
            timer += Time.unscaledDeltaTime;
            if(contentCanvasGroup != null) contentCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer);
            yield return null;
        }
        
        // ▼▼▼ [수정] 현실 시간 대기 ▼▼▼
        yield return new WaitForSecondsRealtime(1f);
        
        Time.timeScale = 1f; 
        SceneManager.LoadScene(titleSceneName);
    }
}