using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Healer : MonoBehaviour
{
    public IEnumerator Heal(Transform playerTransform, Dialog dialog)
    {
        int selectedChoice = 0;
        yield return DialogManager.instance.ShowDialogText("You look tired! Would you like to rest here?", choices: new List<string>() { "Yes", "No" }, onChoiceSelected: (choiceIndex) => selectedChoice = choiceIndex);
        var playerParty = playerTransform.GetComponent<CreaturesParty>();
        if (selectedChoice == 0)
        {
            // yes
            playerParty.creatures.ForEach(c => c.Heal());
            playerParty.PartyUpdated();
            yield return DialogManager.instance.ShowDialogText($"Your creatures should be fully healed now");

        }
        else if (selectedChoice == 1)
        {
            // no
            yield return DialogManager.instance.ShowDialogText($"Okay! Come back if you change your mind");
        }

    }
}