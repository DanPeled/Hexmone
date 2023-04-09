using System.Collections;
using UnityEngine;
public class Pickup : MonoBehaviour, Interactable, ISavable
{
    public ItemBase item;
    public bool used;
    public IEnumerator Interact(Transform initiator = null)
    {
        if (!used)
        {
            initiator.GetComponent<Inventory>().AddItem(item);
            used = true;
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
            var player = initiator.GetComponent<Player>();
            yield return DialogManager.instance.ShowDialogText($"{player.playerName } found {item.name}");
        }
    }
    public object CaptureState(){
        return used;
    }
    public void RestoreState(object state){
        used = (bool)state;

        if (used){
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }
}