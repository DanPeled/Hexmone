using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChoiceText : MonoBehaviour
{
    public TextMeshProUGUI text;
    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }
    public void SetSelected(bool selected){
        text.color = (selected) ? GlobalSettings.i.highlightedColor : Color.black;
    }
}
