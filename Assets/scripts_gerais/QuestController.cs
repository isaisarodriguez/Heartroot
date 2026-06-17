using System.Collections.Generic;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    public static QuestController Instance { get; private set; }

    // Lista de missões ativas (usada ao longo do vídeo)
    public List<QuestProgress> activateQuests = new List<QuestProgress>();

    // SOLUÇÃO: Declaração da lista pedida no início do vídeo para guardar as missões entregues!
    [HideInInspector]
    public List<string> handInQuestIds = new List<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsQuestCompleted(string questID)
    {
        QuestProgress quest = activateQuests.Find(q => q.quest.questID == questID);
        return quest != null && quest.objectives.TrueForAll(o => o.isCompleted);
    }

    // Função que o NPC usa para saber se a missão já foi entregue antes
    public bool IsQuestHandedIn(string questID)
    {
        return handInQuestIds.Contains(questID);
    }

    public void HandInQuest(string questID)
    {
        // 1. Guarda que a missão foi entregue
        if (!handInQuestIds.Contains(questID))
        {
            handInQuestIds.Add(questID);
        }

        // 2. Remove das missões ativas deste controlador
        QuestProgress quest = activateQuests.Find(q => q.quest.questID == questID);
        if (quest != null)
        {
            activateQuests.Remove(quest);
        }

        // 3. Força a remoção TOTAL da lista que a tua UI está a ler
        if (Missoes.Instance != null && Missoes.Instance.missoesAtivasSO != null)
        {
            Missoes.Instance.missoesAtivasSO.RemoveAll(q => q.quest.questID == questID);
        }

        // 4. CHAMADA IGUAL AO VÍDEO: Manda a UI atualizar-se e redesenhar o ecrã limpo!
        if (QuestUI.Instance != null)
        {
            QuestUI.Instance.UpdateQuestUI();
        }
    }
}