using UnityEngine;

[System.Serializable]
public class TileData
{
    public char character;
    public GameObject prefab;
    public string tileName;
    
    public static TileData FromDefinition(TileDefinition definition)
    {
        if (definition == null) return null;
        
        return new TileData
        {
            character = definition.character,
            prefab = definition.prefab,
            tileName = definition.tileName
        };
    }
}