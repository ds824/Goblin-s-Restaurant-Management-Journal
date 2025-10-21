using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class IngredientRequirement
{
    public string ingredientID;
    public int amount;
}

public enum  Rarity { common, Uncommon, Rare, Legendary}

[CreateAssetMenu(fileName = "RecipeData", menuName = "Scriptable Objects/RecipeData")]
public class RecipeData : ScriptableObject
{
    [Header("������ ���� ����")]
    public int id;
    public string recipeName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("���� ������Ʈ")]
    public GameObject foodPrefab;

    [Header("�⺻ ����")]
    public int basePrice;
    public float baseCookTime;
    public Rarity rarity;
    public List<IngredientRequirement> requiredIngredients;
}

