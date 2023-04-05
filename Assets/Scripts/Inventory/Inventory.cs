using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class Inventory : MonoBehaviour
{

    public List<ItemSlot> slots;
    public List<ItemSlot> hexoballsSlots;
    public List<ItemSlot> tmSlots;
    public List<List<ItemSlot>> allSlots;

    public static List<string> ItemCategories { get; set; } = new List<string>() {
        "ITEMS", "HEXOBALLS", "TMs & HMs"
    };
    public event Action onUpdated;
    private void Awake()
    {
        allSlots = new List<List<ItemSlot>>() {slots, hexoballsSlots, tmSlots};
    }
    public List<ItemSlot> GetSlotsByCategory(int categoryIndex){
        return allSlots[categoryIndex];
    }
    public ItemBase UseItem(int index, Creature creature)
    {
        var item = slots[index].item;
        bool itemUsed = item.Use(creature);

        if (itemUsed)
        {
            RemoveItem(item);
            return item;
        }
        return null;
    }
    public void RemoveItem(ItemBase item)
    {
        var itemSlot = slots.First(slot => slot.item == item);
        itemSlot.count--;
        if (itemSlot.count == 0)
        {
            slots.Remove(itemSlot);
        }
        onUpdated?.Invoke();
    }
    public static Inventory GetInventory()
    {
        return FindObjectOfType<Player>().GetComponent<Inventory>();
    }

}
[System.Serializable]
public class ItemSlot
{
    public ItemBase item;
    public int count;
}