using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene : MonoBehaviour, IPlayerTriggerable
{
    [SerializeReference]
    public List<CutSceneAction> actions;

    public IEnumerator Play()
    {
        GameController.instance.StartCutsceneState();
        foreach (var action in actions)
        {
            if (action.waitForCompletion)
                yield return action.Play();
            else
                StartCoroutine(action.Play());
        }
        GameController.instance.StartFreeRoamState();
    }

    public void AddAction(CutSceneAction action)
    {
        action.name = action.GetType().ToString();
        actions.Add(action);
    }

    public bool TriggerRepeatdly => false;

    public void OnPlayerTriggered(Player player)
    {
        player.isMoving = false;
        StartCoroutine(Play());
    }
}
