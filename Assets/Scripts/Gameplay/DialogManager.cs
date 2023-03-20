using System.Diagnostics.SymbolStore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DialogManager : MonoBehaviour
{
    public Action OnShowDialog;
    public Action OnCloseDialog;
    public GameObject dialogBox;
    public TextMeshProUGUI dialogText;
    public int lettersPerSecond;
    public static DialogManager instance;
    Coroutine lastRoutine;
    void Awake()
    {
        instance = this;
    }
    Dialog dialog;
    int currentLine = 0;
    bool isTyping;
    public IEnumerator ShowDialog(Dialog dialog)
    {
        yield return new WaitForEndOfFrame();
        OnShowDialog?.Invoke();
        this.dialog = dialog;

        dialogBox.SetActive(true);
        lastRoutine = StartCoroutine(TypeDialog(dialog.lines[0]));
    }
    public IEnumerator TypeDialog(string line)
    {
        yield return new WaitForEndOfFrame();
        dialogText.text = "";
        isTyping = true;
        yield return new WaitForSeconds(0.01f);
        foreach (var letter in line.ToCharArray())
        {
            if (isTyping)
            {
                dialogText.text += letter;
                yield return new WaitForSeconds(1f / lettersPerSecond);
            }
            else {
                dialogText.text = "";
                break;
            }
        }
        isTyping = false;
    }
    public void HandleUpdate()
    {
        if (Input.GetButtonDown("Action") && !(isTyping))
        {
            currentLine++;
            if (currentLine < dialog.lines.Count)
            {
                isTyping = false;
                StopCoroutine(lastRoutine);
                lastRoutine = StartCoroutine(TypeDialog(dialog.lines[currentLine]));
            }
            else
            {
                currentLine = 0;
                dialogBox.SetActive(false);
                OnCloseDialog?.Invoke();
            }
        }
    }
}
