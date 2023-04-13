using System.Collections;
using UnityEngine;
[System.Serializable]
public class CutSceneAction
{
    public string name;
    public bool waitForCompletion = true;

    public virtual IEnumerator Play()
    {
        yield break;
    }
}