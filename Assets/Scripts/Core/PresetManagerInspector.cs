#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PresetManager))]
public class PresetManagerInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PresetManager presetManager = (PresetManager)target;

        if (GUILayout.Button("Real Game Startup"))
        {
            presetManager.RealGameStartup_Editor();
        }  
        if (GUILayout.Button("Main Menu Viewer"))
        {
            presetManager.MainMenuViewer_Editor(blackScreenOff: true);
        }  
        if (GUILayout.Button("Room 1 Viewer"))
        {
            presetManager.Room1Viewer_Editor();
        }  
    }
}
#endif