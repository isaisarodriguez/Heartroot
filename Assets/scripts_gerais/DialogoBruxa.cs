using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class DialogoBruxa : MonoBehaviour
{
    [Header("Configurações de UI")]
    public GameObject painelDialogo;
    public TMP_Text textoDialogo;
    public TMP_Text nomeNPCTexto;
    public Image retratoNPCImage;

    [Header("Ficheiros de Diálogo")]
    public NPCDialogue dialogoDaBruxa; // Diálogo Inicial

    private NPCDialogue dialogoAtivoMomento; // Guarda qual o diálogo a passar no momento
    private int indiceAtual;
    private bool dialogoAtivo = false;
    private bool jogadorPorPerto = false;

    void Start()
    {
        if (painelDialogo != null) painelDialogo.SetActive(false);
    }

    void Update()
    {
        // 1. INTERAÇÃO INICIAL COM A BRUXA (Carregar em F para falar)
        if (jogadorPorPerto && !dialogoAtivo && Keyboard.current != null)
        {
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                // Vê se o jogador JÁ TEM o diário para entregar
                if (Missoes.Instance != null && Missoes.Instance.TemDiario)
                {
                    Inimigo inimigoScript = GetComponent<Inimigo>();
                    if (inimigoScript != null)
                    {
                        inimigoScript.SendMessage("FinalizarMissao", SendMessageOptions.DontRequireReceiver);
                    }
                    return;
                }

                // Se a missão já foi entregue no passado, mostra o agradecimento direto
                if (QuestController.Instance != null && QuestController.Instance.IsQuestHandedIn("DiarioBruxa"))
                {
                    Inimigo inimigoScript = GetComponent<Inimigo>();
                    if (inimigoScript != null && inimigoScript.dialogoAgradecimento != null)
                    {
                        AtivarDialogo(inimigoScript.dialogoAgradecimento);
                        return;
                    }
                }

                // Se não tem o diário nem acabou a missão, mostra o diálogo inicial normal
                AtivarDialogo(dialogoDaBruxa);
                return; // Corta aqui para o mesmo clique do F não avançar a primeira frase!
            }
        }

        // 2. AVANÇAR AS FRASES COM O 'F' (Em vez do clique do rato)
        if (dialogoAtivo && Keyboard.current != null)
        {
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                AvancarFrase();
            }
        }
    }

    // --- FUNÇÃO PRINCIPAL QUE ABRE O DIÁLOGO ---
    public void AtivarDialogo(NPCDialogue ficheiroPretendido)
    {
        if (ficheiroPretendido == null || dialogoAtivo) return;

        Time.timeScale = 0f; // Pausa o jogo
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

        Time.timeScale = 1f; // Despausa o jogo
        AudioListener.pause = false;

        if (dialogoAtivoMomento == dialogoDaBruxa && dialogoDaBruxa.quest != null)
        {
            if (Missoes.Instance != null && Missoes.Instance.missoesAtivasSO.Find(q => q.quest.questID == dialogoDaBruxa.quest.questID) == null)
            {
                Missoes.Instance.AceitarMissaoSO(dialogoDaBruxa.quest);
            }
        }
    }

    // --- AQUI ESTÃO AS NOVAS FUNÇÕES QUE DETETAM A MAIA (O TRIGGER) ---
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
}