using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{

    public List<ItemSlot> slots;
    public static Inventory GetInventory(){
        return FindObjectOfType<Player>().GetComponent<Inventory>();
    }

}
[System.Serializable]
public class ItemSlot {
    public ItemBase item;
    public int count;
}