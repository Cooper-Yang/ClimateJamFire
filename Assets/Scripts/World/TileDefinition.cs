using UnityEngine;

[CreateAssetMenu(fileName = "New Tile Definition", menuName = "Level System/Tile Definition")]
public class TileDefinition : ScriptableObject
{
    [Header("Basic Properties")]
    public char character = '.';
    public string tileName = "Tile";
    public GameObject prefab;
    
    [Header("Editor Display")]
    public Color editorColor = Color.white;
    [TextArea(2, 4)]
    public string description;
}
