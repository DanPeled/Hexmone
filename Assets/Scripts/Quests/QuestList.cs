using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class QuestList : MonoBehaviour, ISavable
{
    List<Quest> quests = new List<Quest>();
    public event Action OnUpdated;

    public void AddQuest(Quest quest)
    {
        if (!quests.Contains(quest))
            quests.Add(quest);

        OnUpdated?.Invoke();
    }

    public static QuestList GetQuestList()
    {
        return FindObjectOfType<QuestList>();
    }

    public bool IsStarted(string questName)
    {
        var questStatus = quests.FirstOrDefault(p => p.Base.name == questName)?.status;
        return questStatus == QuestStatus.Started || questStatus == QuestStatus.Completed;
    }

    public bool IsCompleted(string questName)
    {
        var questStatus = quests.FirstOrDefault(p => p.Base.name == questName)?.status;
        return questStatus == QuestStatus.Completed;
    }

    public object CaptureState()
    {
        return quests.Select(q => q.GetSaveData()).ToList();
    }

    public void RestoreState(object state)
    {
        var saveData = state as List<QuestSaveData>;
        if (saveData != null)
        {
            quests = saveData.Select(q => new Quest(q)).ToList();
            OnUpdated?.Invoke();
        }
    }
}
