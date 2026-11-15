
using UnityEngine;
using UnityEditor;

public class ForceTreeDistance : EditorWindow
{
    Terrain terrain;
    float distance = 1000000f;

    [MenuItem("Tools/Force Terrain Tree Distance")]
    static void Open()
    {
        GetWindow<ForceTreeDistance>("Tree Distance");
    }

    void OnGUI()
    {
        terrain = (Terrain)EditorGUILayout.ObjectField(
            "Terrain", terrain, typeof(Terrain), true);

        distance = EditorGUILayout.FloatField(
            "Tree Distance", distance);

        if (GUILayout.Button("Apply"))
        {
            if (terrain == null)
            {
                Debug.LogError("Assign a Terrain first.");
                return;
            }

            terrain.treeDistance = distance;
            EditorUtility.SetDirty(terrain);
            Debug.Log("Tree distance set to " + terrain.treeDistance);
        }
    }
}
