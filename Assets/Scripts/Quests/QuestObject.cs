using UnityEngine;

public class QuestObject : MonoBehaviour
{
    public QuestBase questToCheck;
    public ObjectActions onStart,
        onComplete;

    QuestList questList;

    void Start()
    {
        questList = QuestList.GetQuestList();
        questList.OnUpdated += UpdateObjectStatus;
        UpdateObjectStatus();
    }

    void OnDestroy()
    {
        questList.OnUpdated -= UpdateObjectStatus;
    }

    public void UpdateObjectStatus()
    {
        if (onStart != ObjectActions.None && questList.IsStarted(questToCheck.name))
        {
            foreach (Transform child in transform)
            {
                if (onStart == ObjectActions.Enable)
                {
                    child.gameObject.SetActive(true);
                    var savable = child.GetComponent<SavableEntity>();
                    if (savable != null)
                    {
                        SavingSystem.i.RestoreEntity(savable);
                    }
                }
                else if (onStart == ObjectActions.Disable)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
        if (onComplete != ObjectActions.None && questList.IsCompleted(questToCheck.name))
        {
            foreach (Transform child in transform)
            {
                if (onComplete == ObjectActions.Enable)
                {
                    child.gameObject.SetActive(true);
                    var savable = child.GetComponent<SavableEntity>();
                    if (savable != null)
                    {
                        SavingSystem.i.RestoreEntity(savable);
                    }
                }
                else if (onComplete == ObjectActions.Disable)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }
}

public enum ObjectActions
{
    None,
    Enable,
    Disable
}
