using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuButton : MonoBehaviour, IPointerEnterHandler
{
    public int index;
    Button button;

    void Awake()
    {
        this.button = this.GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // This function will be called when the mouse pointer enters the button
        GameObject.FindObjectOfType<MainMenuSelect>().currentAction = index;
    }
}