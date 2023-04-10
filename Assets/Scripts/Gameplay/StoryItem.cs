using UnityEngine;
using System.Linq;
public class StoryItem : MonoBehaviour, IPlayerTriggerable
{
    public Dialog dialog;
    public void OnPlayerTriggered(Player player){
        player.isMoving = false;
        player.playerActive = false;
        this.GetComponent<BoxCollider2D>().enabled = false;
        StartCoroutine(DialogManager.instance.ShowDialog(dialog));
    }
    void OnTriggerExit2D(Collider2D other)
    {
    }
}