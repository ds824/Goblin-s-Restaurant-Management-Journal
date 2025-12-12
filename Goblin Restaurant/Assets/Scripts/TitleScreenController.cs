using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenController : MonoBehaviour
{
    [Header("이동할 게임 씬의 이름")]
    public string gameSceneName = "MainScene";
    public void OnGameStartClick()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnGameExitClick()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}