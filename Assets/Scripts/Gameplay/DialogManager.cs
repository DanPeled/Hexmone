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
    [SerializeField] public int currentLine;
    Action onEnd;
    void Awake()
    {
        instance = this;
    }
    Dialog dialog;
    public IEnumerator ShowDialog(Dialog dialog)
    {
        yield return new WaitForEndOfFrame();
        OnShowDialog?.Invoke();
        this.dialog = dialog;
        dialogBox.SetActive(true);
        foreach (var line in dialog.lines)
        {
            yield return TypeDialog(line);
            yield return new WaitUntil(() => InputSystem.instance.action.isClicked());
        }
        dialogBox.SetActive(false);
    }
    public IEnumerator TypeDialog(string line)
    {
        yield return new WaitForEndOfFrame();
        dialogText.text = " ";
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        OnShowDialog?.Invoke();
    }
    public IEnumerator ShowDialogText(string text, bool waitForInput = true, bool autoClose = true)
    {
        OnShowDialog?.Invoke();
        dialogBox.SetActive(true);
        yield return TypeDialog(text);
        if (waitForInput)
        {
            yield return new WaitUntil(() => InputSystem.instance.action.isClicked());
        }
        if (autoClose)
        {
            CloseDialog();
        }
        OnCloseDialog?.Invoke();
    }
    public void CloseDialog()
    {
        dialogBox.SetActive(false);
        OnCloseDialog?.Invoke();
        GameController.instance.state = GameState.FreeRoam;
    }
    public void SetDialog(string line)
    {
        this.dialogText.text = line;
    }
    public void HandleUpdate()
    {

    }
    void Update()
    {
        if (dialog != null)
            SetDialog(dialog.lines[currentLine]);
        if (dialogBox != null && !dialogBox.activeInHierarchy)
        {
            GameController.instance.state = GameState.FreeRoam;
        }
    }
}
