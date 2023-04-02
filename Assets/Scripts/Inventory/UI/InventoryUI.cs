using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public GameObject itemList;
    public ItemSlotUI itemSlotUI;
    Inventory inventory;
    int selectedItem = 0;
    List<ItemSlotUI> slotUIs;
    public Image itemIcon;
    public TextMeshProUGUI descriptionText;
    void Awake()
    {
        inventory = Inventory.GetInventory();
    }

    void Start()
    {
        UpdateItemList();
    }
    public void UpdateItemList(){
        // Clear all the existing items
        foreach(Transform child in itemList.transform){
            Destroy(child.gameObject);
        }
        slotUIs = new List<ItemSlotUI>();
        foreach(var itemSlot in inventory.slots){
            var slot = Instantiate(itemSlotUI, itemList.transform);
            slot.SetData(itemSlot);

            slotUIs.Add(slot);
        }

        UpdateItemSelection();
    }
    public void HandleUpdate(Action onBack){
        int prevSelection = selectedItem;
        if (InputSystem.instance.down.isClicked())
        {
            selectedItem++;
        }
        else if (InputSystem.instance.up.isClicked())
        {
            selectedItem--;
        }
        selectedItem = Mathf.Clamp(selectedItem, 0, inventory.slots.Count - 1);
        if (prevSelection != selectedItem)
            UpdateItemSelection();
        if (InputSystem.instance.back.isClicked()){
            onBack?.Invoke();
        }
    }
    void UpdateItemSelection()
    {
        for (int i = 0; i < slotUIs.Count; i++)
        {
            if (i == selectedItem)
            {
                slotUIs[i].nameText.color = GlobalSettings.i.highlightedColor;
            }
            else
            {
                slotUIs[i].nameText.color = Color.black;
            }
        }
        var slot = inventory.slots[selectedItem].item;
        itemIcon.sprite = slot.icon;
        descriptionText.text = slot.description;
    }
}