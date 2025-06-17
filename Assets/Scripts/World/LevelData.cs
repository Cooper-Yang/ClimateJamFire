using UnityEngine;

[CreateAssetMenu(fileName = "New Level", menuName = "Level System/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Level Info")]
    public string levelName;
    public int width;
    public int height;
    
    [Header("Tile System")]
    public TileDatabase tileDatabase;
    
    [Header("Level Layout")]
    [TextArea(10, 20)]
    public string levelLayout;
    
    // Get tile data by character
    public TileData GetTileData(char character)
    {
        if (tileDatabase == null) return null;
        return tileDatabase.GetTileData(character);
    }
}