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

    void Start()
    {
        if (painelDialogo != null)
            painelDialogo.SetActive(false);
    }

    void Update()
    {
        // Se o diálogo estiver ativo, verifica o clique do rato (isto funciona mesmo com o jogo pausado)
        if (dialogoAtivo && Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                AvancarFrase();
            }
        }
    }

    public void AtivarDialogoPosDerrota()
    {
        // 1. Força a paragem do tempo
        Time.timeScale = 0f;

        // 2. PAUSA TODOS OS SONS DO JOGO (Corta o som do ataque a meio!)
        AudioListener.pause = true;

        if (dialogoDaBruxa == null) return;
        if (dialogoAtivo) return;

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

        // 1. VOLTA O TEMPO AO NORMAL
        Time.timeScale = 1f;

        // 2. REATIVA OS SONS DO JOGO
        AudioListener.pause = false;

        if (dialogoDaBruxa != null && dialogoDaBruxa.quest != null && Missoes.Instance != null)
        {
            Missoes.Instance.AceitarMissaoSO(dialogoDaBruxa.quest);
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