// ���� �̸�: RecipeData.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New RecipeData", menuName = "GoblinChef/Recipe Data")]
public class RecipeData : ScriptableObject
{
    [Header("������ ����")]
    public string recipeName; // ������ �̸�
    public string description;  // ����
    public Sprite foodImage;  // ���� �̹���

    [Header("���׷��̵� �� ����")]
    public int level = 1;       // ���׷��̵� ����
    public int basePrice;       // �⺻ �Ǹ� ����
    public int pricePerLevel;   // ���� �� �߰� ����

    // ���� �Ǹ� ������ ����ϴ� ������Ƽ
    public int CurrentPrice => basePrice + (pricePerLevel * (level - 1));

    [Header("�ʿ� ���")]
    public List<Ingredient> requiredIngredients; // �ʿ��� ��� ���
}

// ��� ������ ���� ������ Ŭ�����Դϴ�.
// RecipeData�� ���� ���Ͽ� �־ �������ϴ�.
[System.Serializable]
public class Ingredient
{
    public string ingredientName;
    public int count;
}