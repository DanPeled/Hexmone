using UnityEngine;
using TMPro;
public class ItemSlotUI : MonoBehaviour
{
    public TextMeshProUGUI nameText, countText;
    RectTransform rectTransform;
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    public float height => rectTransform.rect.height;
    public void SetData(ItemSlot itemSlot){
        nameText.text = itemSlot.item.name;
        countText.text = $"X {itemSlot.count}";
    }
}