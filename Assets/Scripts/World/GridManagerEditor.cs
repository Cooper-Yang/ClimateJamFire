using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridManager))]
public class GridManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridManager gm = (GridManager)target;

        EditorGUILayout.Space();

        if (GUILayout.Button("Generate Grid"))
        {
            gm.GenerateGrid();
        }

        if (GUILayout.Button("Clear Grid"))
        {
            gm.ClearGrid();
        }
    }
}
