using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
public class ShopUI : MonoBehaviour
{
    public GameObject itemList;
    public ItemSlotUI itemSlotUI;
    public TextMeshProUGUI itemDescription;
    public Image itemIcon, upArrow, downArrow;
    List<ItemBase> items;
    List<ItemSlotUI> slotUIs;
    int selectedItem = 0;
    const int itemsInViewport = 8;
    RectTransform itemListRect;
    Action<ItemBase> onItemSelected;
    Action onBack;
    void Awake()
    {
        itemListRect = itemList.GetComponent<RectTransform>();
    }
    public void Show(List<ItemBase> items, Action<ItemBase> onItemSelected, Action onBack)
    {
        this.onItemSelected = onItemSelected;
        this.onBack= onBack;
        this.items = items;
        gameObject.SetActive(true);

        UpdateItemList();

    }
    public void HandleUpdate()
    {
        var prevSelection = selectedItem;
        if (InputSystem.down.isClicked())
        {
            selectedItem++;
        }
        else if (InputSystem.up.isClicked())
        {
            selectedItem--;
        }
        selectedItem = Mathf.Clamp(selectedItem, 0, items.Count - 1);
        if (prevSelection != selectedItem)
        {
            UpdateItemList();
        }
        if (InputSystem.action.isClicked()){
            onItemSelected?.Invoke(items[selectedItem]);
        } else if (InputSystem.back.isClicked()){
            onBack?.Invoke();
        }
    }
    public void UpdateItemList()
    {
        // Clear all the existing items
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }
        slotUIs = new List<ItemSlotUI>();
        foreach (var item in items)
        {
            var slot = Instantiate(itemSlotUI, itemList.transform);
            slot.SetNameAndPrice(item);

            slotUIs.Add(slot);
        }

        UpdateItemSelection();
    }
    void UpdateItemSelection()
    {
        selectedItem = Mathf.Clamp(selectedItem, 0, items.Count - 1);

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

        if (items.Count > 0)
        {
            var item = items[selectedItem];
            itemIcon.color = Color.white;
            itemIcon.sprite = item.icon;
            itemDescription.text = item.description;
        }
        HandleScrolling();
    }
    void HandleScrolling()
    {
        if (slotUIs.Count <= itemsInViewport)
        {
            downArrow.gameObject.SetActive(false);
            upArrow.gameObject.SetActive(false);
            return;
        }
        float scrollPos = Mathf.Clamp(selectedItem - itemsInViewport / 2, 0, selectedItem) * slotUIs[0].height;
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);

        bool showUpArrow = selectedItem > itemsInViewport / 2;
        upArrow.gameObject.SetActive(showUpArrow);
        bool showDownArrow = selectedItem + 4 < slotUIs.Count;
        downArrow.gameObject.SetActive(showDownArrow);
    }
    public void Close(){
        gameObject.SetActive(false);
    }
}