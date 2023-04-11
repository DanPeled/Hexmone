using UnityEngine;
using System;
public class Wallet : MonoBehaviour
{
    public float money;
    public event Action onMoneyChanged;
    public static Wallet i;
    void Awake()
    {
        i = this;
    }
    public void AddMoney(float amount){
        money += amount;
        onMoneyChanged?.Invoke();
    } public void TakeMoney(float amount){
        money -= amount;
        onMoneyChanged?.Invoke();
    }
}