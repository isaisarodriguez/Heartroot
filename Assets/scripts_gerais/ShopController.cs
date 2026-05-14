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

        if (CurrencyController.Instance != null)
        {
            CurrencyController.Instance.OnGoldChanged += UpdateMoneyDisplay;
            UpdateMoneyDisplay(CurrencyController.Instance.GetGold());
        }
    }

    private void UpdateMoneyDisplay(int amount)
    {
        if (playerMoneyText != null)
        {
            playerMoneyText.text = amount.ToString();
        }
    }

    public void OpenShop(ShopNPC shop)
    {
        currentShop = shop;
        ShopPanel.SetActive(true);
        if (shopTitleText != null)
        {
            // Atribui o nome do dono da loja ao título da UI
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

        // Limpa os slots visualmente antes de criar os novos
        foreach (Transform child in shopInventoryGrid) Destroy(child.gameObject);

        foreach (var stockItem in currentShop.GetCurrentStock())
        {
            if (stockItem.quantity <= 0) continue;

            // Cria o slot da loja com base no stock do NPC
            CreateShopSlot(shopInventoryGrid, stockItem.itemID, stockItem.quantity, true);
        }
    }

    public void RefreshPlayerInventoryDisplay()
    {
        if (InventoryController.Instance == null) return;

        // Limpa os slots do lado do jogador na UI da loja
        foreach (Transform child in playerInventoryGrid) Destroy(child.gameObject);

        foreach (Transform slotTransform in InventoryController.Instance.inventoryPanel.transform)
        {
            Slot inventorySlot = slotTransform.GetComponent<Slot>();
            if (inventorySlot?.currentItem != null)
            {
                Item originalItemScript = inventorySlot.currentItem.GetComponent<Item>();

                // Cria o slot para venda baseado no que o jogador tem no inventário real
                CreateShopSlot(playerInventoryGrid, originalItemScript.ID, originalItemScript.quantity, false, inventorySlot);
            }
        }
    }

    // Função principal que cria os itens visualmente na loja
    private void CreateShopSlot(Transform grid, int itemID, int quantity, bool isShop, Slot originalSlot = null)
    {
        // O "throw new NotImplementedException" que estava aqui foi removido para o código não crashar!

        GameObject slotObj = Instantiate(shopSlotPrefab, grid);

        // Pede ao dicionário o prefab correto (Anel, Bota ou Diário)
        GameObject itemPrefab = itemDictionary.GetItemPrefab(itemID);

        if (itemPrefab == null)
        {
            Debug.LogWarning("ID do item não encontrado no Dicionário!");
            return;
        }

        GameObject itemInstance = Instantiate(itemPrefab, slotObj.transform);

        // Garante que o item fica centrado no quadrado branco
        if (itemInstance.GetComponent<RectTransform>() != null)
        {
            itemInstance.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        Item item = itemInstance.GetComponent<Item>();
        item.quantity = quantity;
        item.UpdateQuantityDisplay();

        // Se for loja usa preço de compra, se for inventário usa preço de venda
        int price = isShop ? item.buyPrice : item.GetSellPrice();

        ShopSlot slot = slotObj.GetComponent<ShopSlot>();
        if (slot != null)
        {
            slot.isShopSlot = isShop;
            slot.SetItem(itemInstance, price);
        }

        //ItemHandler - Espaço para lógica futura de cliques no item
    }
}