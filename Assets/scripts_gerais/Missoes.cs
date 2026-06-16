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
    public List<Missao> missoes = new List<Missao>();
    public bool TemDiario = false;

    [Header("Sistema de Scriptable Objects")]
    public List<QuestProgress> missoesAtivasSO = new List<QuestProgress>();

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

        // FORÇA O PAINEL ROXO A ABRIR AUTOMATICAMENTE (Remover se preferires que o jogador abra com uma tecla)
        if (PainelMissoes != null)
        {
            PainelMissoes.SetActive(true);
        }

        NotificarUI();
    }

    public void AlternarPainel()
    {
        if (PainelMissoes == null) return;

        bool estadoAtual = PainelMissoes.activeSelf;
        PainelMissoes.SetActive(!estadoAtual);

        if (PainelMissoes.activeSelf)
        {
            Time.timeScale = 0f;
            NotificarUI();
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    private void NotificarUI()
    {
        QuestUI ui = FindFirstObjectByType<QuestUI>();
        if (ui != null)
        {
            ui.UpdateQuestUI();
        }
    }
}