using System.Collections.Generic;
using System;
using UnityEngine;
public class ItemDB {
    static Dictionary<string, ItemBase> items;
    public static void Init()
    {
        items = new Dictionary<string, ItemBase>();
        var itemArr = Resources.LoadAll<ItemBase>("");
        foreach (var item in itemArr)
        {
            if (items.ContainsKey(item.name))
            {
                continue;
            }
            items[item.name] = item;
        }
    }
    public static ItemBase GetItemByName(string name)
    {
        if (!items.ContainsKey(name))
        {
            return null;
        }
        return items[name];
    }
}