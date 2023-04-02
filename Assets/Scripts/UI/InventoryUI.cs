using System;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public void HandleUpdate(Action onBack){
        if (InputSystem.instance.back.isClicked()){
            onBack?.Invoke();
        }
    }
}