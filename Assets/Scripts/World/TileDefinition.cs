using UnityEngine;

[CreateAssetMenu(fileName = "New Tile Definition", menuName = "Level System/Tile Definition")]
public class TileDefinition : ScriptableObject
{
    [Header("Basic Properties")]
    public char character = '.';
    public TileType tileType;
    public GameObject prefab;

    [Header("Editor Display")]
    public Color editorColor = Color.white;
    [TextArea(2, 4)]
    public string description;

    public string GetTileName()
    {
        switch (tileType)
        {
            case TileType.Mountain:
                return "Mountain";
            case TileType.Tree:
                return "Tree";
            case TileType.Smoke:
                return "SmokeTree";
            case TileType.Plain:
                return "Plains";
            case TileType.House:
                return "House";
            case TileType.FireStation:
                return "Fire Station";
            default:
                return "Default";
        }
    }
}
