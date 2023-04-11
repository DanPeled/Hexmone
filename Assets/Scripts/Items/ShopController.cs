using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
public enum ShopState { menu, buying, selling, busy }
public class ShopController : MonoBehaviour
{
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
                state = ShopState.buying;
                walletUI.Show();
                shopUI.Show(this.merchant.items);
                break;
            case 1:
                //Sell
                state = ShopState.selling;
                inventoryUI.gameObject.SetActive(true);
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
}