// ����: ���ӿ� ���Ǵ� ��� �ɷ�ġ�� ����(Enum)�� ������ ������ �����մϴ�.

// [�ٽ� 1] �ɷ�ġ ������ ������(Enum)���� �����մϴ�.
// ���߿� '�丮 �ӵ�', '��� ����' ���� �߰��ϰ� ������ ���⿡ �߰��� �ϸ� �˴ϴ�.
public enum StatType
{
    // �丮 ����
    Cooking_Speed,      // �丮 �ӵ�
    Cooking_Quality,    // �丮 ǰ��
    Ingredient_Preparation, // ��� ���� (�丮 ǰ���� ���ʽ�)
    Plating_Skill,      // �÷����� ��� (�丮 ��޿� ���� ����)

    // ���� ����
    Serving_Speed,      // ���� �ӵ�
    Movement_Speed,     // �̵� �ӵ�
    Cleaning_Speed,     // ���̺� ���� �ӵ�
    Order_Capacity,     // ó�� ������ �ֹ� ����

    // ���� ������� ������ �̷��� ���� Ȯ�� ����
    Charisma,           // �ŷ� (�� ȹ�淮 ���� ��)
    Stamina,            // ü��

    // ���� �ɷ�ġ (�ٸ� �������� ����)
    Leadership,         // ������ (�ֺ� ������ ȿ�� ����)
    Teaching_Ability    // ���� �ɷ� (���� ���� �ӵ� ����)
}

// [�ٽ� 2] ���� �ɷ�ġ�� ������ ������ �����մϴ�.
// �� ����ü�� '�ɷ�ġ ����'�� '�⺻��'�� �� ������ �����ݴϴ�.
[System.Serializable]
public struct CharacterStat
{
    public StatType type;
    public int baseValue;
}

