#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GalaxyVFXController))]
public class GalaxyVFXInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GalaxyVFXController vfxController = (GalaxyVFXController)target;

        if (GUILayout.Button("Level 1 Spiral"))
        {
            vfxController.Level1SprialShader_Editor();
        }  
        if (GUILayout.Button("Level 2 Spiral"))
        {
            vfxController.Level2SprialShader_Editor();
        }  
        if (GUILayout.Button("Level 3 Spiral"))
        {
            vfxController.Level3SprialShader_Editor();
        }  
        if (GUILayout.Button("Level 4 Spiral"))
        {
            vfxController.Level4SprialShader_Editor();
        }  
        if (GUILayout.Button("Level 5 Spiral"))
        {
            vfxController.Level5SprialShader_Editor();
        }  
        if (GUILayout.Button("Level 6 Spiral"))
        {
            vfxController.Level6SprialShader_Editor();
        }  
        if (GUILayout.Button("Level 7 Spiral"))
        {
            vfxController.Level7SprialShader_Editor();
        }  
        if (GUILayout.Button("Level 8 Spiral"))
        {
            vfxController.Level8SprialShader_Editor();
        }  
        if (GUILayout.Button("Level 9 Spiral"))
        {
            vfxController.Level9SprialShader_Editor();
        }  
    }
}
#endif
