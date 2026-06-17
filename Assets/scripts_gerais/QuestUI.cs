using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class QuestUI : MonoBehaviour
{
    // NOVO: Adicionado o Singleton para o QuestController o conseguir chamar (como no vídeo!)
    public static QuestUI Instance { get; private set; }

    [Header("Configurações de UI (Minuto 09:05)")]
    public Transform questListContent;
    public GameObject questEntryPrefab;
    public GameObject objectiveTextPrefab;

    [Header("Configurações de Teste (Minuto 09:25)")]
    public Quest testQuest;
    public int testQuestAmount;

    private List<QuestProgress> testQuests = new();

    void Awake()
    {
        // Configura o Singleton
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (testQuest != null)
        {
            for (int i = 0; i < testQuestAmount; i++)
            {
                testQuests.Add(new QuestProgress(testQuest));
            }
        }

        UpdateQuestUI();
    }

    public void UpdateQuestUI()
    {
        if (questListContent == null) return;

        foreach (Transform child in questListContent)
        {
            Destroy(child.gameObject);
        }

        List<QuestProgress> listaParaDesenhar = (Missoes.Instance != null)
            ? Missoes.Instance.missoesAtivasSO
            : testQuests;

        foreach (var qProgress in listaParaDesenhar)
        {
            GameObject entry = Instantiate(questEntryPrefab, questListContent);

            TMP_Text questNameText = entry.transform.Find("QuestNameText")?.GetComponent<TMP_Text>();
            if (questNameText != null)
            {
                // Usa a função IsCompleted() correta
                string status = qProgress.IsCompleted() ? "[CONCLUÍDO] " : "[EM CURSO] ";
                questNameText.text = status + qProgress.quest.questName;
            }

            Transform objectiveList = entry.transform.Find("ObjectiveList");

            if (objectiveList != null && objectiveTextPrefab != null)
            {
                foreach (var obj in qProgress.objectives)
                {
                    GameObject objTextGo = Instantiate(objectiveTextPrefab, objectiveList);
                    TMP_Text objText = objTextGo.GetComponent<TMP_Text>();

                    if (objText != null)
                    {
                        string statusObj = obj.isCompleted ? "[X] " : "[ ] ";
                        objText.text = $"{statusObj}{obj.description} ({obj.currentAmount}/{obj.requiredAmount})";
                    }
                }
            }
        }
    }
}