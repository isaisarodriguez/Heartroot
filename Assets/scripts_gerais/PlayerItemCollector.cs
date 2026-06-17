using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    private InventoryController inventoryController;

    void Start()
    {
        inventoryController = FindFirstObjectByType<InventoryController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Item"))
        {
            Item item = collision.GetComponent<Item>();
            if (item != null)
            {
                // 1. Tenta adicionar o item ao inventário normalmente
                bool itemAdded = inventoryController.AddItem(collision.gameObject);
                if (itemAdded)
                {
                    Debug.Log($"[COLLECTOR] Item {item.ID} adicionado ao inventário.");

                    if ((item.ID == 13 || item.ID == 3) && Missoes.Instance != null)
                    {
                        Debug.Log("[COLLECTOR] Diário detetado! A atualizar o objetivo da missão...");
                        Missoes.Instance.CompletarMissaoDireto("Recuperar Diário");
                        Missoes.Instance.AvancarObjetivoItem("Diario", 1);
                    }

                    // 3. Liberta o caminho se o objeto tiver o script ItemMissao agarrado
                    ItemMissao itemMissao = collision.GetComponent<ItemMissao>();
                    if (itemMissao != null)
                    {
                        Player p = GetComponent<Player>();
                        if (p != null) p.PodePassar = true;
                    }

                    // Destrói o objeto do chão apenas no fim
                    Destroy(collision.gameObject);
                }
            }
        }
    }
}