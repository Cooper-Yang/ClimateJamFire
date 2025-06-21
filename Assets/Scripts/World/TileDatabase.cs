using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "TileDatabase", menuName = "Level System/Tile Database")]
public class TileDatabase : ScriptableObject
{    [Header("Tile Definitions")]
    public List<TileDefinition> tiles = new List<TileDefinition>();
    
    private Dictionary<char, TileDefinition> tileDict;
      void OnEnable()
    {
        RefreshDictionary();
        

    }
    
    public void RefreshDictionary()
    {
        tileDict = new Dictionary<char, TileDefinition>();
        foreach (var tile in tiles)
        {
            if (tile != null && !tileDict.ContainsKey(tile.character))
            {
                tileDict[tile.character] = tile;
            }
        }
    }
    
    public TileDefinition GetTileDefinition(char character)
    {
        if (tileDict == null) RefreshDictionary();
        
        tileDict.TryGetValue(character, out TileDefinition tile);
        return tile;
    }    
    
    public TileData GetTileData(char character)
    {
        var definition = GetTileDefinition(character);
        if (definition == null) return null;

        return new TileData
        {
            character = character,
            prefab = definition.prefab,
            tileType = definition.tileType,
            tileName = definition.GetTileName(),
        };
    }
    
    public TileDefinition[] GetAllTiles()
    {
        return tiles.Where(t => t != null).ToArray();
    }
    
    public void AddTile(TileDefinition tile)
    {
        if (tile != null && !tiles.Contains(tile))
        {
            tiles.Add(tile);
            RefreshDictionary();
        }
    }
    
    public void RemoveTile(TileDefinition tile)
    {
        if (tiles.Contains(tile))
        {
            tiles.Remove(tile);
            RefreshDictionary();
        }   
    }
    
}
