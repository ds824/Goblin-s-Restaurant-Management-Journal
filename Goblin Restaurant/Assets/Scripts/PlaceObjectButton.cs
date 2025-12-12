using UnityEngine;
using UnityEngine.UI;

public class PlaceObjectButton : MonoBehaviour
{
    public enum ObjectType { Table, Stove }
    public ObjectType objectType;
    public int tablePrice = 100;
    public int stovePrice = 1000;


    private Button myButton;
    private bool isPurchased = false;

    void Awake()
    {
        myButton = GetComponent<Button>();
        myButton.onClick.AddListener(OnButtonClick);
    }

    void Update()
    {
        bool isPreparing = (GameManager.instance.currentState == GameManager.GameState.Preparing);
        bool shouldBeVisible = isPreparing && !isPurchased;
        if (gameObject.activeSelf != shouldBeVisible)
        {
            gameObject.SetActive(shouldBeVisible);
        }
        if (shouldBeVisible && myButton != null)
        {
            int price = 0;
            if (objectType == ObjectType.Table)
            {
                price = tablePrice;
            }
            else if (objectType == ObjectType.Stove)
            {
                price = stovePrice;
            }
            myButton.interactable = (GameManager.instance.totalGoldAmount >= price);
        }
    }

    public void OnButtonClick()
    {
        if (objectType == ObjectType.Table)
        {
            UpgradePanelController.instance.ShowTablePanel(this);
        }
        else if (objectType == ObjectType.Stove)
        {
            UpgradePanelController.instance.ShowStovePanel(this);
        }
    }

    public void SetPurchased()
    {
        isPurchased = true;
        gameObject.SetActive(false);
    }

    public int GetPrice()
    {
        if (objectType == ObjectType.Table)
        {
            return tablePrice;
        }
        else if (objectType == ObjectType.Stove)
        {
            return stovePrice;
        }
        return 0;
    }
}
