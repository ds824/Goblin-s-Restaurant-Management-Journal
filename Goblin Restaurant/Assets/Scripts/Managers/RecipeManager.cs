using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ����: ���ӿ� �����ϴ� ��� �����ǿ� �÷��̾ ������ �����Ǹ� �����ϴ� �߾� �������Դϴ�.
public class RecipeManager : MonoBehaviour
{
    public static RecipeManager Instance { get; private set; }

    [Header("������ �����ͺ��̽�")]
    [Tooltip("���ӿ� �����ϴ� ��� �������� ����(.asset) ���")]
    public List<RecipeData> allRecipesInGame;

    [Header("�÷��̾� ������")]
    [Tooltip("�÷��̾ ���� �����ϰ� �ִ� ������ ���")]
    public List<RecipeInstance> ownedRecipes = new List<RecipeInstance>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ���ο� �����Ǹ� ȹ���Ͽ� '������ ����'(ownedRecipes)�� �߰��մϴ�.
    /// </summary>
    /// <param name="recipeToLearn">ȹ���� �������� ���� ������</param>
    public void LearnNewRecipe(RecipeData recipeToLearn)
    {
        // �̹� ��� ���������� Ȯ���մϴ�.
        if (ownedRecipes.Any(r => r.BaseData == recipeToLearn))
        {
            Debug.LogWarning($"{recipeToLearn.recipeName}��(��) �̹� ��� �������Դϴ�!");
            // TODO: ��ȹ���� ���� ��峪 ���� ��ȯ�Ͽ� �����ϴ� ���� �߰�
            return;
        }

        // ���ο� ������ �ν��Ͻ��� �����Ͽ� ��Ͽ� �߰��մϴ�.
        RecipeInstance newRecipe = new RecipeInstance(recipeToLearn);
        ownedRecipes.Add(newRecipe);
        Debug.Log($"���ο� ������ '{newRecipe.BaseData.recipeName}'��(��) ������ϴ�!");
    }
    // RecipeManager.cs�� Start() �Լ��� �߰�
    void Start()
    {
        if (allRecipesInGame.Count > 0)
        {
            LearnNewRecipe(allRecipesInGame[0]);
        }
    }
}
