using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using System.Collections.Generic;

public class MenuPlanner : MonoBehaviour
{
    public static MenuPlanner instance;
    public PlayerRecipe[] dailyMenu = new PlayerRecipe[5];
    public Dictionary<int, int> dailyMenuQuantities = new Dictionary<int, int>();

    private void Awake()
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

    public void SetDailyMenu(int slotIndex, PlayerRecipe recipe)
    {
        if (slotIndex >= 0 && slotIndex < 5)
        {
            dailyMenu[slotIndex] = recipe;
        }
    }

    public void ClearDailyMenu()
    {
        dailyMenu = new PlayerRecipe[5];

        dailyMenuQuantities.Clear();
    }

    public void SetDailyMenu(int slotIndex, PlayerRecipe recipe, int quantity)
    {
        if (slotIndex >= 0 && slotIndex < 5)
        {
            if (dailyMenu[slotIndex] != null)
            {
                dailyMenuQuantities.Remove(dailyMenu[slotIndex].data.id);
            }

            dailyMenu[slotIndex] = recipe;
            if (recipe != null)
            {
                dailyMenuQuantities[recipe.data.id] = quantity;
            }
        }
    }

    public void SetQuantity(int recipeId, int quantity)
    {
        if (dailyMenuQuantities.ContainsKey(recipeId))
        {
            dailyMenuQuantities[recipeId] = quantity;
        }
    }

    public int GetQuantity(int recipeId)
    {
        dailyMenuQuantities.TryGetValue(recipeId, out int quantity);
        return quantity;
    }

    public void ConsumeIngredientsForToday()
    {
        Debug.Log("������ �޴��� ���� ��Ḧ �Ҹ��մϴ�.");

        // '������ �޴�' �迭�� ��ȸ�մϴ�.
        foreach (PlayerRecipe recipe in dailyMenu)
        {
            // �� ������ �ǳʶݴϴ�.
            if (recipe == null) continue;

            // �ش� �������� �Ǹ� ������ �����ɴϴ�.
            int quantity = GetQuantity(recipe.data.id);

            // �ش� �����ǿ� �ʿ��� �� ��Ḧ ��ȸ�մϴ�.
            foreach (IngredientRequirement requirement in recipe.data.requiredIngredients)
            {
                // �� �ʿ� ���� ��� (��� �䱸�� * �Ǹ� ����)
                int totalAmountNeeded = requirement.amount * quantity;

                // InventoryManager�� ��� �Ҹ� ��û�մϴ�.
                if (totalAmountNeeded > 0)
                {
                    InventoryManager.instance.ConsumeIngredient(requirement.ingredientID, totalAmountNeeded);
                }
            }
        }
    }
}
