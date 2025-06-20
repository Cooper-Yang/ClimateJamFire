using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameplayTileDatabase", menuName = "Gameplay System/Gameplay Tile Database")]
public class GameplayTileDatabase : ScriptableObject
{
 //   [Header("Visual Tiles")]
//    public List<TileDefinition> visualTiles = new List<TileDefinition>();

//    [Header("Gameplay Definitions")]
//    public List<GameplayTileDefinition> gameplayDefinitions = new List<GameplayTileDefinition>();

    [Header("Type to Gameplay Mapping")]
    public List<TileTypeMapping> typeMappings = new List<TileTypeMapping>();

 //   private Dictionary<char, TileDefinition> visualDict;
    private Dictionary<TileType, GameplayTileDefinition> gameplayDict;

    void OnEnable()
    {
        RefreshDictionaries();
    }

    public void RefreshDictionaries()
    {
        // Visual dictionary (existing)
//         visualDict = new Dictionary<char, TileDefinition>();
//         foreach (var tile in visualTiles)
//         {
//             if (tile != null && !visualDict.ContainsKey(tile.character))
//             {
//                 visualDict[tile.character] = tile;
//             }
//         }

        // Gameplay dictionary (new)
        gameplayDict = new Dictionary<TileType, GameplayTileDefinition>();
        foreach (var mapping in typeMappings)
        {
            if (mapping.gameplayDefinition != null && !gameplayDict.ContainsKey(mapping.tileType))
            {
                gameplayDict[mapping.tileType] = mapping.gameplayDefinition;
            }
        }
    }

    public GameplayTileDefinition GetGameplayDefinition(TileType tileType)
    {
        if (gameplayDict == null) RefreshDictionaries();
        gameplayDict.TryGetValue(tileType, out GameplayTileDefinition definition);
        return definition;
    }

    public GameplayTileDefinition GetGameplayDefinition(TileData tileData)
    {
        return GetGameplayDefinition(tileData.tileType);
    }
}

[System.Serializable]
public class TileTypeMapping
{
    public TileType tileType;
    public GameplayTileDefinition gameplayDefinition;
}