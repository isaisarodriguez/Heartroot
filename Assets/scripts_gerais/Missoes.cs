using UnityEngine;
using System.Collections.Generic;

public class Missoes : MonoBehaviour
{
    [System.Serializable]
    public class Missao
    {
        public string nome;
        public bool aceite;
        public bool completada;
    }

    public GameObject PainelMissoes;

    // --- DADOS E ESTADOS ---
    public List<Missao> missoes = new List<Missao>(); // Missões antigas
    public bool TemDiario = false;

    [Header("Sistema de Scriptable Objects (Vídeo)")]
    public List<QuestProgress> missoesAtivasSO = new List<QuestProgress>();

    // Instância estática para o QuestUI conseguir aceder facilmente às missões de qualquer lado
    public static Missoes Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (PainelMissoes != null)
            PainelMissoes.SetActive(false);
    }

    // --- GESTÃO DE MISSÕES ---

    public void AceitarMissao(string nomeMissao)
    {
        foreach (var m in missoes)
        {
            if (m.nome == nomeMissao)
            {
                m.aceite = true;
                break;
            }
        }
        NotificarUI();
    }

    public void CompletarMissaoDireto(string nomeMissao)
    {
        TemDiario = true;
        foreach (var m in missoes)
        {
            if (m.nome == nomeMissao)
            {
                m.aceite = true;
                m.completada = true;
                break;
            }
        }
        NotificarUI();
    }

    public void AceitarMissaoSO(Quest novaQuest)
    {
        if (novaQuest == null) return;

        foreach (var progresso in missoesAtivasSO)
        {
            if (progresso.questID == novaQuest.questID) return;
        }

        QuestProgress novoProgresso = new QuestProgress(novaQuest);
        missoesAtivasSO.Add(novoProgresso);

        NotificarUI();
        QuestUI ui = Object.FindAnyObjectByType<QuestUI>();
        if (ui != null)
        {
            ui.UpdateQuestUI();
        }
    }

    public void AlternarPainel()
    {
        if (PainelMissoes == null) return;

        bool estadoAtual = PainelMissoes.activeSelf;
        PainelMissoes.SetActive(!estadoAtual);

        // Se o painel foi ABERTO, congela o jogo. Se foi FECHADO, descongela.
        if (PainelMissoes.activeSelf)
        {
            Time.timeScale = 0f; // Para o jogo completamente
            NotificarUI();
        }
        else
        {
            Time.timeScale = 1f; // Devolve o jogo à velocidade normal
        }
    }

    // Avisa o script QuestUI para redesenhar o ecrã
    private void NotificarUI()
    {
        QuestUI ui = FindFirstObjectByType<QuestUI>();
        if (ui != null)
        {
            ui.UpdateQuestUI();
        }
    }
}