using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileDatabase))]
public class TileDatabaseEditor : Editor
{    public override void OnInspectorGUI()
    {
        TileDatabase database = (TileDatabase)target;
        
        DrawDefaultInspector();
        
        GUILayout.Space(10);
        
        // Display tile count
        EditorGUILayout.LabelField($"Total Tiles: {database.tiles.Count}", EditorStyles.helpBox);
        
        // Display character mapping
        if (database.tiles.Count > 0)
        {
            GUILayout.Label("Character Mapping:", EditorStyles.boldLabel);
            foreach (var tile in database.tiles)
            {
                if (tile != null)
                {
                    EditorGUILayout.LabelField($"'{tile.character}' → {tile.tileName}", EditorStyles.miniLabel);
                }
            }
        }
    }
}
