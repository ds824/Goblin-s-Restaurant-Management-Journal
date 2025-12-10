using UnityEngine;

/// <summary>
/// 저장/로드 기능을 테스트하기 위한 스크립트입니다.
/// F5키를 눌러 저장, F9키를 눌러 로드할 수 있습니다.
/// </summary>
public class SaveLoadTester : MonoBehaviour
{
    void Update()
    {
        // F5 키로 저장
        if (Input.GetKeyDown(KeyCode.F5))
        {
            if (SaveLoadManager.Instance != null)
            {
                SaveLoadManager.Instance.SaveGame();
                Debug.Log("[SaveLoadTester] F5 키 - 게임 저장됨");
            }
            else
            {
                Debug.LogError("[SaveLoadTester] SaveLoadManager를 찾을 수 없습니다!");
            }
        }

        // F9 키로 로드
        if (Input.GetKeyDown(KeyCode.F9))
        {
            if (SaveLoadManager.Instance != null)
            {
                SaveLoadManager.Instance.LoadGame();
                Debug.Log("[SaveLoadTester] F9 키 - 게임 로드됨");
            }
            else
            {
                Debug.LogError("[SaveLoadTester] SaveLoadManager를 찾을 수 없습니다!");
            }
        }

        // Delete 키로 저장 파일 삭제 (테스트용)
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            if (SaveLoadManager.Instance != null)
            {
                SaveLoadManager.Instance.DeleteSaveFile();
                Debug.Log("[SaveLoadTester] Delete 키 - 저장 파일 삭제됨");
            }
        }
    }

    void OnGUI()
    {
        // 화면 왼쪽 상단에 저장/로드 단축키 안내 표시
        GUI.Label(new Rect(10, 10, 300, 60),
            "F5: 게임 저장\nF9: 게임 로드\nDelete: 저장 파일 삭제");

        // 저장 파일 존재 여부 표시
        if (SaveLoadManager.Instance != null)
        {
            bool hasSave = SaveLoadManager.Instance.HasSaveFile();
            string saveStatus = hasSave ? "저장 파일 있음" : "저장 파일 없음";
            GUI.Label(new Rect(10, 80, 200, 20), saveStatus);
        }
    }
}
