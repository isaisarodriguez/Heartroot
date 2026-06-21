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

    [Header("Indicador Visual com Fade-In (ATUALIZADO)")]
    // Mudado para CanvasGroup para podermos fazer o efeito de transparência
    public CanvasGroup indicadorAvancar;
    public float velocidadeFadeIn = 2f; // Velocidade do efeito (maior = mais rápido)

    [Header("Pop-up de Recompensa")]
    public GameObject painelArtefatoPopUp; // Arrasta o PainelArtefato para aqui!
    public float tempoExibicaoPopUp = 3f;  // Tempo em segundos que o pop-up fica no ecrã

    [Header("Ficheiros de Diálogo")]
    public NPCDialogue dialogoDaBruxa; // Diálogo Inicial

    [Header("Recompensa da Missão")]
    public GameObject itemRecompensaPrefab; // Arrefece aqui a Prefab Variant do teu item!
    private bool recompensaEntregue = false;
    private bool deveMostrarPopUpNoFim = false; // Controla o momento de exibir o painel

    private NPCDialogue dialogoAtivoMomento;
    private int indiceAtual;
    private bool dialogoAtivo = false;
    private bool jogadorPorPerto = false;

    private Coroutine cronometroIndicador;
    private Coroutine animacaoFadeIn; // Guarda a animação ativa para podermos pará-la se necessário

    void Start()
    {
        if (painelDialogo != null) painelDialogo.SetActive(false);
        if (painelArtefatoPopUp != null) painelArtefatoPopUp.SetActive(false);

        // Garante que o indicador começa totalmente invisível
        if (indicadorAvancar != null) indicadorAvancar.alpha = 0f;
    }

    void Update()
    {
        // 1. INTERAÇÃO INICIAL COM A BRUXA (Carregar em F para falar)
        if (jogadorPorPerto && !dialogoAtivo && Keyboard.current != null)
        {
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                if (Missoes.Instance != null && Missoes.Instance.TemDiario)
                {
                    if (InventoryController.Instance != null)
                    {
                        InventoryController.Instance.RemoveItemsFromInventory(3, 1);
                        Debug.Log("[DIÁLOGO] Diário ID 3 removido do inventário.");
                    }

                    if (QuestController.Instance != null)
                    {
                        QuestController.Instance.HandInQuest("DiarioBruxa");
                    }

                    if (!recompensaEntregue && InventoryController.Instance != null && itemRecompensaPrefab != null)
                    {
                        bool conseguiuAdicionar = InventoryController.Instance.AddItem(itemRecompensaPrefab);
                        if (conseguiuAdicionar)
                        {
                            recompensaEntregue = true;
                            deveMostrarPopUpNoFim = true;
                            Debug.Log($"[RECOMPENSA COMPLETA] O jogador recebeu o prémio: {itemRecompensaPrefab.name}!");
                        }
                        else
                        {
                            Debug.LogWarning("[RECOMPENSA ERRO] Inventário cheio! Não foi possível adicionar o item.");
                        }
                    }

                    Inimigo inimigoScript = GetComponent<Inimigo>();
                    if (inimigoScript == null) inimigoScript = GetComponentInChildren<Inimigo>();

                    if (inimigoScript != null)
                    {
                        inimigoScript.Invoke("FinalizarMissao", 0f);
                    }

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

            // Reseta os cronómetros e garante que volta a ficar invisível instantaneamente
            if (cronometroIndicador != null) StopCoroutine(cronometroIndicador);
            if (animacaoFadeIn != null) StopCoroutine(animacaoFadeIn);

            if (indicadorAvancar != null) indicadorAvancar.alpha = 0f;

            cronometroIndicador = StartCoroutine(ContagemIndicadorAvancar());
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

    // Espera os 3 segundos em tempo real
    IEnumerator ContagemIndicadorAvancar()
    {
        yield return new WaitForSecondsRealtime(1f);

        // Terminado o tempo, inicia a animação do Fade-In gradual
        if (indicadorAvancar != null)
        {
            animacaoFadeIn = StartCoroutine(EfeitoFadeIn());
        }
    }

    // NOVO: Coroutine que faz o efeito suave de surgir no ecrã
    IEnumerator EfeitoFadeIn()
    {
        float alphaAtual = 0f;
        while (alphaAtual < 1f)
        {
            // Usamos Time.unscaledDeltaTime para a animação rodar suavemente mesmo com o jogo pausado
            alphaAtual += Time.unscaledDeltaTime * velocidadeFadeIn;
            indicadorAvancar.alpha = alphaAtual;
            yield return null; // Espera pelo próximo frame
        }
        indicadorAvancar.alpha = 1f; // Garante que fica totalmente visível no fim
    }

    void FimDoDialogo()
    {
        dialogoAtivo = false;
        if (painelDialogo != null) painelDialogo.SetActive(false);

        if (cronometroIndicador != null) StopCoroutine(cronometroIndicador);
        if (animacaoFadeIn != null) StopCoroutine(animacaoFadeIn);

        if (indicadorAvancar != null) indicadorAvancar.alpha = 0f;

        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (deveMostrarPopUpNoFim)
        {
            deveMostrarPopUpNoFim = false;
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