using System.Collections;
using UnityEngine;

[System.Serializable]
public class DialougeAction : CutSceneAction
{
    public string dialog;

    public override IEnumerator Play()
    {
        yield return DialogManager.instance.ShowDialogText(dialog);
    }
}