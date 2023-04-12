using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapArea))]
public class MapAreaEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var mapArea = (MapArea)target;
        var style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        GUILayout.Label($"Total Chance = {mapArea.totalChance}", style);
        if (mapArea.totalChance != 100)
            EditorGUILayout.HelpBox("The total chance is not 100", MessageType.Error);
    }
}