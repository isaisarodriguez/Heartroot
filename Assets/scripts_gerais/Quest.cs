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

[System.Serializable]
public class QuestProgress
{
    public Quest quest;
    public List<QuestObjective> objectives;

    public string questID => quest.questID;

    public QuestProgress(Quest acceptedQuest)
    {
        quest = acceptedQuest;
        objectives = new List<QuestObjective>();

        foreach (var obj in quest.objectives)
        {
            QuestObjective newObj = new QuestObjective
            {
                objectiveID = obj.objectiveID,
                description = obj.description,
                type = obj.type,
                requiredAmount = obj.requiredAmount,
                currentAmount = 0
            };
            objectives.Add(newObj);
        }
    }

    public bool IsCompleted()
    {
        return objectives.TrueForAll(o => o.isCompleted);
    }
}
