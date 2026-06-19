using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class DialogoBruxa : MonoBehaviour
{
    [Header("Configurações de UI")]
    public GameObject painelDialogo;
    public TMP_Text textoDialogo;
    public TMP_Text nomeNPCTexto;
    public Image retratoNPCImage;

    [Header("Pop-up de Recompensa")]
    public GameObject painelArtefatoPopUp; // Arrasta o PainelArtefato para aqui!
    public float tempoExibicaoPopUp = 3f;  // Tempo em segundos que o pop-up fica no ecrã

    [Header("Ficheiros de Diálogo")]
    public NPCDialogue dialogoDaBruxa; // Diálogo Inicial

    [Header("Recompensa da Missão")]
    public GameObject itemRecompensaPrefab; // Arrefece aqui a Prefab Variant do teu item!
    private bool recompensaEntregue = false;
    private bool deveMostrarPopUpNoFim = false; // <-- NOVO: Controla o momento de exibir o painel

    private NPCDialogue dialogoAtivoMomento;
    private int indiceAtual;
    private bool dialogoAtivo = false;
    private bool jogadorPorPerto = false;

    void Start()
    {
        if (painelDialogo != null) painelDialogo.SetActive(false);
        if (painelArtefatoPopUp != null) painelArtefatoPopUp.SetActive(false);
    }

    void Update()
    {
        // 1. INTERAÇÃO INICIAL COM A BRUXA (Carregar em F para falar)
        if (jogadorPorPerto && !dialogoAtivo && Keyboard.current != null)
        {
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                // Vamos ver se o jogador JÁ TEM o diário para entregar
                if (Missoes.Instance != null && Missoes.Instance.TemDiario)
                {
                    // 1. Remove o Diário Antigo
                    if (InventoryController.Instance != null)
                    {
                        InventoryController.Instance.RemoveItemsFromInventory(3, 1);
                        Debug.Log("[DIÁLOGO] Diário ID 3 removido do inventário.");
                    }

                    // 2. Conclui a missão no caderno roxo
                    if (QuestController.Instance != null)
                    {
                        QuestController.Instance.HandInQuest("DiarioBruxa");
                    }

                    // 3. ENTREGA DA RECOMPENSA (Guarda o item em segredo)
                    if (!recompensaEntregue && InventoryController.Instance != null && itemRecompensaPrefab != null)
                    {
                        bool conseguiuAdicionar = InventoryController.Instance.AddItem(itemRecompensaPrefab);
                        if (conseguiuAdicionar)
                        {
                            recompensaEntregue = true;
                            deveMostrarPopUpNoFim = true; // <-- AJUSTE: Avisa que vai mostrar o pop-up no fim da conversa!
                            Debug.Log($"[RECOMPENSA COMPLETA] O jogador recebeu o prémio: {itemRecompensaPrefab.name}!");
                        }
                        else
                        {
                            Debug.LogWarning("[RECOMPENSA ERRO] Inventário cheio! Não foi possível adicionar o item.");
                        }
                    }

                    // 4. Avisa o script Inimigo que a missão acabou
                    Inimigo inimigoScript = GetComponent<Inimigo>();
                    if (inimigoScript == null) inimigoScript = GetComponentInChildren<Inimigo>();

                    if (inimigoScript != null)
                    {
                        inimigoScript.Invoke("FinalizarMissao", 0f);
                    }

                    // 5. Abre o diálogo de agradecimento imediatamente
                    if (inimigoScript != null && inimigoScript.dialogoAgradecimento != null)
                    {
                        AtivarDialogo(inimigoScript.dialogoAgradecimento);
                    }
                    else
                    {
                        AtivarDialogo(dialogoDaBruxa);
                    }

                    return;
                }

                // Se a missão já foi entregue no passado, mostra o agradecimento direto
                if (QuestController.Instance != null && QuestController.Instance.IsQuestHandedIn("DiarioBruxa"))
                {
                    Inimigo inimigoScript = GetComponent<Inimigo>();
                    if (inimigoScript == null) inimigoScript = GetComponentInChildren<Inimigo>();

                    if (inimigoScript != null && inimigoScript.dialogoAgradecimento != null)
                    {
                        AtivarDialogo(inimigoScript.dialogoAgradecimento);
                        return;
                    }
                }

                // Se não tem o diário nem acabou a missão, mostra o diálogo inicial normal
                AtivarDialogo(dialogoDaBruxa);
                return;
            }
        }

        // 2. AVANÇAR AS FRASES COM O 'F'
        if (dialogoAtivo && Keyboard.current != null)
        {
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                AvancarFrase();
            }
        }
    }

    // Temporizador do Pop-up (agora usa o tempo normal do jogo)
    IEnumerator MostrarPopUpTemporario()
    {
        painelArtefatoPopUp.SetActive(true);
        yield return new WaitForSeconds(tempoExibicaoPopUp);
        painelArtefatoPopUp.SetActive(false);
    }

    public void AtivarDialogo(NPCDialogue ficheiroPretendido)
    {
        if (ficheiroPretendido == null || dialogoAtivo) return;

        Time.timeScale = 0f;
        AudioListener.pause = true;

        dialogoAtivoMomento = ficheiroPretendido;
        dialogoAtivo = true;
        indiceAtual = 0;

        if (painelDialogo != null) painelDialogo.SetActive(true);
        if (nomeNPCTexto != null) nomeNPCTexto.text = dialogoAtivoMomento.npcName;
        if (retratoNPCImage != null && dialogoAtivoMomento.npcPortrait != null)
            retratoNPCImage.sprite = dialogoAtivoMomento.npcPortrait;

        ExibirFraseAtual();
    }

    void ExibirFraseAtual()
    {
        if (textoDialogo != null && dialogoAtivoMomento != null && indiceAtual < dialogoAtivoMomento.frases.Length)
        {
            textoDialogo.text = dialogoAtivoMomento.frases[indiceAtual];
        }
    }

    public void AvancarFrase()
    {
        indiceAtual++;
        if (dialogoAtivoMomento != null && indiceAtual < dialogoAtivoMomento.frases.Length)
        {
            ExibirFraseAtual();
        }
        else
        {
            FimDoDialogo();
        }
    }

    void FimDoDialogo()
    {
        dialogoAtivo = false;
        if (painelDialogo != null) painelDialogo.SetActive(false);

        Time.timeScale = 1f;
        AudioListener.pause = false;

        // --- MUDANÇA AQUI: Ativa o pop-up assim que a caixa de diálogo fecha de vez ---
        if (deveMostrarPopUpNoFim)
        {
            deveMostrarPopUpNoFim = false; // Reseta para não repetir
            if (painelArtefatoPopUp != null)
            {
                StartCoroutine(MostrarPopUpTemporario());
            }
        }

        if (dialogoAtivoMomento == dialogoDaBruxa && dialogoDaBruxa.quest != null)
        {
            if (Missoes.Instance != null && Missoes.Instance.missoesAtivasSO.Find(q => q.quest.questID == dialogoDaBruxa.quest.questID) == null)
            {
                Missoes.Instance.AceitarMissaoSO(dialogoDaBruxa.quest);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jogadorPorPerto = true;
            Debug.Log("[INTERAÇÃO] Maia aproximou-se! Prime 'F' para falar.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jogadorPorPerto = false;
            Debug.Log("[INTERAÇÃO] Maia afastou-se.");
        }
    }

    void HandleQuestCompletion(Quest quest)
    {
        QuestController.Instance.HandInQuest(quest.questID);
    }
}