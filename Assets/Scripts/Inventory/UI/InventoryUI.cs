using System.Runtime.CompilerServices;
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
    public RectTransform itemListRect;
    public PartyScreen partyScreen;
    const int itemsInViewport = 8;
    public Image upArrow, downArrow;
    InventoryUIState state;
    void Awake()
    {
        inventory = Inventory.GetInventory();
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    void Start()
    {
        UpdateItemList();
    }
    void Update()
    {
    }
    public void UpdateItemList()
    {
        // Clear all the existing items
        foreach (Transform child in itemList.transform)
        {
            Destroy(child.gameObject);
        }
        slotUIs = new List<ItemSlotUI>();
        foreach (var itemSlot in inventory.slots)
        {
            var slot = Instantiate(itemSlotUI, itemList.transform);
            slot.SetData(itemSlot);

            slotUIs.Add(slot);
        }

        UpdateItemSelection();
    }
    public void HandleUpdate(Action onBack)
    {
        if (state == InventoryUIState.ItemSelection)
        {
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
            if (InputSystem.instance.action.isClicked()){
                state = InventoryUIState.PartySelection;
                OpenPartyScreen();
            }
            else if (InputSystem.instance.back.isClicked())
            {
                onBack?.Invoke();
            }
        } else if (state == InventoryUIState.PartySelection){
            // Handle Party selection
            Action onSelected = () => {
                // use the item on the selected creature
            };
            Action onBackPartyScreen = () => {
                ClosePartyScreen();
            };
            partyScreen.HandleUpdate(onSelected, onBack);
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

        HandleScrolling();
    }
    void HandleScrolling()
    {
        if (slotUIs.Count <= itemsInViewport){
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
    void OpenPartyScreen(){
        state = InventoryUIState.PartySelection;
        partyScreen.gameObject.SetActive(true);

    }
    void ClosePartyScreen(){
        state = InventoryUIState.ItemSelection;
        partyScreen.gameObject.SetActive(false);
    }
}
public enum InventoryUIState
{
    ItemSelection, PartySelection, Busy
}