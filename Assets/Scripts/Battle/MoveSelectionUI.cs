using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MoveSelectionUI : MonoBehaviour
{
    public List<TextMeshProUGUI> moveTexts;
    public Color hightlightedColor;
    int currentSelection = 0;

    public void SetMoveData(List<MoveBase> currentMoves, MoveBase newMove)
    {
        for (int i = 0; i < currentMoves.Count; i++)
        {
            moveTexts[i].text = currentMoves[i].name;
        }
        moveTexts[currentMoves.Count].text = newMove.name;
    }
    public void HandleMoveSelection(Action<int> onSelected)
    {
        if (InputSystem.down.isClicked())
        {
            currentSelection++;
        }
        else if (InputSystem.up.isClicked())
        {
            currentSelection--;
        }
        currentSelection = Mathf.Clamp(currentSelection, 0, 4);
        UpdateMoveSelection(currentSelection);
        if (InputSystem.action.isClicked()){
            onSelected?.Invoke(currentSelection);
        }
    }
    public void UpdateMoveSelection(int selection)
    {
        for (int i = 0; i < 5; i++)
        {
            if (i == selection){
                moveTexts[i].color = hightlightedColor;
            } else {
                moveTexts[i].color = Color.black;
            }
        }
    }
}