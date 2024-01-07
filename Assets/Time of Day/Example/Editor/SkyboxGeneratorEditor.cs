using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SkyboxGenerator))]
public class SkyboxGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Render Skybox"))
        {
            (target as SkyboxGenerator).RenderSkybox();
            AssetDatabase.Refresh();
        }
    }
}
