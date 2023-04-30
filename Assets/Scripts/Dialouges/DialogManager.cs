using System.Diagnostics.SymbolStore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class DialogManager : MonoBehaviour
{
    [Header("NPC Details")]
    public GameObject npcDetails;
    public Image npcImage;
    public TextMeshProUGUI npcName;
    public Action OnShowDialog;
    public Action OnCloseDialog;
    [Header("Refrences")]
    public GameObject dialogBox;
    public ChoiceBox choiceBox;
    public TextMeshProUGUI dialogText;
    [Header("Stats")]
    public int lettersPerSecond;
    public static DialogManager instance;
    [SerializeField] public int currentLine;
    Action onEnd;
    void Awake()
    {
        instance = this;
    }
    Dialog dialog;
    public IEnumerator ShowDialog(Dialog dialog, List<string> choices = null, Action<int> onActionSelected = null, Transform init = null)
    {
        if (init == null)
        {
            ClearDetails();
        }
        //yield return new WaitForEndOfFrame();
        OnShowDialog?.Invoke();
        this.dialog = dialog;
        dialogBox.SetActive(true);
        foreach (var line in dialog.lines)
        {
            yield return TypeDialog(line);
            yield return new WaitUntil(() => InputSystem.action.isClicked());
        }
        if (choices != null && choices.Count > 1)
        {
            yield return choiceBox.ShowChoices(choices, onActionSelected);
        }
        dialogBox.SetActive(false);
        OnCloseDialog?.Invoke();
    }
    public void SetNPCDetails(NPCController npc)
    {
        npcDetails.SetActive(true);
        npcName.text = npc.Name;
        npcImage.sprite = npc.dialougeSprite;
    }
    public void SetTrainerDetails(TrainerController trainer)
    {
        npcDetails.SetActive(true);
        npcName.text = trainer.Name;
        npcImage.sprite = trainer.dialougeSprite;
    }
    public void ClearDetails()
    {
        npcDetails.SetActive(false);
        npcName.name = "";
        npcImage.sprite = null;
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
    public IEnumerator ShowDialogText(string text, bool waitForInput = true, bool autoClose = true, List<string> choices = null, Action<int> onChoiceSelected = null, Transform init = null)
    {
        if (init == null)
        {
            ClearDetails();
        }
        // if (dialogBox.gameObject.activeInHierarchy){
        //     dialogBox.SetActive(false);
        //     CloseDialog();
        // }
        dialogText.text = "";
        OnShowDialog?.Invoke();
        dialogBox.SetActive(true);
        AudioManager.i.PlaySFX(AudioId.UISelect);

        yield return TypeDialog(text);
        if (waitForInput)
        {
            yield return new WaitUntil(() => InputSystem.action.isClicked());
        }
        if (choices != null && choices.Count > 1)
        {
            yield return choiceBox.ShowChoices(choices, onChoiceSelected);
        }
        if (autoClose)
        {
            CloseDialog();
        }
        CloseDialog();
        OnCloseDialog?.Invoke();
    }
    public void CloseDialog()
    {
        dialogBox.SetActive(false);
        OnCloseDialog?.Invoke();
        GameController.instance.state = GameController.instance.prevState;
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
    }
}
