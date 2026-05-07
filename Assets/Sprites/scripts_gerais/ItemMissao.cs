using UnityEngine;

public class ItemMissao : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Verifica se quem tocou no item foi o Jogador
        if (collision.CompareTag("Player"))
        {
            Debug.Log("DIÁRIO RECOLHIDO! Missão concluída e caminho livre.");

            // 1. ATUALIZAÇÃO DO SISTEMA DE MISSÕES
            // Procura o gestor de missões na cena
            Missoes M = Object.FindAnyObjectByType<Missoes>();
            if (M != null)
            {
                // Completa a missão específica e marca que a Maia já tem o diário
                M.CompletarMissaoDireto("Recuperar Diário");
            }

            // 2. LIBERTAÇÃO DO MOVIMENTO
            // Acede ao script Player que está no objeto que colidiu
            Player p = collision.GetComponent<Player>();
            if (p != null)
            {
                // Desativa a "parede invisível" que bloqueava a Maia
                p.PodePassar = true;
            }

            // 3. REMOÇÃO DO ITEM
            // O diário desaparece do mapa pois já foi recolhido
            Destroy(gameObject);
        }
    }
}