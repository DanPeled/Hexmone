using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleDialogBox : MonoBehaviour
{
    public int letterPerSecond;
    public TextMeshProUGUI dialogText;
    public GameObject actionSelector,
        moveSelector,
        moveDetails,
        choiceBox;
    public List<TextMeshProUGUI> actionTexts;
    public List<TextMeshProUGUI> moveTexts;
    public TextMeshProUGUI ppText,
        typeText,
        yesText,
        noText;

    private Coroutine typingCoroutine;

    /// <summary>
    /// Sets the dialog text and starts typing animation.
    /// </summary>
    /// <param name="dialog">The dialog text to display.</param>
    public void SetDialog(string dialog)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        dialogText.text = "";
        typingCoroutine = StartCoroutine(TypeDialog(dialog));
    }

    /// <summary>
    /// Coroutine for typing the dialog text.
    /// </summary>
    /// <param name="dialog">The dialog text to type.</param>
    /// <param name="dialogText">The dialog text component.</param>
    /// <returns></returns>
    private IEnumerator TypeDialog(string dialog, TextMeshProUGUI dialogText)
    {
        dialogText.text = " ";
        yield return new WaitForSeconds(0.03f);
        dialogText.text = " ";

        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / letterPerSecond);
        }

        yield return new WaitForSeconds(1f);
    }

    /// <summary>
    /// Coroutine for typing the dialog text using the default dialog text component.
    /// </summary>
    /// <param name="dialog">The dialog text to type.</param>
    /// <returns></returns>
    public IEnumerator TypeDialog(string dialog)
    {
        yield return TypeDialog(dialog, this.dialogText);
    }

    /// <summary>
    /// Toggles the visibility of the dialog text.
    /// </summary>
    /// <param name="state">The state to set for the dialog text visibility.</param>
    public void ToggleDialogText(bool state)
    {
        dialogText.enabled = state;
    }

    /// <summary>
    /// Toggles the visibility of the choice box.
    /// </summary>
    /// <param name="state">The state to set for the choice box visibility.</param>
    public void ToggleChoiceBox(bool state)
    {
        choiceBox.SetActive(state);
    }

    /// <summary>
    /// Toggles the visibility of the action selector.
    /// </summary>
    /// <param name="state">The state to set for the action selector visibility.</param>
    public void ToggleActionSelector(bool state)
    {
        actionSelector.SetActive(state);
    }

    /// <summary>
    /// Toggles the visibility of the move selector and move details.
    /// </summary>
    /// <param name="state">The state to set for the move selector and move details visibility.</param>
    public void ToggleMoveSelector(bool state)
    {
        moveSelector.SetActive(state);
        moveDetails.SetActive(state);
    }

    /// <summary>
    /// Updates the selection of the action in the action selector.
    /// </summary>
    /// <param name="selectedAction">The index of the selected action.</param>
    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; i++)
        {
            if (i == selectedAction)
            {
                actionTexts[i].color = GlobalSettings.i.highlightedColor;
            }
            else
            {
                actionTexts[i].color = Color.black;
            }
        }
    }

    /// <summary>
    /// Updates the selection in the choice box.
    /// </summary>
    /// <param name="yesSelected">The state of the "yes" selection.</param>
    public void UpdateChoiceBoxSelection(bool yesSelected)
    {
        yesText.color = yesSelected ? GlobalSettings.i.highlightedColor : Color.black;
        noText.color = !yesSelected ? GlobalSettings.i.highlightedColor : Color.black;
    }

    /// <summary>
    /// Updates the selection of the move in the move selector.
    /// </summary>
    /// <param name="selectedMove">The index of the selected move.</param>
    /// <param name="move">The selected move.</param>
    public void UpdateMoveSelection(int selectedMove, Move move)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i == selectedMove)
            {
                moveTexts[i].color = GlobalSettings.i.highlightedColor;
            }
            else
            {
                moveTexts[i].color = Color.black;
            }
        }

        ppText.text = $"PP {move.PP} / {move.base_.pp}";
        typeText.text = move.base_.type.ToString();

        if (move.PP == 0)
        {
            ppText.color = Color.red;
        }
        else
        {
            ppText.color = Color.black;
        }
    }

    /// <summary>
    /// Sets the names of the moves in the move selector.
    /// </summary>
    /// <param name="moves">The list of moves.</param>
    public void SetMoveNames(List<Move> moves)
    {
        for (int i = 0; i < moveTexts.Count; i++)
        {
            if (i < moves.Count)
            {
                moveTexts[i].text = moves[i].base_.name;
            }
            else
            {
                moveTexts[i].text = "-";
            }
        }
    }
}
