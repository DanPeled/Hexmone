using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
public enum ShopState { menu, buying, selling, busy }
public class ShopController : MonoBehaviour
{
    [Header("Camera offset")]
    public Vector2 shopCameraOffset;

    [Header("Refrences")]
    public InventoryUI inventoryUI;
    public WalletUI walletUI;
    public CountSelectorUI countSelectorUI;
    public ShopUI shopUI;

    [Header("State")]
    public ShopState state;
    public static ShopController i { get; private set; }
    Inventory inv;
    public event Action onStart, onFinish;

    Merchant merchant;
    void Awake()
    {
        i = this;
    }
    void Start()
    {
        inv = Inventory.GetInventory();
    }
    public IEnumerator StartTrade(Merchant merchant)
    {
        this.merchant = merchant;
        onStart?.Invoke();
        yield return StartMenuState();
    }
    IEnumerator StartMenuState()
    {
        state = ShopState.menu;
        int selectedChoice = 0;
        yield return DialogManager.instance.ShowDialogText($"How may I serve you?",
         waitForInput: false, choices: new List<string>() { "Buy", "Sell", "Quit" },
          onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);
        switch (selectedChoice)
        {
            case 0:
                //Buy
                GameController.instance.MoveCamera(shopCameraOffset);
                walletUI.Show();
                shopUI.Show(this.merchant.items, (selectedItem) => StartCoroutine(BuyItem(selectedItem)), OnBackFromBuying);
                state = ShopState.buying;
                break;
            case 1:
                //Sell
                state = ShopState.selling;
                inventoryUI.gameObject.SetActive(true);
                inventoryUI.UpdateItemList();
                break;
            case 2:
                //Quit
                onFinish?.Invoke();
                yield break;
        }
    }
    public void HandleUpdate()
    {
        if (state == ShopState.selling)
        {
            inventoryUI.HandleUpdate(onBackFromSelling, (selectedItem) => { StartCoroutine(SellItem(selectedItem)); });
        }
        else if (state == ShopState.buying)
        {
            shopUI.HandleUpdate();
        }
    }
    void onBackFromSelling()
    {
        inventoryUI.UpdateItemList();

        inventoryUI.gameObject.SetActive(false);
        StartCoroutine(StartMenuState());
    }
    IEnumerator SellItem(ItemBase item)
    {
        state = ShopState.busy;


        if (!item.isSellable)
        {
            yield return DialogManager.instance.ShowDialogText($"You can't sell that!");
            state = ShopState.selling;
            yield break;
        }
        walletUI.Show();
        float sellPrice = Mathf.Round(item.price / 2);
        int countToSell = 1;

        var itemCount = inv.GetItemCount(item);
        if (itemCount > 1)
        {
            yield return DialogManager.instance.ShowDialogText($"How many would you like to sell?", waitForInput: false, autoClose: false);
            yield return countSelectorUI.ShowSelector(itemCount, sellPrice, (selectedCount) => countToSell = selectedCount);
            DialogManager.instance.CloseDialog();
        }
        sellPrice *= countToSell;


        int selectedChoice = 0;
        yield return DialogManager.instance.ShowDialogText($"I can give you {sellPrice} for that! Would you like to sell it?",
         waitForInput: false, choices: new List<string>() { "Yes", "No" },
          onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        switch (selectedChoice)
        {
            case 0:
                //Yes
                inv.RemoveItem(item, countToSell);
                Wallet.i.AddMoney(sellPrice);
                yield return DialogManager.instance.ShowDialogText($"Turned over {item.name} and recived {sellPrice}!");
                break;
            case 1:
                //No
                break;
        }
        walletUI.Close();
        state = ShopState.selling;
    }
    IEnumerator BuyItem(ItemBase item)
    {
        state = ShopState.busy;

        yield return DialogManager.instance.ShowDialogText("How many would you like to buy?", waitForInput: false, autoClose: false);
        int countToBuy = 1;
        yield return countSelectorUI.ShowSelector(100, item.price, (selectedCount) => countToBuy = selectedCount);
        DialogManager.instance.CloseDialog();

        float totalPrice = item.price * (float)countToBuy;
        if (Wallet.i.HasMoney(totalPrice))
        {
            int selectedChoice = 0;
            yield return DialogManager.instance.ShowDialogText($"That will be {totalPrice}",
             waitForInput: false, choices: new List<string>() { "Yes", "No" },
              onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);
            if (selectedChoice == 0)
            {
                // yes
                inv.AddItem(item, countToBuy);
                Wallet.i.TakeMoney(totalPrice);
                yield return DialogManager.instance.ShowDialogText($"Thank you for shopping with us");
                OnBackFromBuying();
            }
        }
        else
        {
            yield return DialogManager.instance.ShowDialogText($"You don't have enough money for that");
            OnBackFromBuying();
        }
        state = ShopState.buying;
    }
    void OnBackFromBuying()
    {
        GameController.instance.MoveCamera(-shopCameraOffset);
        shopUI.Close();
        walletUI.Close();
        StartCoroutine(StartMenuState());
    }
}