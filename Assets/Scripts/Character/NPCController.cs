using System.Collections;
using UnityEngine;
using System.Collections.Generic;
public class NPCController : MonoBehaviour, Interactable, ISavable
{
    [Header("Dialog")]
    public string dialog;

    [Header("Quests")]
    public QuestBase questToStart, questToComplete;

    [Header("Movement")]
    public List<Sprite> sprites;
    public List<Vector2> movementPattern;
    public float timeBetweenPatterns;
    NPCState state;
    float idleTimer = 0f;
    public Character character;
    ItemGiver itemGiver;
    Healer healer;
    CreatureGiver creatureGiver;
    SpriteAnimator spriteAnimator;
    Merchant merchant;
    Quest activeQuest;

    void Start()
    {
        spriteAnimator = new SpriteAnimator(sprites, GetComponentInChildren<SpriteRenderer>());
        character = GetComponent<Character>();
        itemGiver = GetComponent<ItemGiver>();
        spriteAnimator.Start();
        creatureGiver = GetComponent<CreatureGiver>();
        healer = GetComponent<Healer>();
        merchant = GetComponent<Merchant>();
    }
    void Update()
    {
        if (state == NPCState.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > 2f)
            {
                idleTimer = 0f;
                if (movementPattern.Count > 0)
                {
                    spriteAnimator.HandleUpdate();
                    var pattern = movementPattern[Random.Range(0, movementPattern.Count)];
                    state = NPCState.Walking;
                }
            }
        }

    }
    public IEnumerator Interact(Transform initiator = null)
    {
        idleTimer = 0f;
        state = NPCState.Idle;
        if (questToComplete != null)
        {
            var quest = new Quest(questToComplete);
            yield return quest.CompleteQuest();
            questToComplete = null;
        }
        if (itemGiver != null && itemGiver.CanBeGiven())
        {
            yield return itemGiver.GiveItem(initiator.GetComponent<Player>());
        }
        else if (creatureGiver != null && creatureGiver.CanBeGiven())
        {
            yield return creatureGiver.GiveCreature(initiator.GetComponent<Player>());
        }
        else if (questToStart != null)
        {
            activeQuest = new Quest(questToStart);
            yield return activeQuest.StartQuest();
            questToStart = null;

            if (activeQuest.CanBeCompleted())
            {
                yield return activeQuest.CanBeCompleted();
                activeQuest = null;
            }
        }
        else if (activeQuest != null)
        {
            if (activeQuest.CanBeCompleted())
            {
                yield return activeQuest.CanBeCompleted();
                activeQuest = null;
            }
            else
            {
                yield return DialogManager.instance.ShowDialog(activeQuest.Base.InProgressDialog);
            }
        }
        else if (healer != null)
        {
            yield return healer.Heal(initiator, dialog);
        } else if (merchant != null){
            yield return merchant.Trade();
        }
        else
        {
            if (CreaturesParty.GetPlayerParty().GetHealthyCreature() == null) yield break;
            yield return (DialogManager.instance.ShowDialogText(dialog, autoClose: false));

        }
    }
    public object CaptureState()
    {
        var saveData = new NPCQuestSaveData();
        saveData.activeQuest = activeQuest?.GetSaveData();
        if (questToStart != null)
            saveData.questToStart = (new Quest(questToStart)).GetSaveData();
        if (questToComplete != null)
            saveData.questToComplete = (new Quest(questToComplete)).GetSaveData();
        return saveData;
    }
    public void RestoreState(object state)
    {
        var saveData = state as NPCQuestSaveData;
        if (saveData != null)
        {
            activeQuest = (saveData.activeQuest != null) ? new Quest(saveData.activeQuest) : null;
            questToStart = (saveData.questToStart != null) ? new Quest(saveData.questToStart).Base : null;
            questToComplete = (saveData.questToComplete != null) ? new Quest(saveData.questToComplete).Base : null;
        }
    }
}
[System.Serializable]
public class NPCQuestSaveData
{
    public QuestSaveData activeQuest, questToStart, questToComplete;
}
public enum NPCState
{
    Idle, Walking
}
