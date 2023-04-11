using UnityEngine;
using TMPro;
public class ItemSlotUI : MonoBehaviour
{
    public TextMeshProUGUI nameText, countText;
    RectTransform rectTransform;
    void Awake()
    {
    }
    public float height => rectTransform.rect.height;
    public void SetData(ItemSlot itemSlot)
    {
        rectTransform = GetComponent<RectTransform>();

        nameText.text = itemSlot.item.name;
        countText.text = $"{itemSlot.count}";
    }
}