using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 전체의 세이브 데이터를 담는 최상위 클래스입니다.
/// </summary>
[System.Serializable]
public class GameSaveData
{
    public string saveVersion = "1.0";
    public string saveDate;

    // 각 매니저의 데이터
    public GameManagerData gameManagerData;
    public InventoryData inventoryData;
    public RecipeManagerData recipeManagerData;
    public EmployeeManagerData employeeManagerData;
    public FameManagerData fameManagerData;
    public QuestManagerData questManagerData;
}

/// <summary>
/// GameManager의 저장 데이터
/// </summary>
[System.Serializable]
public class GameManagerData
{
    public int totalGoldAmount;
    public int dayCount;
    public bool isRecipeUnlocked;
    public bool isEmployeeUnlocked;
}

/// <summary>
/// InventoryManager의 저장 데이터
/// </summary>
[System.Serializable]
public class InventoryData
{
    public List<IngredientEntry> ingredients = new List<IngredientEntry>();
    public List<string> discoveredIngredients = new List<string>();
}

[System.Serializable]
public class IngredientEntry
{
    public string ingredientID;
    public int amount;
}

/// <summary>
/// RecipeManager의 저장 데이터
/// </summary>
[System.Serializable]
public class RecipeManagerData
{
    public List<PlayerRecipeSaveData> playerRecipes = new List<PlayerRecipeSaveData>();
}

[System.Serializable]
public class PlayerRecipeSaveData
{
    public int recipeID;
    public int currentLevel;
}

/// <summary>
/// EmployeeManager의 저장 데이터
/// </summary>
[System.Serializable]
public class EmployeeManagerData
{
    public List<EmployeeInstanceSaveData> hiredEmployees = new List<EmployeeInstanceSaveData>();
}

/// <summary>
/// EmployeeInstance를 저장하기 위한 데이터 클래스
/// ScriptableObject 참조는 저장할 수 없으므로, ID나 이름으로 저장합니다.
/// </summary>
[System.Serializable]
public class EmployeeInstanceSaveData
{
    // 기본 정보
    public string baseDataSpeciesName; // BaseData를 찾기 위한 종족 이름
    public bool isProtagonist;
    public string firstName;
    public int currentLevel;
    public float currentExperience;
    public int skillPoints;
    public int currentSalary;

    // 능력치
    public int currentCookingStat;
    public int currentServingStat;
    public int currentCharmStat;

    // 특성 (Trait의 이름 리스트로 저장)
    public List<string> traitNames = new List<string>();

    // 역할
    public EmployeeRole assignedRole;

    // 등급
    public EmployeeGrade grade;
}

/// <summary>
/// FameManager의 저장 데이터
/// </summary>
[System.Serializable]
public class FameManagerData
{
    public float currentFamePoints;
    public int currentFameLevel;
}

/// <summary>
/// QuestManager의 저장 데이터
/// </summary>
[System.Serializable]
public class QuestManagerData
{
    public List<QuestSaveData> quests = new List<QuestSaveData>();
}

/// <summary>
/// 퀘스트의 진행 상태를 저장하기 위한 데이터
/// </summary>
[System.Serializable]
public class QuestSaveData
{
    public int questId;
    public bool isUnlocked;
    public bool isCompleted;
    public bool isRewardClaimed;
    public List<ProgressEntry> progressEntries = new List<ProgressEntry>();
}

[System.Serializable]
public class ProgressEntry
{
    public string key;
    public int value;
}
