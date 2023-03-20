using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleDialogBox : MonoBehaviour
{
    public int letterPerSecond;
    public Color highlightedColor;
    public TextMeshProUGUI dialogText;
    public GameObject actionSelector,
        moveSelector,
        moveDetails;
    public List<TextMeshProUGUI> actionTexts;
    public List<TextMeshProUGUI> moveTexts;

    public TextMeshProUGUI ppText,
        typeText;

    public void SetDialog(string dialog)
    {
        dialogText.text = "";
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog, TextMeshProUGUI dialogText)
    {
        dialogText.text = "";
        yield return new WaitForSeconds(0.01f);

        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / letterPerSecond);
        }
        yield return new WaitForSeconds(1f);
    }

    public void ToggleDialogText(bool state)
    {
        dialogText.enabled = state;
    }

    public void ToggleActionSelector(bool state)
    {
        actionSelector.SetActive(state);
    }

    public void ToggleMoveSelector(bool state)
    {
        moveSelector.SetActive(state);
        moveDetails.SetActive(state);
    }

    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; i++)
        {
            if (i == selectedAction)
            {
                actionTexts[i].color = highlightedColor;
            }
            else
            {
                actionTexts[i].color = Color.black;
            }
        }
    }

    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i == selectedMove)
            {
                moveTexts[i].color = highlightedColor;
            }
            else
            {
                moveTexts[i].color = Color.black;
            }
        }
        ppText.text = $"PP {move.PP} / {move.base_.pp}";
        typeText.text = move.base_.type.ToString();

        if (move.PP == 0){
            ppText.color = Color.red;
        }
        else ppText.color = Color.black;
    }

    public void SetMoveNames(List<Move> moves)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i < moves.Count)
            {
                moveTexts[i].text = moves[i].base_.moveName;
            }
            else
            {
                moveTexts[i].text = "-";
            }
        }
    }
}
