using UnityEngine;

// ����� ��͵��� �����ϴ� ������(Enum)�Դϴ�.
public enum IngredientRarity
{
    Common,  // �Ϲ�
    Uncommon, // ���
    Rare,    // ���
    Legendary// ����
}

// ScriptableObject�� ��ӹ޾� ����� '���� ������'�� �����ϴ� Ŭ�����Դϴ�.
[CreateAssetMenu(fileName = "New Ingredient", menuName = "GoblinChef/Ingredient Data")]
public class IngredientData : ScriptableObject
{
    [Header("�⺻ ����")]
    [Tooltip("����� �̸� (��: ����� �а���)")]
    public string ingredientName;
    [Tooltip("��� ������ �̹���")]
    public Sprite icon;

    [Header("����")]
    [Tooltip("����� ��͵� ���")]
    public IngredientRarity rarity;
    [Tooltip("�������� ������ ���� ����")]
    public int buyPrice;
}
