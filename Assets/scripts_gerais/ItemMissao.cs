using UnityEngine;

public class ItemMissao : Item
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log($"[ITEM MISSÃO] A colher item com ID: {this.ID}");

            // 1. ADICIONAR AO INVENTÁRIO
            // Procura o componente de inventário que está no teu Player
            // Nota: Se o teu script de inventário tiver outro nome (ex: Inventario, PlayerInventory), altera aqui!
            var inventario = collision.GetComponent<PlayerItemCollector>();
            if (inventario == null)
            {
                inventario = Object.FindAnyObjectByType<PlayerItemCollector>();
            }

            // Se o teu inventário for gerido pelo ShopController ou ItemDictionary, usamos o ID dele:
            if (inventario != null)
            {
                // Aqui chamamos a função do teu jogo que adiciona itens (ex: AdicionarItem, CollectItem)
                // Se souberes o nome exato da função do teu inventário, substitui abaixo:
                Debug.Log("Inventário encontrado! A enviar o item para a mala.");
            }

            // 2. ATUALIZAR O SISTEMA DE MISSÕES
            if (Missoes.Instance != null)
            {
                Missoes.Instance.CompletarMissaoDireto("Recuperar Diário");

                // "Diario" é o ID do objetivo no teu ScriptableObject da Quest
                Missoes.Instance.AvancarObjetivoItem("Diario", 1);
            }

            // 3. LIBERTAR O CAMINHO DA MAIA
            Player p = collision.GetComponent<Player>();
            if (p != null)
            {
                p.PodePassar = true;
            }

            // Desaparece do chão
            Destroy(gameObject);
        }
    }
}