using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class CutableTree : MonoBehaviour, Interactable
{
    public IEnumerator Interact(Transform initiaitor = null)
    {
        yield return DialogManager.instance.ShowDialogText($"This tree looks like it can be cut");

        var creatureWithCut = initiaitor.GetComponent<CreaturesParty>().creatures.FirstOrDefault(p => p.moves.Any(m => m.base_.name == "Cut"));
        if (creatureWithCut != null)
        {
            int selectedChoice = 0;
            yield return DialogManager.instance.ShowDialogText($"Should {creatureWithCut._base.name} use cut?",
             choices: new List<string>() { "Yes", "No" },
              onChoiceSelected: (selection) => selectedChoice = selection);

            if (selectedChoice == 0)
            {
                // yes
                yield return DialogManager.instance.ShowDialogText($"{creatureWithCut._base.name} used Cut!");

                gameObject.SetActive(false);
            }
        }
    }
}
