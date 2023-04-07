using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public enum ItemCategory
{
    Items, Hexoballs, Tms
}
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
        allSlots = new List<List<ItemSlot>>() { slots, hexoballsSlots, tmSlots };
    }
    public List<ItemSlot> GetSlotsByCategory(int categoryIndex)
    {
        return allSlots[categoryIndex];
    }
    public ItemBase GetItem(int itemIndex, int categoryIndex)
    {
        var currentSlots = GetSlotsByCategory(categoryIndex);
        return currentSlots[itemIndex].item;
    }
    public ItemBase UseItem(int itemIndex, Creature creature, int selectedCategory)
    {

        var item = GetItem(itemIndex, selectedCategory);
        bool itemUsed = item.Use(creature);

        if (itemUsed)
        {
            if (!item.isReusable)
            {
                RemoveItem(item, selectedCategory);
            }
            return item;
        }
        return null;
    }
    public void RemoveItem(ItemBase item, int selectedCategory)
    {
        var currentSlots = GetSlotsByCategory(selectedCategory);
        var itemSlot = currentSlots.First(slot => slot.item == item);
        itemSlot.count--;
        if (itemSlot.count == 0)
        {
            currentSlots.Remove(itemSlot);
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