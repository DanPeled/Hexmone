using UnityEngine;
[CreateAssetMenu(menuName = "Quests/Create a new quest")]
public class QuestBase : ScriptableObject
{
    public string name, description;
    public Dialog startDialog, inProgressDialog, completedDialog;

    public ItemBase requiredItem, rewardItem;

    public Dialog InProgressDialog => inProgressDialog?.lines.Count > 0 ? inProgressDialog : startDialog;
}