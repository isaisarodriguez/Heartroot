using System.Collections.Generic;
using UnityEngine;

public class QuestController : MonoBehaviour
{
    public static QuestController Instance { get; private set; }

    // Lista de missões ativas (mantida do teu projeto)
    public List<QuestProgress> activateQuests = new List<QuestProgress>();

    // Lista exatamente igual ao minuto 00:51 do vídeo para guardar as missões entregues
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

    // Minuto 01:11 - Função que o NPC usa para saber se a missão está pronta
    public bool IsQuestCompleted(string questID)
    {
        QuestProgress quest = activateQuests.Find(q => q.quest.questID == questID);
        return quest != null && quest.objectives.TrueForAll(o => o.isCompleted);
    }

    // Minuto 02:05 - Função principal de entrega da missão
    public void HandInQuest(string questID)
    {
        if (!removeRequiredItemsFromInventory(questID))
        {
            return; // Se não conseguiu remover os itens, para a execução aqui
        }

        // Minuto 09:20 - Guarda o ID na lista de missões finalizadas antes de remover
        if (!handInQuestIds.Contains(questID))
        {
            handInQuestIds.Add(questID);
        }

        // Minuto 08:48 - Remove a missão das missões ativas
        QuestProgress quest = activateQuests.Find(q => q.quest.questID == questID);
        if (quest != null)
        {
            activateQuests.Remove(quest);
        }

        // Extra de Sincronização: Limpa também o teu scriptable object antigo para não duplicar na tela
        if (Missoes.Instance != null && Missoes.Instance.missoesAtivasSO != null)
        {
            Missoes.Instance.missoesAtivasSO.RemoveAll(q => q.quest.questID == questID);
        }

        // Minuto 09:07 - Atualiza a Interface Visual
        if (QuestUI.Instance != null)
        {
            QuestUI.Instance.UpdateQuestUI();
        }
    }

    // Minuto 02:21 - Valida e recolhe os itens necessários da mala do jogador
    public bool removeRequiredItemsFromInventory(string questID)
    {
        QuestProgress quest = activateQuests.Find(q => q.quest.questID == questID);
        if (quest == null) return false;

        // Cria o dicionário temporário para os IDs e quantidades dos itens
        Dictionary<int, int> requiredItems = new Dictionary<int, int>();

        foreach (QuestObjective objective in quest.objectives)
        {
            // Substitui o ObjectiveType pelo teu do projeto (ex: CollectItem)
            if (objective.type == ObjectiveType.CollectItem)
            {
                if (int.TryParse(objective.objectiveID, out int itemID))
                {
                    requiredItems[itemID] = objective.requiredAmount;
                }
            }
        }

        // Minuto 04:01 - Verifica as quantidades reais no teu InventoryController
        if (InventoryController.Instance != null)
        {
            Dictionary<int, int> itemCounts = InventoryController.Instance.GetItemCounts();

            // Valida se o jogador tem a quantidade exigida
            foreach (var item in requiredItems)
            {
                int currentAmount = itemCounts.ContainsKey(item.Key) ? itemCounts[item.Key] : 0;
                if (currentAmount < item.Value)
                {
                    return false; // Falta item, cancela a entrega
                }
            }

            // Minuto 07:47 - Se passou no teste, remove fisicamente os itens do inventário
            foreach (var itemRequirement in requiredItems)
            {
                InventoryController.Instance.RemoveItemsFromInventory(itemRequirement.Key, itemRequirement.Value);
            }
        }

        return true;
    }

    // Minuto 09:33 - Função que o NPC usa para saber se a missão já foi entregue no passado
    public bool IsQuestHandedIn(string questID)
    {
        return handInQuestIds.Contains(questID);
    }
}