using UnityEngine;


[CreateAssetMenu(fileName = "Ingredient", menuName = "Game Data/Ingredient Data")]
public class IngredientData : ScriptableObject
{
    [Header("��� ���� ����")]
    public string id; // ��Ḧ �����ϴ� ���� ID 
    public string ingredientName; // ���ӿ� ǥ�õ� �̸�
    public Sprite icon; // UI�� ǥ�õ� ������

    [Header("��� ��� �� ����")]
    public Rarity rarity; // ����� ��͵� (�Ϲ�, ���, ���, ����)
    public int buyPrice; // �������� ������ ���� ����
}
