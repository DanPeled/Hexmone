using System.Collections;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    List<ItemSlotUI> slotUIs;
    public GameObject itemList;
    public ItemSlotUI itemSlotUI;
    Inventory inventory;
    int selectedItem = 0, selectedCategory;
    public Image itemIcon;
    public TextMeshProUGUI descriptionText, categoryText;
    public RectTransform itemListRect;
    public PartyScreen partyScreen;
    const int itemsInViewport = 8;
    public Image upArrow, downArrow;
    InventoryUIState state;
    Action onItemUsed;
    void Awake()
    {
        inventory = Inventory.GetInventory();
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    void Start()
    {
        UpdateItemList();
        inventory.onUpdated += UpdateItemList;
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
        foreach (var itemSlot in inventory.GetSlotsByCategory(selectedCategory))
        {
            var slot = Instantiate(itemSlotUI, itemList.transform);
            slot.SetData(itemSlot);

            slotUIs.Add(slot);
        }

        UpdateItemSelection();
    }
    public void HandleUpdate(Action onBack, Action onItemUsed = null)
    {
        this.onItemUsed = onItemUsed;
        if (state == InventoryUIState.ItemSelection)
        {
            int prevSelection = selectedItem;
            int prevCategory = selectedCategory;
            if (InputSystem.instance.down.isClicked())
            {
                selectedItem++;
            }
            else if (InputSystem.instance.up.isClicked())
            {
                selectedItem--;
            }
            else if (InputSystem.instance.right.isClicked())
            {
                selectedCategory++;
            }
            else if (InputSystem.instance.left.isClicked())
            {
                selectedCategory--;
            }

            if (selectedCategory > Inventory.ItemCategories.Count - 1)
            {
                selectedCategory = 0;
            }
            else if (selectedCategory < 0)
            {
                selectedCategory = Inventory.ItemCategories.Count - 1;
            }
            selectedCategory = Mathf.Clamp(selectedCategory, 0, Inventory.ItemCategories.Count - 1);
            if (prevCategory != selectedCategory)
            {
                ResetSelection();
                categoryText.text = Inventory.ItemCategories[selectedCategory];
                UpdateItemList();
            }
            else if (prevSelection != selectedItem)
                UpdateItemSelection();
            if (InputSystem.instance.action.isClicked())
            {
                state = InventoryUIState.PartySelection;
                OpenPartyScreen();
            }
            else if (InputSystem.instance.back.isClicked())
            {
                onBack?.Invoke();
            }
        }
        else if (state == InventoryUIState.PartySelection)
        {
            // Handle Party selection
            Action onSelected = () =>
            {
                // use the item on the selected creature
                StartCoroutine(UseItem());
            };
            Action onBackPartyScreen = () =>
            {
                ClosePartyScreen();
            };
            partyScreen.HandleUpdate(onSelected, onBack);
        }
    }
    IEnumerator UseItem()
    {
        state = InventoryUIState.Busy;
        var usedItem = inventory.UseItem(selectedItem, partyScreen.SelectedMember);
        if (usedItem != null)
        {
            yield return DialogManager.instance.ShowDialogText($"The player used {usedItem.name} ");
            onItemUsed?.Invoke();
        }
        else
        {
            yield return DialogManager.instance.ShowDialogText($"It won't have any effect!");
        }
        ClosePartyScreen();

    }
    void UpdateItemSelection()
    {
        var slots = inventory.GetSlotsByCategory(selectedCategory);
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

        selectedItem = Mathf.Clamp(selectedItem, 0, slots.Count - 1);
        if (slots.Count > 0)
        {
            var item = slots[selectedItem].item;
            itemIcon.color = Color.white;
            itemIcon.sprite = item.icon;
            descriptionText.text = item.description;
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
    void ResetSelection()
    {
        selectedItem = 0;

        upArrow.gameObject.SetActive(false);
        downArrow.gameObject.SetActive(false);
        itemIcon.color = new Color(0, 0, 0, 0);
        itemIcon.sprite = null;
        descriptionText.text = "";
    }
    void OpenPartyScreen()
    {
        state = InventoryUIState.PartySelection;
        partyScreen.gameObject.SetActive(true);

    }
    void ClosePartyScreen()
    {
        state = InventoryUIState.ItemSelection;
        partyScreen.gameObject.SetActive(false);
    }
}
public enum InventoryUIState
{
    ItemSelection, PartySelection, Busy
}