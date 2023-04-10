using System.Collections;
using UnityEngine;

public class Healer : MonoBehaviour
{
    public IEnumerator Heal(Transform playerTransform, Dialog dialog)
    {
        yield return DialogManager.instance.ShowDialog(dialog);
        var playerParty = playerTransform.GetComponent<CreaturesParty>();
        playerParty.creatures.ForEach(c => c.Heal());
        playerParty.PartyUpdated();
    }
}