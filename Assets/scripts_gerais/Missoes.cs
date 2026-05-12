using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Missoes : MonoBehaviour
{
    // --- CLASSES AUXILIARES ---
    [System.Serializable]
    public class Missao
    {
        public string nome;
        public bool aceite;
        public bool completada;
    }

    // --- REFERÊNCIAS DE UI ---
    public GameObject PainelMissoes;
    public TextMeshProUGUI TextoMissoes;

    // --- DADOS E ESTADOS ---
    public List<Missao> missoes = new List<Missao>();
    public bool TemDiario = false;

    void Start()
    {
        // Garante que o painel começa fechado
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
        AtualizarInterface();
    }

    // Função que o Diário chama para finalizar tudo de uma vez
    public void CompletarMissaoDireto(string nomeMissao)
    {
        TemDiario = true;
        foreach (var m in missoes)
        {
            if (m.nome == nomeMissao)
            {
                m.aceite = true; // Garante que aparece na lista
                m.completada = true;
                break; // Adicionado para performance
            }
        }
        AtualizarInterface();
    }

    // --- CONTROLO DO PAINEL (UI) ---

    public void AlternarPainel()
    {
        if (PainelMissoes == null) return;

        bool estadoAtual = PainelMissoes.activeSelf;
        PainelMissoes.SetActive(!estadoAtual);

        // Se abriu o painel, atualiza o texto
        if (PainelMissoes.activeSelf)
            AtualizarInterface();
    }

    void AtualizarInterface()
    {
        if (TextoMissoes == null) return;

        TextoMissoes.text = "MISSÕES\n";

        foreach (var m in missoes)
        {
            if (m.aceite)
            {
                string status = m.completada ? "[CONCLUÍDO] " : "[EM CURSO] ";
                TextoMissoes.text += status + m.nome + "\n";
            }
        }
    }
}