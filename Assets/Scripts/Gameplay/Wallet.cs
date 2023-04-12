using UnityEngine;
using System;
public class Wallet : MonoBehaviour, ISavable
{
    public float money;
    public event Action onMoneyChanged;
    public static Wallet i;
    void Awake()
    {
        i = this;
    }
    public void AddMoney(float amount)
    {
        money += amount;
        onMoneyChanged?.Invoke();
    }
    public void TakeMoney(float amount)
    {
        money -= amount;
        onMoneyChanged?.Invoke();
    }
    public bool HasMoney(float amount)
    {
        return amount <= money;
    }
    public object CaptureState()
    {
        return money;
    }
    public void RestoreState(object state)
    {
        money = (float)state;
    }
}