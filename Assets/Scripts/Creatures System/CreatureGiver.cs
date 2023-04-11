using System.Collections;
using UnityEngine;
public class CreatureGiver : MonoBehaviour
{
    public Creature creature;
    public Dialog dialog;


    bool used = false;

    public IEnumerator GiveCreature(Player player)
    {
        yield return DialogManager.instance.ShowDialog(dialog);

        creature.Init();
        player.GetComponent<CreaturesParty>().AddCreature(creature);

        used = true;
        string dialogText = $"{player.playerName} recived {creature._base.Name}";

        yield return DialogManager.instance.ShowDialogText(dialogText);
    }
    public bool CanBeGiven()
    {
        return creature != null && !used;
    }
    public object CaptureState()
    {
        return used;
    }
    public void RestoreState(object state)
    {
        used = (bool)state;
    }
}