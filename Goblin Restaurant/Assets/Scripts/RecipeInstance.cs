// ����: �÷��̾ ������ �����ϰ� �����Ű�� ���� �������� '���� ����'�� �����ϴ� ������ Ŭ�����Դϴ�.

[System.Serializable]
public class RecipeInstance
{
    /// <summary>
    /// �� �������� ���� ������(ScriptableObject)�Դϴ�.
    /// </summary>
    public RecipeData BaseData { get; private set; }

    /// <summary>
    /// �÷��̾ ��ȭ�� ���� �����Դϴ�.
    /// </summary>
    public int currentLevel;

    // --- ������ ---
    public RecipeInstance(RecipeData baseData)
    {
        BaseData = baseData;
        currentLevel = 1; // ��� �����Ǵ� 1�������� �����մϴ�.
    }

    /// <summary>
    /// �����Ǹ� ��ȭ�Ͽ� ������ 1 �ø��ϴ�.
    /// </summary>
    public void Upgrade()
    {
        // TODO: �ִ� ���� ���� ���� �߰� (��: 10����)
        currentLevel++;
        UnityEngine.Debug.Log($"{BaseData.recipeName} �����ǰ� {currentLevel}������ ��ȭ�Ǿ����ϴ�!");
    }
}
