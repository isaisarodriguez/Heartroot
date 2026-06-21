using UnityEngine;
using UnityEngine.Video; // IMPORTANTE: Necessário para controlar vídeos
using UnityEngine.InputSystem;

public class MenuPrincipal : MonoBehaviour
{
    [Header("Painéis de UI")]
    [SerializeField] private GameObject telaInicialPanel;

    [Tooltip("Arrasta aqui o objeto ou grupo que contém a tua UI de jogo (vida, hora, etc.) para esconder durante o vídeo.")]
    [SerializeField] private GameObject hudGameplay;

    [Header("Configurações da Cutscene")]
    [Tooltip("Arrasta aqui o objeto que tem o componente Video Player.")]
    [SerializeField] private VideoPlayer cutscenePlayer;

    [Tooltip("Um texto opcional a dizer 'Pressiona Espaço para saltar' (opcional).")]
    [SerializeField] private GameObject avisoPularCutscene;

    [Header("Música de Fundo (NOVO)")]
    [Tooltip("Arrasta aqui o objeto que vai tocar a música de fundo.")]
    [SerializeField] private AudioSource musicaFundo;

    private bool cutsceneAJogar = false;

    void Start()
    {
        // 1. Garante que o menu começa visível
        if (telaInicialPanel != null)
        {
            telaInicialPanel.SetActive(true);
        }

        // 2. Garante que o vídeo, aviso e HUD do jogo começam desligados
        if (cutscenePlayer != null)
        {
            cutscenePlayer.gameObject.SetActive(false);
        }

        if (avisoPularCutscene != null)
        {
            avisoPularCutscene.SetActive(false);
        }

        if (hudGameplay != null)
        {
            hudGameplay.SetActive(false);
        }

        // 3. Começa a tocar a música do menu logo no início
        if (musicaFundo != null)
        {
            musicaFundo.Play();
        }

        // 4. Pausa o jogo por trás para o jogo não correr no fundo
        Time.timeScale = 0f;
    }

    void Update()
    {
        // Se a cutscene estiver a dar e o jogador pressionar Espaço ou Escape, salta o vídeo!
        if (cutsceneAJogar && Keyboard.current != null)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                TerminarCutscene();
            }
        }
    }

    public void ComecarJogo()
    {
        // 1. Esconde a tela inicial
        if (telaInicialPanel != null)
        {
            telaInicialPanel.SetActive(false);
        }

        // 2. Se tiveres um vídeo configurado, inicia a cutscene!
        if (cutscenePlayer != null)
        {
            IniciarCutscene();
        }
        else
        {
            // Se não houver vídeo, inicia o jogo diretamente
            IniciarJogoDireto();
        }
    }

    private void IniciarCutscene()
    {
        cutsceneAJogar = true;
        cutscenePlayer.gameObject.SetActive(true);

        // Pausa a música de fundo para não baralhar com o áudio do vídeo
        if (musicaFundo != null)
        {
            musicaFundo.Pause();
        }

        // Ativa o aviso visual para saltar o vídeo
        if (avisoPularCutscene != null)
        {
            avisoPularCutscene.SetActive(true);
        }

        // Subscreve-se ao evento do Unity que avisa quando o vídeo chega ao fim
        cutscenePlayer.loopPointReached += AoTerminarVideo;

        // Dá Play no vídeo
        cutscenePlayer.Play();
        Debug.Log("[CUTSCENE] Vídeo iniciado.");
    }

    private void AoTerminarVideo(VideoPlayer vp)
    {
        TerminarCutscene();
    }

    private void TerminarCutscene()
    {
        // Remove a subscrição para evitar erros na memória
        if (cutscenePlayer != null)
        {
            cutscenePlayer.loopPointReached -= AoTerminarVideo;
            cutscenePlayer.Stop();
            cutscenePlayer.gameObject.SetActive(false);
        }

        if (avisoPularCutscene != null)
        {
            avisoPularCutscene.SetActive(false);
        }

        cutsceneAJogar = false;
        IniciarJogoDireto();
    }

    private void IniciarJogoDireto()
    {
        // Despausa o jogo para tudo começar a funcionar!
        Time.timeScale = 1f;

        // Ativa os elementos visuais do jogo (Vida, Hora, Missões...) agora que o jogo começou!
        if (hudGameplay != null)
        {
            hudGameplay.SetActive(true);
        }

        // Volta a tocar a música de fundo (agora para o jogo em si!)
        if (musicaFundo != null)
        {
            musicaFundo.Play();
        }

        Debug.Log("[MENU] Cutscene concluída. Jogo iniciado!");
    }

    public void FecharJogo()
    {
        Debug.Log("[MENU] O jogador fechou o jogo!");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}