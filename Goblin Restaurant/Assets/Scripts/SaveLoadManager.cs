using UnityEngine;
using System.IO;
using System;
using System.Collections;

/// <summary>
/// 게임 데이터를 저장하고 불러오는 기능을 담당하는 매니저입니다.
/// </summary>
public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }

    private string saveFilePath;
    private const string SAVE_FILE_NAME = "gameSave.json";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 저장 파일 경로 설정
        saveFilePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
        Debug.Log($"[SaveLoadManager] 저장 파일 경로: {saveFilePath}");
    }

    /// <summary>
    /// 현재 게임 상태를 파일로 저장합니다.
    /// </summary>
    public void SaveGame()
    {
        try
        {
            GameSaveData saveData = new GameSaveData();
            saveData.saveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // 각 매니저로부터 데이터 수집
            saveData.gameManagerData = CollectGameManagerData();
            saveData.inventoryData = CollectInventoryData();
            saveData.recipeManagerData = CollectRecipeManagerData();
            saveData.employeeManagerData = CollectEmployeeManagerData();
            saveData.fameManagerData = CollectFameManagerData();
            saveData.questManagerData = CollectQuestManagerData();

            // JSON으로 직렬화
            string json = JsonUtility.ToJson(saveData, true);

            // 파일로 저장
            File.WriteAllText(saveFilePath, json);

            Debug.Log($"[SaveLoadManager] 게임이 저장되었습니다: {saveFilePath}");

            if (NotificationController.instance != null)
                NotificationController.instance.ShowNotification("게임이 저장되었습니다!");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveLoadManager] 저장 실패: {e.Message}");

            if (NotificationController.instance != null)
                NotificationController.instance.ShowNotification("저장에 실패했습니다!");
        }
    }

    /// <summary>
    /// 저장된 파일로부터 게임 상태를 불러옵니다.
    /// </summary>
    public void LoadGame()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogWarning("[SaveLoadManager] 저장 파일이 존재하지 않습니다.");

            if (NotificationController.instance != null)
                NotificationController.instance.ShowNotification("저장 파일이 없습니다!");
            return;
        }

        StartCoroutine(LoadGameCoroutine());
    }

    private IEnumerator LoadGameCoroutine()
    {
        // 파일 존재 여부 확인
        if (!File.Exists(saveFilePath))
        {
            Debug.LogError("[SaveLoadManager] 저장 파일을 찾을 수 없습니다.");
            yield break;
        }

        // 파일 읽기
        string json = null;
        GameSaveData saveData = null;

        // JSON 파싱 (yield 밖에서 처리)
        bool parseSuccess = false;
        try
        {
            json = File.ReadAllText(saveFilePath);
            saveData = JsonUtility.FromJson<GameSaveData>(json);
            parseSuccess = (saveData != null);
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveLoadManager] 파일 읽기 실패: {e.Message}");
            if (NotificationController.instance != null)
                NotificationController.instance.ShowNotification("로드에 실패했습니다!");
            yield break;
        }

        if (!parseSuccess || saveData == null)
        {
            Debug.LogError("[SaveLoadManager] 저장 데이터를 읽을 수 없습니다.");
            if (NotificationController.instance != null)
                NotificationController.instance.ShowNotification("로드에 실패했습니다!");
            yield break;
        }

        // 각 매니저에 데이터 복원
        RestoreGameManagerData(saveData.gameManagerData);
        RestoreInventoryData(saveData.inventoryData);
        RestoreRecipeManagerData(saveData.recipeManagerData);
        RestoreFameManagerData(saveData.fameManagerData);
        RestoreQuestManagerData(saveData.questManagerData);

        // 직원 데이터는 프레임을 기다린 후 복원 (씬 준비 보장)
        yield return new WaitForEndOfFrame();
        RestoreEmployeeManagerData(saveData.employeeManagerData);

        Debug.Log($"[SaveLoadManager] 게임이 로드되었습니다. (저장 날짜: {saveData.saveDate})");

        if (NotificationController.instance != null)
            NotificationController.instance.ShowNotification("게임이 로드되었습니다!");
    }

    /// <summary>
    /// 저장 파일이 존재하는지 확인합니다.
    /// </summary>
    public bool HasSaveFile()
    {
        return File.Exists(saveFilePath);
    }

    /// <summary>
    /// 저장 파일을 삭제합니다.
    /// </summary>
    public void DeleteSaveFile()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
            Debug.Log("[SaveLoadManager] 저장 파일이 삭제되었습니다.");
        }
    }

    // ========== 데이터 수집 메서드 ==========

    private GameManagerData CollectGameManagerData()
    {
        if (GameManager.instance == null) return null;

        return new GameManagerData
        {
            totalGoldAmount = GameManager.instance.totalGoldAmount,
            dayCount = GameManager.instance.DayCount,
            isRecipeUnlocked = GameManager.instance.isRecipeUnlocked,
            isEmployeeUnlocked = GameManager.instance.isEmployeeUnlocked
        };
    }

    private InventoryData CollectInventoryData()
    {
        if (InventoryManager.instance == null) return null;

        InventoryData data = new InventoryData();

        foreach (var kvp in InventoryManager.instance.playerIngredients)
        {
            data.ingredients.Add(new IngredientEntry
            {
                ingredientID = kvp.Key,
                amount = kvp.Value
            });
        }

        data.discoveredIngredients.AddRange(InventoryManager.instance.discoveredIngredients);

        return data;
    }

    private RecipeManagerData CollectRecipeManagerData()
    {
        if (RecipeManager.instance == null) return null;

        RecipeManagerData data = new RecipeManagerData();

        foreach (var kvp in RecipeManager.instance.playerRecipes)
        {
            data.playerRecipes.Add(new PlayerRecipeSaveData
            {
                recipeID = kvp.Key,
                currentLevel = kvp.Value.currentLevel
            });
        }

        return data;
    }

    private EmployeeManagerData CollectEmployeeManagerData()
    {
        if (EmployeeManager.Instance == null) return null;

        EmployeeManagerData data = new EmployeeManagerData();

        foreach (var employee in EmployeeManager.Instance.hiredEmployees)
        {
            EmployeeInstanceSaveData empData = new EmployeeInstanceSaveData
            {
                baseDataSpeciesName = employee.BaseData.speciesName,
                isProtagonist = employee.isProtagonist,
                firstName = employee.firstName,
                currentLevel = employee.currentLevel,
                currentExperience = employee.currentExperience,
                skillPoints = employee.skillPoints,
                currentSalary = employee.currentSalary,
                currentCookingStat = employee.currentCookingStat,
                currentServingStat = employee.currentServingStat,
                currentCharmStat = employee.currentCharmStat,
                assignedRole = employee.assignedRole,
                grade = employee.grade
            };

            // 특성 이름 저장
            if (employee.currentTraits != null)
            {
                foreach (var trait in employee.currentTraits)
                {
                    if (trait != null)
                        empData.traitNames.Add(trait.traitName);
                }
            }

            data.hiredEmployees.Add(empData);
        }

        return data;
    }

    private FameManagerData CollectFameManagerData()
    {
        if (FameManager.instance == null) return null;

        return new FameManagerData
        {
            currentFamePoints = FameManager.instance.CurrentFamePoints,
            currentFameLevel = FameManager.instance.CurrentFameLevel
        };
    }

    private QuestManagerData CollectQuestManagerData()
    {
        if (QuestManager.Instance == null) return null;

        QuestManagerData data = new QuestManagerData();

        foreach (var quest in QuestManager.Instance.allQuests)
        {
            QuestSaveData questData = new QuestSaveData
            {
                questId = quest.id,
                isUnlocked = quest.isUnlocked,
                isCompleted = quest.isCompleted,
                isRewardClaimed = quest.isRewardClaimed
            };

            // 진행 상태 저장
            if (quest.progressDict != null)
            {
                foreach (var kvp in quest.progressDict)
                {
                    questData.progressEntries.Add(new ProgressEntry
                    {
                        key = kvp.Key,
                        value = kvp.Value
                    });
                }
            }

            data.quests.Add(questData);
        }

        return data;
    }

    // ========== 데이터 복원 메서드 ==========

    private void RestoreGameManagerData(GameManagerData data)
    {
        if (GameManager.instance == null || data == null) return;

        GameManager.instance.totalGoldAmount = data.totalGoldAmount;
        GameManager.instance.DayCount = data.dayCount;
        GameManager.instance.isRecipeUnlocked = data.isRecipeUnlocked;
        GameManager.instance.isEmployeeUnlocked = data.isEmployeeUnlocked;

        // UI 업데이트
        if (GameManager.instance.totalGold != null)
            GameManager.instance.totalGold.text = data.totalGoldAmount.ToString();

        if (GameManager.instance.dayText != null)
            GameManager.instance.dayText.text = "Day " + data.dayCount;
    }

    private void RestoreInventoryData(InventoryData data)
    {
        if (InventoryManager.instance == null || data == null) return;

        InventoryManager.instance.playerIngredients.Clear();
        foreach (var entry in data.ingredients)
        {
            InventoryManager.instance.playerIngredients[entry.ingredientID] = entry.amount;
        }

        InventoryManager.instance.discoveredIngredients.Clear();
        foreach (var ingredientID in data.discoveredIngredients)
        {
            InventoryManager.instance.discoveredIngredients.Add(ingredientID);
        }
    }

    private void RestoreRecipeManagerData(RecipeManagerData data)
    {
        if (RecipeManager.instance == null || data == null) return;

        RecipeManager.instance.playerRecipes.Clear();
        foreach (var recipeData in data.playerRecipes)
        {
            RecipeData baseRecipeData = GameDataManager.instance.GetRecipeDataById(recipeData.recipeID);
            if (baseRecipeData != null)
            {
                PlayerRecipe recipe = new PlayerRecipe(baseRecipeData);
                recipe.currentLevel = recipeData.currentLevel;
                RecipeManager.instance.playerRecipes[recipeData.recipeID] = recipe;
            }
        }
    }

    private void RestoreEmployeeManagerData(EmployeeManagerData data)
    {
        if (EmployeeManager.Instance == null)
        {
            Debug.LogError("[SaveLoadManager] EmployeeManager.Instance가 null입니다!");
            return;
        }

        if (data == null)
        {
            Debug.LogWarning("[SaveLoadManager] 직원 데이터가 null입니다.");
            return;
        }

        Debug.Log($"[SaveLoadManager] 직원 복원 시작... 총 {data.hiredEmployees.Count}명");

        // 기존 직원들을 맵에서 제거 (씬에 있는 직원 오브젝트들 정리)
        Employee[] existingEmployees = FindObjectsOfType<Employee>();
        Debug.Log($"[SaveLoadManager] 기존 직원 {existingEmployees.Length}명 제거 중...");
        foreach (var emp in existingEmployees)
        {
            Destroy(emp.gameObject);
        }

        EmployeeManager.Instance.hiredEmployees.Clear();

        // RestaurantManager 확인
        if (RestaurantManager.instance == null)
        {
            Debug.LogError("[SaveLoadManager] RestaurantManager.instance가 null입니다! 직원 스폰 불가!");
            return;
        }

        int restoredCount = 0;
        foreach (var empData in data.hiredEmployees)
        {
            // BaseData 찾기
            EmployeeData baseData = null;

            // 주인공인 경우 GameManager의 mainCharacterTemplate 사용
            if (empData.isProtagonist && GameManager.instance != null && GameManager.instance.mainCharacterTemplate != null)
            {
                baseData = GameManager.instance.mainCharacterTemplate;
                Debug.Log($"[SaveLoadManager] 주인공 '{empData.firstName}' - mainCharacterTemplate 사용");
            }
            else
            {
                // 일반 직원은 allSpeciesTemplates에서 찾기
                baseData = EmployeeManager.Instance.allSpeciesTemplates
                    .Find(template => template.speciesName == empData.baseDataSpeciesName);
            }

            if (baseData == null)
            {
                Debug.LogWarning($"[SaveLoadManager] '{empData.baseDataSpeciesName}' 종족 데이터를 찾을 수 없습니다.");
                continue;
            }

            if (baseData.speciesPrefab == null)
            {
                Debug.LogError($"[SaveLoadManager] '{empData.baseDataSpeciesName}' 종족의 프리팹이 null입니다!");
                continue;
            }

            // EmployeeInstance 생성 (데이터만 복원)
            EmployeeInstance employee = new EmployeeInstance(baseData);
            employee.isProtagonist = empData.isProtagonist;
            employee.firstName = empData.firstName;
            employee.currentLevel = empData.currentLevel;
            employee.currentExperience = empData.currentExperience;
            employee.skillPoints = empData.skillPoints;
            employee.currentSalary = empData.currentSalary;
            employee.currentCookingStat = empData.currentCookingStat;
            employee.currentServingStat = empData.currentServingStat;
            employee.currentCharmStat = empData.currentCharmStat;
            employee.assignedRole = empData.assignedRole;
            employee.grade = empData.grade;

            // 특성 복원
            employee.currentTraits.Clear();
            foreach (var traitName in empData.traitNames)
            {
                Trait trait = baseData.possibleTraits?.Find(t => t.traitName == traitName);
                if (trait != null)
                    employee.currentTraits.Add(trait);
            }

            EmployeeManager.Instance.hiredEmployees.Add(employee);

            // 씬에 직원 스폰 (프리팹 생성)
            Debug.Log($"[SaveLoadManager] '{employee.firstName}' 직원 스폰 시도...");
            RestaurantManager.instance.SpawnSingleWorker(employee, baseData.speciesPrefab);
            restoredCount++;
        }

        Debug.Log($"[SaveLoadManager] 직원 복원 완료! {restoredCount}명 스폰됨");

        // UI 갱신
        if (EmployeeUI_Controller.Instance != null)
        {
            EmployeeUI_Controller.Instance.UpdateHiredEmployeeListUI();
        }
    }

    private void RestoreFameManagerData(FameManagerData data)
    {
        if (FameManager.instance == null || data == null) return;

        FameManager.instance.SetFameData(data.currentFamePoints, data.currentFameLevel);
    }

    private void RestoreQuestManagerData(QuestManagerData data)
    {
        if (QuestManager.Instance == null || data == null) return;

        // 활성 퀘스트 리스트 초기화
        QuestManager.Instance.activeQuests.Clear();

        foreach (var questSaveData in data.quests)
        {
            QuestData quest = QuestManager.Instance.allQuests.Find(q => q.id == questSaveData.questId);
            if (quest != null)
            {
                quest.isUnlocked = questSaveData.isUnlocked;
                quest.isCompleted = questSaveData.isCompleted;
                quest.isRewardClaimed = questSaveData.isRewardClaimed;

                // 진행 상태 복원
                quest.progressDict.Clear();
                foreach (var entry in questSaveData.progressEntries)
                {
                    quest.progressDict[entry.key] = entry.value;
                }

                // 언락된 퀘스트는 activeQuests에 추가
                if (quest.isUnlocked)
                {
                    QuestManager.Instance.activeQuests.Add(quest);
                }
            }
        }
    }
}
