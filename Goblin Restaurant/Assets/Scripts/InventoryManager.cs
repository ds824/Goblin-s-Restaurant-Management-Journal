using UnityEngine;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager instance;

    public Dictionary<string, int> playerIngredients = new Dictionary<string, int>();

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

    public void AddIngredient(string ingredientID, int amount)
    {
        if (playerIngredients.ContainsKey(ingredientID))
        {
            playerIngredients[ingredientID] += amount;
        }
        else
        {
            playerIngredients[ingredientID] = amount;
        }
        Debug.Log($"��� {ingredientID} {amount}�� �߰�/�Ҹ�. ���� ����: {playerIngredients[ingredientID]}");
    }

    public void ConsumeIngredient(string ingredientID, int amount)
    {
        if (playerIngredients.ContainsKey(ingredientID))
        {
            playerIngredients[ingredientID] -= amount;

            if (playerIngredients[ingredientID] < 0)
            {
                Debug.LogWarning($"��� {ingredientID}�� ��� ���������� �����Ǿ����ϴ�. ����: {playerIngredients[ingredientID]}");
                playerIngredients[ingredientID] = 0;
            }
            Debug.Log($"��� '{ingredientID}' {amount}�� �Ҹ�. ���� ����: {playerIngredients[ingredientID]}");
        }
        else
        {
            Debug.LogError($"��� ���� ���({ingredientID})�� �Ҹ��Ϸ��� �մϴ�.");
        }
    }

    public bool CanCook(PlayerRecipe recipe)
    {
        return GetMaxCookableAmount(recipe) > 0;
    }

    public int GetMaxCookableAmount(PlayerRecipe recipe)
    {
        if (recipe == null) return 0;

        int maxAmount = int.MaxValue;
        foreach (var requirement in recipe.data.requiredIngredients)
        {
            playerIngredients.TryGetValue(requirement.ingredientID, out int ownedAmount);

            if (requirement.amount == 0) continue;

            int cookableAmount = ownedAmount / requirement.amount;
            if (cookableAmount < maxAmount)
            {
                maxAmount = cookableAmount;
            }
        }
        return maxAmount;
    }
}