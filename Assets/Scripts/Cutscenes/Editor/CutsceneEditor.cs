using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Cutscene))]
public class CutsceneEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var cutscene = target as Cutscene;
        if (GUILayout.Button("Add dialouge action"))
        {
            cutscene.AddAction(new DialougeAction());
        }
        if (GUILayout.Button("Add Actor action"))
        {
            cutscene.AddAction(new MoveActorAction());
        }
        base.OnInspectorGUI();
    }
}