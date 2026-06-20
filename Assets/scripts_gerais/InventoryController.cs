using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public int slotCount;
    public GameObject[] itemPrefabs;

    public static InventoryController Instance { get; private set; }
    Dictionary<int, int> itemsCountCache = new();
    public event Action OnInventoryChanged;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RebuildItemCounts();
        for (int i = 0; i < slotCount; i++)
        {
            Slot slot = Instantiate(slotPrefab, inventoryPanel.transform).GetComponent<Slot>();
            if (i < itemPrefabs.Length)
            {
                GameObject item = Instantiate(itemPrefabs[i], slot.transform);
                item.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.currentItem = item;
                RebuildItemCounts();

            }
        }

    }

    public void RebuildItemCounts()
    {
        itemsCountCache.Clear();

        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            Slot slot = slotTransform.GetComponent<Slot>();
            if(slot.currentItem != null)
            {
                Item item = slot.currentItem.GetComponent<Item>();
                if (item != null)
                {
                    itemsCountCache[item.ID] = itemsCountCache.GetValueOrDefault(item.ID, 0) + item.quantity;
                }
                
            }
        }

        OnInventoryChanged?.Invoke();     
    }

    public Dictionary<int, int> GetItemCounts() => itemsCountCache;
    public bool CheckItem(int itemID)
    {
        // Verifica se o dicionário tem o ID e se a quantidade dele é maior que 0
        return itemsCountCache.ContainsKey(itemID) && itemsCountCache[itemID] > 0;
    }
    public bool AddItem(GameObject itemPrefab)
    {
        //Look for an empty slot
        foreach (Transform slotTransform in inventoryPanel.transform) { 
        Slot slot = slotTransform.GetComponent<Slot>();
            if (slot != null && slot.currentItem == null)
            {
                GameObject newItem = Instantiate(itemPrefab, slotTransform);
                newItem.GetComponent<RectTransform>().anchoredPosition= Vector2.zero;
                slot.currentItem = newItem;
                RebuildItemCounts();
                return true;    
            }
            
        }
        return false;
    }

    public void RemoveItemsFromInventory(int itemID, int amountToRemove)
    {
        Debug.Log($"[INVENTÁRIO] A tentar remover o item com ID: {itemID}. Quantidade pedida: {amountToRemove}");
        bool itemEncontrado = false;

        foreach (Transform slotTransform in inventoryPanel.transform)
        {
            if (amountToRemove <= 0) break;

            Slot slot = slotTransform.GetComponent<Slot>();
            if (slot == null || slot.currentItem == null) continue;

            Item item = slot.currentItem.GetComponent<Item>();
            if (item != null)
            {
                // Este log vai-nos dizer TODOS os itens que tens na mala e os seus IDs reais!
                Debug.Log($"[INVENTÁRIO] Encontrei um item no slot. Nome do Objeto: {slot.currentItem.name} | ID deste item: {item.ID} | Quantidade no slot: {item.quantity}");

                if (item.ID == itemID)
                {
                    itemEncontrado = true;
                    int removed = Mathf.Min(amountToRemove, item.quantity);

                    item.quantity -= removed;
                    amountToRemove -= removed;

                    Debug.Log($"[INVENTÁRIO] ID Correto! A remover {removed} unidades. Restam na mala: {item.quantity}");

                    if (item.quantity <= 0)
                    {
                        Debug.Log("[INVENTÁRIO] Quantidade chegou a 0. A destruir o objeto visual do slot!");
                        Destroy(slot.currentItem);
                        slot.currentItem = null;
                    }
                }
            }
        }

        if (!itemEncontrado)
        {
            Debug.LogError($"[INVENTÁRIO ERRO] O diálogo pediu para apagar o ID {itemID}, mas NÃO EXISTE nenhum item com esse ID exato dentro da mala!");
        }

        RebuildItemCounts();
    }

}