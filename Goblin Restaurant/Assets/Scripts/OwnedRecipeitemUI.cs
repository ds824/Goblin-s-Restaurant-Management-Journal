using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OwnedRecipeItemUI : MonoBehaviour
{
    public Button selectButton;
    public TextMeshProUGUI recipeNameText;
    // (�߰�) public Image recipeIconImage;

    private PlayerRecipe myRecipe;
    private MenuPlannerUI_Controller controller; // ��Ʈ�ѷ��� ������ ������ ����

    // ��Ʈ�ѷ��κ��� ������ �޾� �ڽ��� �ʱ�ȭ�ϴ� �Լ�
    public void Setup(PlayerRecipe recipe, bool canSelect, MenuPlannerUI_Controller uiController)
    {
        myRecipe = recipe;
        controller = uiController; // ��Ʈ�ѷ� ���� ����

        recipeNameText.text = myRecipe.data.recipeName;
        // if (recipeIconImage != null) recipeIconImage.sprite = myRecipe.data.icon;

        // ���� ������ ������ ���� ��ư�� Ȱ��ȭ
        selectButton.interactable = canSelect;

        // ��ư Ŭ�� �� OnSelectButtonClick �Լ��� ȣ���ϵ��� ����
        selectButton.onClick.AddListener(OnSelectButtonClick);
    }

    void OnSelectButtonClick()
    {
        // �����ص� ��Ʈ�ѷ����� "���� ���Ⱦ�!" ��� ���� �˸�
        if (controller != null)
        {
            controller.OnRecipeSelectedFromPopup(myRecipe);
        }
    }
}