using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class QuestUI : MonoBehaviour
{
    [Header("Configurações de UI (Minuto 09:05)")]
    public Transform questListContent;          // O objeto 'Content' do teu Scroll View
    public GameObject questEntryPrefab;         // O Prefab da caixinha da Missão
    public GameObject objectiveTextPrefab;      // O Prefab do texto do Objetivo

    [Header("Configurações de Teste (Minuto 09:25)")]
    public Quest testQuest;
    public int testQuestAmount = 3;

    private List<QuestProgress> testQuests = new List<QuestProgress>();

    void Start()
    {
        // Cria as missões de teste exatamente como no minuto 10:14
        if (testQuest != null)
        {
            for (int i = 0; i < testQuestAmount; i++)
            {
                testQuests.Add(new QuestProgress(testQuest));
            }
        }

        // Desenha a UI logo no início (Minuto 13:14)
        UpdateQuestUI();
    }

    // Função principal que apaga tudo e redesenha (Minuto 10:37)
    public void UpdateQuestUI()
    {
        if (questListContent == null) return;

        // 1. Destrói clones antigos para não duplicar na lista (Minuto 10:47)
        foreach (Transform child in questListContent)
        {
            Destroy(child.gameObject);
        }

        // Escolhe se usa a lista de testes ou a lista real do teu jogo
        List<QuestProgress> listaParaDesenhar = (Missoes.Instance != null && Missoes.Instance.missoesAtivasSO.Count > 0)
            ? Missoes.Instance.missoesAtivasSO
            : testQuests;

        // 2. Cria os blocos visuais para cada missão (Minuto 11:02)
        foreach (var qProgress in listaParaDesenhar)
        {
            // Cria o container da missão dentro do Content (Minuto 11:10)
            GameObject entry = Instantiate(questEntryPrefab, questListContent);

            // Encontra e define o Texto do Nome da Missão (Minuto 11:20 a 11:57)
            TMP_Text questNameText = entry.transform.Find("QuestNameText")?.GetComponent<TMP_Text>();
            if (questNameText != null)
            {
                string status = qProgress.IsCompleted() ? "[CONCLUÍDO] " : "[EM CURSO] ";
                questNameText.text = status + qProgress.quest.questName;
            }

            // Encontra a sub-lista onde vão entrar os objetivos (Minuto 11:44)
            Transform objectiveList = entry.transform.Find("ObjectiveList");

            if (objectiveList != null && objectiveTextPrefab != null)
            {
                // Cria cada objetivo individual dentro da missão (Minuto 12:05)
                foreach (var obj in qProgress.objectives)
                {
                    GameObject objTextGo = Instantiate(objectiveTextPrefab, objectiveList);
                    TMP_Text objText = objTextGo.GetComponent<TMP_Text>();

                    if (objText != null)
                    {
                        string statusObj = obj.isCompleted ? "✓ " : "○ ";
                        // Formato idêntico ao do vídeo: "○ Descrição (0/5)" (Minuto 12:34)
                        objText.text = $"{statusObj}{obj.description} ({obj.currentAmount}/{obj.requiredAmount})";
                    }
                }
            }
        }
    }
}
