using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WorldEditor))]
public class WorldEditorEditor : Editor
{
    // Custom editor for world editor screen
    public override void OnInspectorGUI()
    {
        var we = (WorldEditor)target;
        if (GUILayout.Button("Clear"))
        {
            we.Clear();
        }
        base.OnInspectorGUI();
    }
}
