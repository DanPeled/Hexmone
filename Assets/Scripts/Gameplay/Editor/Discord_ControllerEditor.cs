using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Discord_Controller))]
public class Discord_ControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Discord_Controller dc = (Discord_Controller)target;
        if (GUILayout.Button("Connect"))
        {
            dc.ConnectToDiscord();
        }
        if (GUILayout.Button("Disconnect"))
        {
            dc.Disconnect();
        }
        base.OnInspectorGUI();
    }
}