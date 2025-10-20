using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class MenuPlannerUI_Controller : MonoBehaviour
{
    public GameObject recipeSelectionPanel;      //������ ���� �˾� �г�
    public GameObject selectableRecipePrefab;    //�˾� ��Ͽ� ǥ�õ� ������ ������ ������
    public Transform selectableRecipeContent; //�˾� ��ũ�� ���� Content ������Ʈ

    public List<DailyMenuSlotUI> dailyMenuSlots; //5���� ���� �޴� ���� UI ����Ʈ

    private DailyMenuSlotUI currentEditingSlot;

    void Awake()
    {
        foreach (var slot in dailyMenuSlots)
        {
            slot.Initialize(this);
        }
    }

    public void OpenRecipeSelectionPanel(DailyMenuSlotUI slot)
    {
        currentEditingSlot = slot; 
        recipeSelectionPanel.SetActive(true);
        recipeSelectionPanel.transform.SetAsLastSibling();
        UpdateSelectableRecipeList();
    }

    public void OnRecipeSelectedFromPopup(PlayerRecipe selectedRecipe)
    {
        if (currentEditingSlot != null)
        {
            MenuPlanner.instance.SetDailyMenu(currentEditingSlot.slotIndex, selectedRecipe, 1);
        }

        UpdateAllSlotsUI(); 
        recipeSelectionPanel.SetActive(false);
    }

    public void RemoveRecipeFromDailyMenu(DailyMenuSlotUI slot)
    {
        MenuPlanner.instance.SetDailyMenu(slot.slotIndex, null, 0);
        UpdateAllSlotsUI();
    }

    public void ChangeRecipeQuantity(PlayerRecipe recipe, int amount)
    {
        if (recipe == null) return;

        int currentQuantity = MenuPlanner.instance.GetQuantity(recipe.data.id);
        int maxQuantity = InventoryManager.instance.GetMaxCookableAmount(recipe);
        int newQuantity = currentQuantity + amount;

        if (newQuantity >= 1 && newQuantity <= maxQuantity)
        {
            MenuPlanner.instance.SetQuantity(recipe.data.id, newQuantity);
            UpdateAllSlotsUI(); 
        }
    }

    private void UpdateSelectableRecipeList()
    {
        foreach (Transform child in selectableRecipeContent)
        {
            Destroy(child.gameObject);
        }

        var alreadyAddedIDs = MenuPlanner.instance.dailyMenu.Where(r => r != null).Select(r => r.data.id);

        foreach (var playerRecipe in RecipeManager.instance.playerRecipes.Values)
        {
            if (!alreadyAddedIDs.Contains(playerRecipe.data.id))
            {
                bool canCook = InventoryManager.instance.CanCook(playerRecipe);

                GameObject itemGO = Instantiate(selectableRecipePrefab, selectableRecipeContent);
                itemGO.GetComponent<SelectableRecipeItemUI>().Setup(playerRecipe, canCook, this);
            }
        }
    }

    public void UpdateAllSlotsUI() // public���� ����
    {
        for (int i = 0; i < dailyMenuSlots.Count; i++)
        {
            PlayerRecipe recipe = MenuPlanner.instance.dailyMenu[i];
            if (recipe != null)
            {
                int quantity = MenuPlanner.instance.GetQuantity(recipe.data.id);
                dailyMenuSlots[i].SetData(recipe, quantity);

                // int max = InventoryManager.instance.GetMaxCookableAmount(recipe); // InventoryManager �ʿ�
                int max = 99; // �ӽ� �ִ� ����
                if (dailyMenuSlots[i].plusButton != null) dailyMenuSlots[i].plusButton.interactable = (quantity < max);
                if (dailyMenuSlots[i].minusButton != null) dailyMenuSlots[i].minusButton.interactable = (quantity > 1);
            }
            else
            {
                dailyMenuSlots[i].ClearData();
            }
        }

        bool canStartBusiness = MenuPlanner.instance.dailyMenu.Any(r => r != null);
        if (GameManager.instance != null)
        {
            // �� '���� ����' ��ư�� '����'�� �����ϵ��� �����մϴ�.
            GameManager.instance.SetStartButtonInteractable(canStartBusiness);
        }
    }
}