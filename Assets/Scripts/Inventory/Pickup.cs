using System.Collections;
using UnityEngine;
public class Pickup : MonoBehaviour, Interactable{
    public ItemBase item;
    public bool used;
    public IEnumerator Interact(Transform initiator=null){
        if (!used){
            initiator.GetComponent<Inventory>().AddItem(item);
            used = true;
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
            
            yield return DialogManager.instance.ShowDialogText($"Player found {item.name}");
        }
    }
}