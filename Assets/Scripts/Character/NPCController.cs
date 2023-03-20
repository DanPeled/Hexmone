using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    public Dialog dialog;
    public void Interact(){
        StartCoroutine(DialogManager.instance.ShowDialog(dialog));
    }
}