using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoiceBox : MonoBehaviour
{
    public ChoiceText choiceTextPrefab;
    bool choiceSelected = false;
    List<ChoiceText> choiceTexts;
    int currentChoice;
    public IEnumerator ShowChoices(List<string> choices, Action<int> onChoiceSelected)
    {
        choiceSelected = false;
        currentChoice = 0;
        gameObject.SetActive(true);

        //Delete existing choices
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        choiceTexts = new List<ChoiceText>();
        foreach (string c in choices)
        {
            var choiceTextObject = Instantiate(choiceTextPrefab, transform);
            choiceTextObject.text.text = c;
            choiceTexts.Add(choiceTextObject);
        }
        yield return new WaitUntil(() => choiceSelected);

        onChoiceSelected?.Invoke(currentChoice);
        gameObject.SetActive(false);
    }
    void Update()
    {
        if (InputSystem.down.isClicked())
        {
            currentChoice++;
        }
        else if (InputSystem.up.isClicked())
        {
            currentChoice--;
        }
        currentChoice = Mathf.Clamp(currentChoice, 0, choiceTexts.Count - 1);
        for (int i = 0; i < choiceTexts.Count; i++)
        {
            choiceTexts[i].SetSelected(i == currentChoice);
        }
        if (InputSystem.action.isClicked()){
            choiceSelected = true;
        }
    }
}
