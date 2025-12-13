using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewTutorialTopic", menuName = "Tutorial/Tutorial Topic Data")]
public class TutorialData : ScriptableObject
{
    [Header("주제 설정")]
    public string topicTitle; // 예: "영업 단계", "만족도"
    public List<TutorialStep> steps = new List<TutorialStep>(); // 페이지들
}

[System.Serializable]
public class TutorialStep
{
    [TextArea(3, 10)]
    public string description; // 설명 텍스트
    public Sprite contentImage; // 설명 이미지
}