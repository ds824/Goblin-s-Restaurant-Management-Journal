using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RecipeManager : MonoBehaviour
{
    public static RecipeManager instance;

    // ���ӿ� �����ϴ� ��� ������ ���� ������
    private Dictionary<int, RecipeData> allRecipeData = new Dictionary<int, RecipeData>();

    // �÷��̾ ȹ���ϰ� �����Ų ������ ���
    public Dictionary<int, PlayerRecipe> playerRecipes = new Dictionary<int, PlayerRecipe>();

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            LoadAllRecipeData();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UnlockRecipe(1001);
        UnlockRecipe(1002);
        UnlockRecipe(1003);
        UnlockRecipe(1003);
        UnlockRecipe(1004);
        UnlockRecipe(1005);
        UnlockRecipe(1006);
        UnlockRecipe(1007);
        UnlockRecipe(1008);
        UnlockRecipe(1009);
        UnlockRecipe(1010);

    }

    private void LoadAllRecipeData()
    {
        RecipeData[] recipes = Resources.LoadAll<RecipeData>("Recipes");
        foreach (RecipeData recipe in recipes)
        {
            allRecipeData[recipe.id] = recipe;
        }
        Debug.Log($"{allRecipeData.Count}���� ������ �����͸� �ε��߽��ϴ�.");
    }

    public void UnlockRecipe(int recipeId)
    {
        if (playerRecipes.ContainsKey(recipeId))
        {
            Debug.Log("�̹� ȹ���� �������Դϴ�. �ٸ� �������� ��ȯ�˴ϴ�.");
            return;
        }

        if (allRecipeData.ContainsKey(recipeId))
        {
            PlayerRecipe newPlayerRecipe = new PlayerRecipe(allRecipeData[recipeId]);
            playerRecipes[recipeId] = newPlayerRecipe;
            Debug.Log($"���ο� ������ '{newPlayerRecipe.data.recipeName}' ȹ��!");
        }
    }

    public void UpgradeRecipe(int recipeId)
    {
        if (playerRecipes.TryGetValue(recipeId, out PlayerRecipe recipeToUpgrade))
        {
            recipeToUpgrade.currentLevel++;
            Debug.Log($"'{recipeToUpgrade.data.recipeName}' �����ǰ� {recipeToUpgrade.currentLevel}������ �Ǿ����ϴ�!");
        }
    }

}
