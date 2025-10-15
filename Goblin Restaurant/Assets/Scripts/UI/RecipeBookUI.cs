using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class RecipeBookUI : MonoBehaviour
{
    [Header("��� UI")]
    public TextMeshProUGUI collectionStatusText;
    public TMP_Dropdown sortDropdown;
    public TMP_Dropdown filterDropdown;

    [Header("���� ��� �г�")]
    public Transform recipeListContent;
    public GameObject recipeListItemPrefab;
    public Sprite filledStarSprite;
    public Sprite emptyStarSprite;

    [Header("���� �� ���� �г�")]
    public GameObject detailPanelParent;
    public Image detail_RecipeIcon;
    public TextMeshProUGUI detail_RecipeName;
    public Transform detail_GradeStarsParent;
    public TextMeshProUGUI detail_RecipeInfoText;
    public Transform detail_IngredientsParent;
    public GameObject ingredientIconPrefab;
    public TextMeshProUGUI detail_Description;
    public Button upgradeButton;

    private RecipeInstance selectedRecipe;
    private List<GameObject> spawnedListItems = new List<GameObject>();

    // [�ٽ� ����] UI�� Ȱ��ȭ�� ������ ����� ���ΰ�ħ�ϵ��� OnEnable �Լ��� ����մϴ�.
    void OnEnable()
    {
        RefreshRecipeList();
        // UI�� ���� ���� �׻� �� ����â�� ����ϴ�.
        if (detailPanelParent != null) detailPanelParent.SetActive(false);
    }

    void Start()
    {
        if (upgradeButton != null) upgradeButton.onClick.AddListener(UpgradeSelectedRecipe);
    }

    public void OpenRecipeBook()
    {
        gameObject.SetActive(true);
    }

    public void CloseRecipeBook()
    {
        gameObject.SetActive(false);
    }

    void RefreshRecipeList()
    {
        // ���� ��� �����۵��� ��� �����մϴ�.
        foreach (GameObject item in spawnedListItems)
        {
            Destroy(item);
        }
        spawnedListItems.Clear();

        // RecipeManager�� �غ�Ǿ����� Ȯ���մϴ�.
        if (RecipeManager.Instance == null) return;

        List<RecipeInstance> currentRecipes = RecipeManager.Instance.ownedRecipes;

        // ����� �� �����ǿ� ���� UI �������� �����ϰ� ������ ä��ϴ�.
        foreach (RecipeInstance recipe in currentRecipes)
        {
            GameObject listItem = Instantiate(recipeListItemPrefab, recipeListContent);

            Image icon = listItem.transform.Find("Icon")?.GetComponent<Image>();
            TextMeshProUGUI nameText = listItem.transform.Find("NameText")?.GetComponent<TextMeshProUGUI>();
            Transform starsParent = listItem.transform.Find("GradeStarsContainer");
            Button itemButton = listItem.GetComponent<Button>();

            if (icon != null) icon.sprite = recipe.BaseData.icon;
            if (nameText != null) nameText.text = recipe.BaseData.recipeName;

            if (starsParent != null) UpdateGradeStars(starsParent, recipe.BaseData.GetGrade(recipe.currentLevel));

            // �� ��� �������� Ŭ���ϸ� ShowRecipeDetails �Լ��� ����ǵ��� �����մϴ�.
            if (itemButton != null)
            {
                itemButton.onClick.RemoveAllListeners();
                itemButton.onClick.AddListener(() => ShowRecipeDetails(recipe));
            }

            spawnedListItems.Add(listItem);
        }

        // ���� ��Ȳ �ؽ�Ʈ�� ������Ʈ�մϴ�.
        if (collectionStatusText != null)
            collectionStatusText.text = $"���� ��Ȳ: {currentRecipes.Count}/{RecipeManager.Instance.allRecipesInGame.Count}";
    }

    void ShowRecipeDetails(RecipeInstance recipe)
    {
        selectedRecipe = recipe;
        if (detailPanelParent == null) return;

        detailPanelParent.SetActive(true);

        if (detail_RecipeIcon != null) detail_RecipeIcon.sprite = recipe.BaseData.icon;
        if (detail_RecipeName != null) detail_RecipeName.text = recipe.BaseData.recipeName;
        if (detail_Description != null) detail_Description.text = recipe.BaseData.description;

        if (detail_RecipeInfoText != null)
            detail_RecipeInfoText.text = $"���� ����: Lv.{recipe.currentLevel}\n�Ǹ� ����: {recipe.BaseData.basePrice}���\n�丮 �ð�: {recipe.BaseData.baseCookTime}��";

        if (detail_GradeStarsParent != null)
            UpdateGradeStars(detail_GradeStarsParent, recipe.BaseData.GetGrade(recipe.currentLevel));

        // TODO: �ʿ� ��� �����ܵ��� �����ϰ� ǥ���ϴ� ����
    }

    void UpdateGradeStars(Transform starsParent, RecipeGrade grade)
    {
        int gradeNumber = (int)grade + 1;
        for (int i = 0; i < starsParent.childCount; i++)
        {
            Image starImage = starsParent.GetChild(i).GetComponent<Image>();
            if (starImage == null) continue;

            if (i < gradeNumber) { starImage.sprite = filledStarSprite; }
            else { starImage.sprite = emptyStarSprite; }
        }
    }

    void UpgradeSelectedRecipe()
    {
        if (selectedRecipe != null)
        {
            // TODO: ��ȭ ���/��� Ȯ�� ����
            selectedRecipe.Upgrade();
            RefreshRecipeList();
            ShowRecipeDetails(selectedRecipe);
        }
    }
}