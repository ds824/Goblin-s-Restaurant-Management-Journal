using UnityEngine;

// Ư�� �ϳ��� ���� ������ ��� ������ Ʋ�Դϴ�.
[CreateAssetMenu(fileName = "New Trait", menuName = "GoblinChef/Trait Data")]
public class Trait : ScriptableObject
{
    [Tooltip("UI�� ǥ�õ� Ư���� �̸� (��: ������, �丮��)")]
    public string traitName;

    [TextArea(3, 5)] // ���� �ٷ� ������ ���ϰ� �Է��� �� �ְ� ���ݴϴ�.
    [Tooltip("�� Ư���� ���� ����")]
    public string description;

    // TODO: ���߿� ���⿡ Ư���� ���� ȿ���� �����ϴ� �ڵ带 �߰��� �� �ֽ��ϴ�.
    // (��: public float cookingStatBonus;)
}
