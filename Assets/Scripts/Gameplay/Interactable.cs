using System.Collections;
using UnityEngine;

public interface Interactable
{
    IEnumerator Interact(Transform initiator=null);
}