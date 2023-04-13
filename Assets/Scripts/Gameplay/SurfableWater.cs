using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;
public class SurfableWater : MonoBehaviour, Interactable
{
    public IEnumerator Interact(Transform initiaitor)
    {
        yield return DialogManager.instance.ShowDialogText($"The water is deep blue!");
        // finding a creature that has surf in his moves
        var creature = initiaitor.GetComponent<CreaturesParty>().creatures.FirstOrDefault(p => p.moves.Any(m => m.base_.name == "Surf"));
        if (creature != null)
        {
            int selectedChoice = 0;
            yield return DialogManager.instance.ShowDialogText($"Should {creature._base.name} use Surf?",
             choices: new List<string>() { "Yes", "No" },
              onChoiceSelected: (selection) => selectedChoice = selection);

            if (selectedChoice == 0)
            {
                // yes
                yield return DialogManager.instance.ShowDialogText($"{creature._base.name} used Surf!");
                var animator = initiaitor.GetComponent<CharacterAnimator>();
                var dir = new Vector3(animator.moveX, animator.moveY);
                var targetPos = initiaitor.position + dir;

                yield return initiaitor.DOJump(targetPos, 0.3f, 1, 0.5f).WaitForCompletion();
                animator.isSurfing = true;
            }
        }
    }
}
