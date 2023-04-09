using System.Collections;
using UnityEngine;

public class Quest
{
    public QuestBase Base { get; private set; }
    public QuestStatus status { get; private set; }
    public Quest(QuestBase _base)
    {
        this.Base = _base;
    }
    public IEnumerator StartQuest()
    {
        status = QuestStatus.Started;
        yield return DialogManager.instance.ShowDialog(Base.startDialog);
    }
    public IEnumerator CompleteQuest()
    {
        status = QuestStatus.Completed;
        yield return DialogManager.instance.ShowDialog(Base.completedDialog);
        var inv = Inventory.GetInventory();
        if (Base.requiredItem != null)
        {
            inv.RemoveItem(Base.requiredItem);
        }
        if (Base.rewardItem != null)
        {
            inv.AddItem(Base.rewardItem);
            yield return DialogManager.instance.ShowDialogText($"{Player.instance.playerName} recived {Base.rewardItem.name}");
        }
    }
    public bool CanBeCompleted()
    {
        var inv = Inventory.GetInventory();

        if (Base.requiredItem != null)
        {
            return inv.HasItem(Base.requiredItem);
        }
        return true;
    }
}
public enum QuestStatus
{
    None, Started, Completed
}