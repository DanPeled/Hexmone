using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MobileButton : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    Button button;
    Action onDown;
    Action onUp;
    void Awake()
    {
        this.button = this.GetComponent<Button>();
    }
    public void SetAction(Action action){
        this.onDown = action;
    }
    public void SetUp(Action action){
        this.onUp = action;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        // This function will be called when the mouse pointer enters the button
    }
    public void OnPointerDown(PointerEventData eventData){
        onDown?.Invoke();
    }
    public void OnPointerUp(PointerEventData eventData){
        onUp?.Invoke();
    }
    public void OnPointerExit(PointerEventData eventData){
    }
}