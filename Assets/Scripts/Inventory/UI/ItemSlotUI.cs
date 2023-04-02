using UnityEngine;
using TMPro;
public class ItemSlotUI : MonoBehaviour
{
    public TextMeshProUGUI nameText, countText;
    public void SetData(ItemSlot itemSlot){
        nameText.text = itemSlot.item.name;
        countText.text = $"X {itemSlot.count}";
    }
}