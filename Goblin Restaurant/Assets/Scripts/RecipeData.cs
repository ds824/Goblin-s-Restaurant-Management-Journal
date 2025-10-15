using UnityEngine;
using System.Collections.Generic;
using System.Linq; // LINQ�� ����Ͽ� ��͵��� ���� ����մϴ�.

// �������� ����� �����ϴ� ������(Enum)�Դϴ�.
public enum RecipeGrade
{
    Ordinary,    // ����� (1��)
    Skilled,     // ���õ� (2��)
    FirstClass,  // �Ϸ� (3��)
    Artisan,     // ������ (4��)
    Master       // �밡�� (5��)
}

// �����Ǹ� ����� ���� �ʿ��� ���� �� ������ ��� �����ϴ� Ŭ�����Դϴ�.
[System.Serializable]
public class IngredientRequirement
{
    [Tooltip("�ʿ��� ����� ���� ������")]
    public IngredientData ingredient;
    [Tooltip("�ʿ��� ����� ����")]
    public int amount;
}

[CreateAssetMenu(fileName = "New Recipe", menuName = "GoblinChef/Recipe Data")]
public class RecipeData : ScriptableObject
{
    [Header("�⺻ ����")]
    [Tooltip("�������� �̸� (��: ��� ���ɷ�)")]
    public string recipeName;
    [Tooltip("������ ������ �̹���")]
    public Sprite icon;
    [TextArea(3, 5)]
    [Tooltip("�����ǿ� ���� ����")]
    public string description;

    [Header("�ٽ� �ɷ�ġ")]
    [Tooltip("1���� ���� �⺻ �Ǹ� ����")]
    public int basePrice;
    [Tooltip("1���� ���� �⺻ �丮 �ð�(��)")]
    public float baseCookTime;

    [Header("�ʿ� ���")]
    [Tooltip("�� �����Ǹ� ����� �� �ʿ��� ��� ���")]
    public List<IngredientRequirement> requirements;

    // --- ���� ��� ������Ƽ ---

    /// <summary>
    /// �ʿ� ��� �� ���� ���� ��͵��� �ڵ����� ����Ͽ� ��ȯ�մϴ�.
    /// </summary>
    public IngredientRarity Rarity
    {
        get
        {
            if (requirements == null || !requirements.Any())
            {
                return IngredientRarity.Common;
            }
            // ��� ����� ��͵� �� ���� ���� ���� ã�� ��ȯ�մϴ�.
            return requirements.Max(req => req.ingredient.rarity);
        }
    }

    /// <summary>
    /// ���� �������� ������ ���� ���(����)�� �ڵ����� ����Ͽ� ��ȯ�մϴ�.
    /// </summary>
    public RecipeGrade GetGrade(int currentLevel)
    {
        if (currentLevel >= 9) return RecipeGrade.Master;
        if (currentLevel >= 7) return RecipeGrade.Artisan;
        if (currentLevel >= 5) return RecipeGrade.FirstClass;
        if (currentLevel >= 3) return RecipeGrade.Skilled;
        return RecipeGrade.Ordinary;
    }
}