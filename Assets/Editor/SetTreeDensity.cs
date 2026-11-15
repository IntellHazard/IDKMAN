using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class SetTreeDensity
{
    static SetTreeDensity()
    {
        EditorApplication.update += OnEditorUpdate;
    }

    private static void OnEditorUpdate()
    {
        try
        {
            // Force-set the terrain tree density editor preference to 1.0
            EditorPrefs.SetFloat("TerrainBrushTreeDensity", 1.0f);
        }
        catch (System.Exception)
        {
            // Suppress background errors to keep the console clean
        }
    }
}