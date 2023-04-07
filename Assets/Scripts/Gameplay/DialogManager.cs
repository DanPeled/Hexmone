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
    public bool isTyping;
    public IEnumerator ShowDialog(Dialog dialog, Action onEnd = null)
    {
        this.onEnd = onEnd;
        yield return new WaitForEndOfFrame();
        OnShowDialog?.Invoke();
        this.dialog = dialog;

        dialogBox.SetActive(true);
        SetDialog(dialog.lines[0]);
    }
    public IEnumerator TypeDialog(string line)
    {
        yield return new WaitForEndOfFrame();
        dialogText.text = " ";
        isTyping = true;
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        isTyping = false;
        dialogText.text = line;
    }
    public IEnumerator ShowDialogText(string text, bool waitForInput=true, bool autoClose=true){
        isTyping = true;
        dialogBox.SetActive(true);
        yield return TypeDialog(text);
        if (waitForInput){
            yield return new WaitUntil(() => InputSystem.instance.action.isClicked());
        }
        if (autoClose){
            CloseDialog();
        }
    }
    public void CloseDialog(){
        dialogBox.SetActive(false);
        isTyping = false;
    }
    public void SetDialog(string line){
        isTyping = true;
        this.dialogText.text = line;
        isTyping = false;
    }
    public void HandleUpdate()
    {
        if (InputSystem.instance.action.isClicked())
        {
            currentLine++;
            if (currentLine < dialog.lines.Count)
            {
                SetDialog(dialog.lines[currentLine]);
            }
            else
            {
                currentLine = 0;
                dialogBox.SetActive(false);
                OnCloseDialog?.Invoke();
                GameController.instance.state = GameState.FreeRoam;
                onEnd?.Invoke();
            }
        }
    }
    void Update()
    {
        if (dialog != null)
        SetDialog(dialog.lines[currentLine]);
    }
}
