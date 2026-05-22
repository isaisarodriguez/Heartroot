using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NovaMissaoSO", menuName = "Quests/Quest")]
public class Quest : ScriptableObject
{
    public string questID;
    public string questName;
    [TextArea] public string description;

    public List<QuestObjective> objectives = new List<QuestObjective>();

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(questID))
        {
            questID = questName + "_" + System.Guid.NewGuid().ToString();
        }
    }
}

// --- CLASSES DE SUPORTE (Minuto 03:13 até 07:26) ---

public enum ObjectiveType
{
    CollectItem,
    DefeatEnemy,
    ReachLocation,
    TalkToNPC,
    Custom
}

[System.Serializable]
public class QuestObjective
{
    public string objectiveID;
    public string description;
    public ObjectiveType type;
    public int requiredAmount;
    public int currentAmount;

    public bool isCompleted => currentAmount >= requiredAmount;
}

// NOVO: Classe adicionada pelo rapaz no minuto 05:06
[System.Serializable]
public class QuestProgress
{
    public Quest quest;
    public List<QuestObjective> objectives;

    // Atalho para não termos de escrever "quest.questID" a toda a hora (Minuto 06:58)
    public string questID => quest.questID;

    // O Construtor: Cria a tal cópia profunda para não estragar o ficheiro original (Minuto 05:19)
    public QuestProgress(Quest acceptedQuest)
    {
        quest = acceptedQuest;
        objectives = new List<QuestObjective>();

        // Percorre os objetivos do molde original e duplica-os do zero (Minuto 06:03, 07:15)
        foreach (var ob in quest.objectives)
        {
            QuestObjective newObj = new QuestObjective
            {
                objectiveID = ob.objectiveID,
                description = ob.description,
                type = ob.type,
                requiredAmount = ob.requiredAmount,
                currentAmount = 0 // Começa sempre a zero no início da missão (Minuto 06:34)
            };
            objectives.Add(newObj);
        }
    }

    // Devolve verdadeiro se todos os objetivos desta cópia forem cumpridos (Minuto 06:39)
    public bool IsCompleted()
    {
        return objectives.TrueForAll(o => o.isCompleted);
    }
}
