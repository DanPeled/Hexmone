using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
public enum ShopState { menu, buying, selling, busy }
public class ShopController : MonoBehaviour
{
    public event Action onStart, onFinish;
    public InventoryUI inventoryUI;
    public WalletUI walletUI;
    public static ShopController i { get; private set; }
    Inventory inv;
    public ShopState state;
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

        int selectedChoice = 0;
        yield return DialogManager.instance.ShowDialogText($"I can give you {sellPrice} for that! Would you like to sell it?",
         waitForInput: false, choices: new List<string>() { "Yes", "No" },
          onChoiceSelected: choiceIndex => selectedChoice = choiceIndex);

        switch (selectedChoice)
        {
            case 0:
                //Yes
                inv.RemoveItem(item);
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