using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public enum ItemCategory
{
    Items, Hexoballs, Tms
}
public class Inventory : MonoBehaviour, ISavable
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
        if (item is HexoballItem && BattleSystem.i.isTrainerBattle)
        {
            //StartCoroutine(DialogManager.instance.ShowDialogText("This item cannot be used in a trainer battle!"));
            return null;
        }
        if (itemUsed)
        {
            if (!item.isReusable)
            {
                RemoveItem(item);
            }
            return item;
        }
        return null;
    }
    public bool HasItem(ItemBase item)
    {
        var category = (int)GetCategoryFromItem(item);

        var currentSlots = GetSlotsByCategory(category);
        return currentSlots.Exists(slot => slot.item == item);
    }
    public void RemoveItem(ItemBase item, int count = 1)
    {
        var category = (int)GetCategoryFromItem(item);

        var currentSlots = GetSlotsByCategory(category);
        var itemSlot = currentSlots.FirstOrDefault(slot => slot.item == item);
        itemSlot.count -= count;
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
    public void AddItem(ItemBase item, int count = 1)
    {
        var category = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(category);

        var itemSlot = currentSlots.FirstOrDefault(slot => slot.item == item);
        if (itemSlot != null)
        {
            itemSlot.count += count;
        }
        else
        {
            currentSlots.Add(new ItemSlot()
            {
                item = item,
                count = count
            });
        }
        onUpdated?.Invoke();

    }
    public ItemCategory GetCategoryFromItem(ItemBase item)
    {
        if (item is RecoveryItem || item is EvolutionItem)
        {
            return ItemCategory.Items;
        }
        else if (item is HexoballItem)
        {
            return ItemCategory.Hexoballs;
        }
        else return ItemCategory.Tms;
    }
    public object CaptureState()
    {
        var saveData = new InventorySaveData()
        {
            items = slots.Select(i => i.GetSaveData()).ToList(),
            hexoballs = hexoballsSlots.Select(i => i.GetSaveData()).ToList(),
            tms = tmSlots.Select(i => i.GetSaveData()).ToList(),
        };
        return saveData;
    }
    public void RestoreState(object state)
    {
        var saveData = (InventorySaveData)state;
        slots = saveData.items.Select(i => new ItemSlot(i)).ToList();
        hexoballsSlots = saveData.hexoballs.Select(i => new ItemSlot(i)).ToList();
        tmSlots = saveData.tms.Select(i => new ItemSlot(i)).ToList();

        onUpdated?.Invoke();
        allSlots = new List<List<ItemSlot>>() { slots, hexoballsSlots, tmSlots };

    }
    public int GetItemCount(ItemBase item)
    {
        int category = (int)GetCategoryFromItem(item);
        var currentSlots = GetSlotsByCategory(category);
        var itemSlot = currentSlots.FirstOrDefault(slot => slot.item == item);

        if (itemSlot != null)
        {
            return itemSlot.count;
        }
        else return 0;
    }
}
[System.Serializable]
public class ItemSlot
{
    public ItemBase item;
    public int count;
    public ItemSlot()
    {

    }
    public ItemSlot(ItemSaveData saveData)
    {
        item = ItemDB.GetObjectByName(saveData.name);
        count = saveData.count;
    }
    public ItemSaveData GetSaveData()
    {
        var saveData = new ItemSaveData()
        {
            name = item.name,
            count = count
        };
        return saveData;
    }

}
[System.Serializable]
public class ItemSaveData
{
    public string name;
    public int count;
}
[System.Serializable]
public class InventorySaveData
{
    public List<ItemSaveData> items, hexoballs, tms;
}