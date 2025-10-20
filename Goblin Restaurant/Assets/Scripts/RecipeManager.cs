using UnityEngine;
using System.Collections.Generic;

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager instance;

    public Dictionary<int, PlayerRecipe> playerRecipes = new Dictionary<int, PlayerRecipe>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UnlockRecipe(1001);
        UnlockRecipe(1002);
    }

    public void UnlockRecipe(int recipeId)
    {
        if (playerRecipes.ContainsKey(recipeId))
        {
            Debug.Log("�̹� ȹ���� �������Դϴ�.");
            return;
        }

        RecipeData recipeData = GameDataManager.instance.GetRecipeDataById(recipeId);
        if (recipeData != null)
        {
            PlayerRecipe newPlayerRecipe = new PlayerRecipe(recipeData);
            playerRecipes[recipeId] = newPlayerRecipe;
            Debug.Log($"���ο� ������ '{newPlayerRecipe.data.recipeName}' ȹ��!");
        }
    }

    public void UpgradeRecipe(int recipeId)
    {
        if (playerRecipes.TryGetValue(recipeId, out PlayerRecipe recipeToUpgrade))
        {
            // ��� �� ��� �Ҹ� ���� Ȯ�� ���� (���߿� ����)
            recipeToUpgrade.currentLevel++;
            Debug.Log($"'{recipeToUpgrade.data.recipeName}' �����ǰ� {recipeToUpgrade.currentLevel}������ �Ǿ����ϴ�!");
        }
    }
}
