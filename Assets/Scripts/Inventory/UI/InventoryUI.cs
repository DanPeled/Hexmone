using System.Collections;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
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
    MoveBase moveToLearn;
    public MoveSelectionUI moveSelectionUI;
    Action<ItemBase> onItemUsed;
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
    public void HandleUpdate(Action onBack, Action<ItemBase> onItemUsed = null)
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
                StartCoroutine(ItemSelected());
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
            partyScreen.HandleUpdate(onSelected, onBackPartyScreen);
        }
        else if (state == InventoryUIState.MoveToForget)
        {

            Action<int> onMoveSelected = (int moveIndex) =>
            {
                StartCoroutine(onMoveToForgetSelected(moveIndex));
            };


            moveSelectionUI.HandleMoveSelection(onMoveSelected);
        }
    }
    IEnumerator ItemSelected()
    {
        state = InventoryUIState.Busy;
        var item = inventory.GetItem(selectedItem, selectedCategory);
        if (GameController.instance.state == GameState.Battle)
        {
            // In a battle
            if (!item.CanUseInBattle)
            {
                // dont allow to use item
                yield return DialogManager.instance.ShowDialogText($"This item cannot be used in battle");
                state = InventoryUIState.ItemSelection;
                yield break;
            }
        }
        else
        {
            // outside a battle
            if (!item.CanUseOutsideBattle)
            {
                // dont allow to use item
                yield return DialogManager.instance.ShowDialogText($"This item cannot be used outside battle");
                state = InventoryUIState.ItemSelection;
                yield break;
            }

        }

        if (selectedCategory == (int)ItemCategory.Hexoballs)
        {
            // Hexoball
            StartCoroutine(UseItem());
        }
        else
        {
            OpenPartyScreen();

            if (item is TMItem){
                // Show if the tm is usable
                partyScreen.ShowIfTmIsUsable(item as TMItem);
            } 
        }
    }
    IEnumerator UseItem()
    {
        state = InventoryUIState.Busy;

        yield return HandleTMItems();

        var usedItem = inventory.UseItem(selectedItem, partyScreen.SelectedMember, selectedCategory);
        if (usedItem != null)
        {
            if (usedItem is RecoveryItem)
                yield return DialogManager.instance.ShowDialogText($"The player used {usedItem.name} ");
            onItemUsed?.Invoke(usedItem);
        }
        else
        {
            if (usedItem is RecoveryItem)
                yield return DialogManager.instance.ShowDialogText($"It won't have any effect!");
        }
        ClosePartyScreen();

    }

    IEnumerator HandleTMItems()
    {
        var tmItem = inventory.GetItem(selectedItem, selectedCategory) as TMItem;
        if (tmItem == null)
        {
            yield break;
        }
        var creature = partyScreen.SelectedMember;

        if (creature.HasMove(tmItem.move))
        {
            yield return DialogManager.instance.ShowDialogText($"{creature._base.creatureName} already knows {tmItem.move.name}");
            yield break;
        }
        if (!tmItem.CanBeTaught(creature))
        {
            yield return DialogManager.instance.ShowDialogText($"{creature._base.creatureName} can't learn {tmItem.move.name}");
            yield break;
        }
        if (creature.moves.Count < creature._base.maxNumberOfMoves)
        {

            creature.LearnMove(tmItem.move);
            yield return DialogManager.instance.ShowDialogText($"{creature._base.creatureName} learned {tmItem.move.moveName}");
        }
        else
        {
            yield return DialogManager.instance.ShowDialogText($"{creature._base.creatureName} is trying to learn {tmItem.move.moveName}");
            yield return DialogManager.instance.ShowDialogText($"But it cannot learn more than 4 moves");
            yield return ChooseMoveToForget(creature, tmItem.move);
            yield return new WaitUntil(() => state != InventoryUIState.MoveToForget);
        }

    }
    IEnumerator ChooseMoveToForget(Creature creature, MoveBase newMove)
    {
        state = InventoryUIState.Busy;
        yield return DialogManager.instance.ShowDialogText($"Choose a move you want to forget", autoClose: false);
        moveSelectionUI.gameObject.SetActive(true);
        moveSelectionUI.SetMoveData(creature.moves.Select(x => x.base_).ToList(), newMove);
        moveToLearn = newMove;
        state = InventoryUIState.MoveToForget;
    }

    void UpdateItemSelection()
    {
        var slots = inventory.GetSlotsByCategory(selectedCategory);
        selectedItem = Mathf.Clamp(selectedItem, 0, slots.Count - 1);

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

        partyScreen.ClearMemberSlotMessages();
        partyScreen.gameObject.SetActive(false);
    }
    IEnumerator onMoveToForgetSelected(int moveIndex)
    {
        var creature = partyScreen.SelectedMember;
        moveSelectionUI.gameObject.SetActive(false);

        DialogManager.instance.CloseDialog();
        if (moveIndex == 4)
        {
            // Dont learn the new move
            yield return (DialogManager.instance.ShowDialogText($"{creature._base.creatureName} did not learn {moveToLearn.moveName}"));
        }
        else
        {
            // forget the selected move and learn new move
            var selectedMove = creature.moves[moveIndex].base_;
            yield return (DialogManager.instance.ShowDialogText($"{creature._base.creatureName} forgot {selectedMove.moveName} and learned {moveToLearn.moveName}"));

            creature.moves[moveIndex] = new Move(moveToLearn);
        }
        moveToLearn = null;
        state = InventoryUIState.ItemSelection;
    }
}
public enum InventoryUIState
{
    ItemSelection, PartySelection, Busy, MoveToForget
}