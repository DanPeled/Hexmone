using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CountSelectorUI : MonoBehaviour
{
    public TextMeshProUGUI countText, priceText;
    bool selected;
    int currentCount, maxCount;
    float pricePerUnit;
    public IEnumerator ShowSelector(int maxCount, float pricePerUnit, Action<int> onCountSelected)
    {
        this.maxCount = maxCount;
        this.pricePerUnit = pricePerUnit;

        currentCount = 1;
        selected = false;

        gameObject.SetActive(true);
        SetValues();
        yield return new WaitUntil(() => selected);

        onCountSelected?.Invoke(currentCount);
        gameObject.SetActive(false);
    }
    void Update()
    {
        int prevCount = currentCount;
        if (InputSystem.instance.up.isClicked())
        {
            currentCount++;
        }
        else if (InputSystem.instance.down.isClicked())
        {
            currentCount--;
        }

        currentCount = Mathf.Clamp(currentCount, 1, maxCount);

        if (currentCount != prevCount)
        {
            SetValues();
        }
        if (InputSystem.instance.action.isClicked()){
            selected = true;
        }
    }
    void SetValues()
    {
        countText.text = "x" + currentCount;
        priceText.text = "$" + (pricePerUnit * currentCount);
    }
}
