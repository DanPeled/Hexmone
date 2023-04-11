using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class WalletUI : MonoBehaviour
{
    public TextMeshProUGUI moneyText;
    void Start()
    {
        Wallet.i.onMoneyChanged += SetMoneyText;
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
    public void SetMoneyText()
    {
        moneyText.text = "$" + Wallet.i.money;

    }
}
