using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class ShopController : MonoBehaviour
{

    public static ShopController Instance;

    [Header("UI")]
    public GameObject ShopPanel;
    public Transform shopInventoryGrid, playerInventoryGrid;
    public GameObject shopSlotPrefab;
    public TMP_Text playerMoneyText, shopTitleText;

    private ItemDictionary itemDictionary;
    private ShopNPC currentShop;
    private object originalItem;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    void Start()
    {
        itemDictionary = FindFirstObjectByType<ItemDictionary>();
        ShopPanel.SetActive(false);
        if(CurrencyController.Instance != null)
        {
            CurrencyController.Instance.OnGoldChanged += UpdateMoneyDisplay;
            UpdateMoneyDisplay(CurrencyController.Instance.GetGold());
        }
    }
    private void UpdateMoneyDisplay(int amount0)
    {
        if(playerMoneyText != null)
        {
            playerMoneyText.text = amount0.ToString();
        }
    }

    public void OpenShop(ShopNPC shop)
    {
        currentShop = shop;
        ShopPanel.SetActive(true);
        if (shopTitleText != null)
        {
            shopTitleText.text = shop.shopkeeperName;
            RefreshShopDisplay();
            RefreshPlayerInventoryDisplay();
            //PauseController.SetPause(true);
        }
    }

    public void CloseShop()
    {
        ShopPanel.SetActive(false);
        currentShop = null;
        //PauseController.SetPause(false);
    }

    public void RefreshShopDisplay()
    {
        if (currentShop == null) return;
        foreach (Transform child in shopInventoryGrid) Destroy(child.gameObject);

        foreach (var stockItem in currentShop.GetCurrentStock())
        {
            if (stockItem.quantity <= 0) continue;

            CreateShopSlot(shopInventoryGrid, stockItem.itemID, stockItem.quantity, true);
        }
    }
    public void RefreshPlayerInventoryDisplay()
    {
        if (InventoryController.Instance == null) return;
        foreach (Transform child in playerInventoryGrid) Destroy(child.gameObject);

        foreach (Transform slotTransform in InventoryController.Instance.inventoryPanel.transform)
        {
            Slot inventorySlot = slotTransform.GetComponent<Slot>();
            if(inventorySlot?.currentItem != null)
            {
                Item originalItem = inventorySlot.currentItem.GetComponent<Item>();
            }

           // CreateShopSlot(playerInventoryGrid, originalItem.ID, originalItem.quantity, false, inventorySlot);
        }
    }

    private void CreateShopSlot(Transform playerInventoryGrid, object iD, object quantity, bool v, Slot inventorySlot)
    {
        throw new NotImplementedException();
    }

    private void CreateShopSlot(Transform grid, int itemID, int quantity, bool isShop, Slot originalSlot = null)
    {
        GameObject slotObj = Instantiate(shopSlotPrefab, grid);
        GameObject itemPrefab = itemDictionary.GetItemPrefab(itemID);
        if (itemPrefab == null) return;

        GameObject itemInstance = Instantiate(itemPrefab, slotObj.transform);
        itemInstance.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        Item item = itemInstance.GetComponent<Item>();
        item.quantity = quantity;
        item.UpdateQuantityDisplay();

        int price = isShop ? item.buyPrice : item.GetSellPrice();

        ShopSlot slot = slotObj.GetComponent<ShopSlot>();
        slot.isShopSlot = isShop;
        slot.SetItem(itemInstance, price);

        //ItemHandler
    }


}
