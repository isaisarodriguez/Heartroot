using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;

public class InteracaoArvore : MonoBehaviour
{
    [Header("Configurações do Item")]
    [SerializeField] private int idArtefato = 7; // <-- Atualizado para o ID 7!

    [Header("UI de Interação")]
    [SerializeField] private GameObject textoAvisoF; // O texto "Entregar Artefato (F)"
    [SerializeField] private GameObject painelMissaoConcluida; // O pop-up de vitória
    [SerializeField] private float tempoExibicaoPopUp = 4f;

    private bool jogadorPorPerto = false;
    private bool missaoEntregue = false;

    void Start()
    {
        if (textoAvisoF != null) textoAvisoF.SetActive(false);
        if (painelMissaoConcluida != null) painelMissaoConcluida.SetActive(false);
    }

    void Update()
    {
        // Só permite carregar no F se o jogador estiver perto, não tiver entregue, e TIVER o artefato
        if (jogadorPorPerto && !missaoEntregue && TemOArtefatoNoInventario() && Keyboard.current != null)
        {
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                EntregarArtefato();
            }
        }
    }

    // --- NOVA FUNÇÃO: Verifica se o item existe na mala ---
    private bool TemOArtefatoNoInventario()
    {
        if (InventoryController.Instance == null) return false;

        // Acede ao dicionário de contagem do inventário diretamente
        var contagemItens = InventoryController.Instance.GetItemCounts();

        // Verifica se o ID do artefato está lá dentro e se tens pelo menos 1
        return contagemItens.ContainsKey(idArtefato) && contagemItens[idArtefato] > 0;
    }

    private void EntregarArtefato()
    {
        missaoEntregue = true;

        // 1. Remove o Artefato ID 7 da mala
        InventoryController.Instance.RemoveItemsFromInventory(idArtefato, 1);
        Debug.Log($"[ÁRVORE] Artefato ID {idArtefato} entregue com sucesso!");

        // 2. Esconde o aviso de interação imediatamente
        if (textoAvisoF != null) textoAvisoF.SetActive(false);

        // 3. Mostra o grande Pop-up de Missão Concluída
        if (painelMissaoConcluida != null)
        {
            StartCoroutine(MostrarPopUpVitoria());
        }
    }

    IEnumerator MostrarPopUpVitoria()
    {
        painelMissaoConcluida.SetActive(true);
        yield return new WaitForSeconds(tempoExibicaoPopUp);
        painelMissaoConcluida.SetActive(false);
    }

    // --- DETETAR APROXIMAÇÃO ---
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Só ativa o aviso se quem entrou for o Player E ele tiver o item ID 7 na mala
        if (collision.CompareTag("Player") && !missaoEntregue)
        {
            jogadorPorPerto = true;

            if (TemOArtefatoNoInventario())
            {
                if (textoAvisoF != null) textoAvisoF.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jogadorPorPerto = false;
            if (textoAvisoF != null) textoAvisoF.SetActive(false);
        }
    }
}