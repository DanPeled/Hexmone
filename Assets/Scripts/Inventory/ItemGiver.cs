using System.Collections;
using UnityEngine;

public class ItemGiver : MonoBehaviour, ISavable
{
    public ItemBase item;
    public int count = 1;
    public Dialog dialog;


    bool used = false;

    public IEnumerator GiveItem(Player player)
    {
        yield return DialogManager.instance.ShowDialog(dialog);

        player.GetComponent<Inventory>().AddItem(item, count);

        used = true;
        string dialogText = $"{player.playerName} recived {item.name}";
        if (count > 1)
        {
            dialogText = $"{player.playerName} recived {count} {item.name}s";
        }
        yield return DialogManager.instance.ShowDialogText(dialogText);
    }
    public bool CanBeGiven()
    {
        return item != null && count > 0 && !used;
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