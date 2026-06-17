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

    [Header("Ficheiro de Diálogo")]
    public NPCDialogue dialogoDaBruxa;

    private int indiceAtual;
    private bool dialogoAtivo = false;
    private bool jogadorPorPerto = false;

    void Start()
    {
        if (painelDialogo != null)
            painelDialogo.SetActive(false);
    }

    void Update()
    {
        // Alterado para detetar a tecla F
        if (jogadorPorPerto && !dialogoAtivo && Keyboard.current != null)
        {
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                AtivarDialogo();
                return;
            }
        }

        if (dialogoAtivo && Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                AvancarFrase();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jogadorPorPerto = true;
            Debug.Log("[INTERAÇÃO] Carrega em 'F' para falar com a Bruxa.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            jogadorPorPerto = false;
        }
    }

    public void AtivarDialogo()
    {
        Time.timeScale = 0f;
        AudioListener.pause = true;

        if (dialogoDaBruxa == null) return;
        if (dialogoAtivo) return;

        // NOVO: Se a missão já existe no teu caderno antigo, garante que o QuestController também a conhece!
        if (Missoes.Instance != null && QuestController.Instance != null && dialogoDaBruxa.quest != null)
        {
            string idDaMissao = dialogoDaBruxa.quest.questID;

            // Se a missão está ativa no teu jogo mas não está no controlador do vídeo, sincroniza-as:
            var questNoCaderno = Missoes.Instance.missoesAtivasSO.Find(q => q.quest.questID == idDaMissao);
            var questNoController = QuestController.Instance.activateQuests.Find(q => q.quest.questID == idDaMissao);

            if (questNoCaderno != null && questNoController == null)
            {
                QuestController.Instance.activateQuests.Add(questNoCaderno);
            }
        }

        dialogoAtivo = true;
        indiceAtual = 0;

        if (painelDialogo != null) painelDialogo.SetActive(true);

        if (nomeNPCTexto != null) nomeNPCTexto.text = dialogoDaBruxa.npcName;
        if (retratoNPCImage != null && dialogoDaBruxa.npcPortrait != null) retratoNPCImage.sprite = dialogoDaBruxa.npcPortrait;

        ExibirFraseAtual();
    }

    void FimDoDialogo()
    {
        dialogoAtivo = false;
        if (painelDialogo != null) painelDialogo.SetActive(false);

        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (dialogoDaBruxa != null && dialogoDaBruxa.quest != null && QuestController.Instance != null)
        {
            string idDaMissao = dialogoDaBruxa.quest.questID;

            // 1. Verifica se a missão já foi entregue antes para não repetir
            if (QuestController.Instance.IsQuestHandedIn(idDaMissao))
            {
                Debug.Log("[NPC] Esta missão já foi entregue anteriormente.");
                return;
            }

            // 2. Procura a missão no teu caderno para ver se os objetivos estão feitos
            bool missaoConcluidaNoCaderno = false;
            if (Missoes.Instance != null)
            {
                var questNoCaderno = Missoes.Instance.missoesAtivasSO.Find(q => q.quest.questID == idDaMissao);

                // Se a missão existe e a função IsCompleted() dela der true, ou se o TrueForAll der true
                if (questNoCaderno != null)
                {
                    missaoConcluidaNoCaderno = questNoCaderno.IsCompleted() || questNoCaderno.objectives.TrueForAll(o => o.isCompleted);
                }
            }

            // 3. Força a entrega se o caderno disser que está pronta ou se o QuestController achar que sim
            if (missaoConcluidaNoCaderno || QuestController.Instance.IsQuestCompleted(idDaMissao))
            {
                HandleQuestCompletion(dialogoDaBruxa.quest);
            }
            else
            {
                // Se o jogador ainda não tinha a missão, aceita-a agora
                if (Missoes.Instance != null && Missoes.Instance.missoesAtivasSO.Find(q => q.quest.questID == idDaMissao) == null)
                {
                    Missoes.Instance.AceitarMissaoSO(dialogoDaBruxa.quest);
                }
            }
        }
    }

    private void HandleQuestCompletion(Quest quest)
    {
        Debug.Log($"[NPC] A entregar a missão: {quest.questName}");
        if (QuestController.Instance != null)
        {
            QuestController.Instance.HandInQuest(quest.questID);
        }
    }

    void ExibirFraseAtual()
    {
        if (textoDialogo != null && dialogoDaBruxa != null && indiceAtual < dialogoDaBruxa.frases.Length)
        {
            textoDialogo.text = dialogoDaBruxa.frases[indiceAtual];
        }
    }

    public void AvancarFrase()
    {
        indiceAtual++;

        if (dialogoDaBruxa != null && indiceAtual < dialogoDaBruxa.frases.Length)
        {
            ExibirFraseAtual();
        }
        else
        {
            FimDoDialogo();
        }
    }
}