using UnityEngine;

public class StoryItem : MonoBehaviour, IPlayerTriggerable
{
    public Dialog dialog;
    public void OnPlayerTriggered(Player player){
        player.isMoving = false;
        player.playerActive = false;
        StartCoroutine(DialogManager.instance.ShowDialog(dialog));
    }
}