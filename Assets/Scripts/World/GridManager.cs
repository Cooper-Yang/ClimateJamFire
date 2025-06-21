using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class GridManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public int width = 10;
    public int height = 10;
    public const float cellSize = 2.56f;
    public float cellGapX = 0.1f;
    public float cellGapZ = 0.1f;
    private Tile[,] tiles;


    [Header("Level System")]
    public LevelData currentLevel;

    [Header("Gameplay Data System")]
    public GameplayTileDatabase gameplayDatabase;    public void ClearGrid()
    {
#if UNITY_EDITOR
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
        // Mark the scene as dirty after clearing
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        EditorUtility.SetDirty(this);
#else
    foreach (Transform child in transform)
    {
        Destroy(child.gameObject);
    }
#endif
    }public void GenerateGrid()
    {
        if (currentLevel != null)
        {
            GenerateFromLevel(currentLevel);
        }
        else
        {
            GenerateDefaultGrid();
        }
        
#if UNITY_EDITOR
        // Mark the scene as dirty so changes are saved
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        // Mark this GameObject as dirty for undo/redo support
        EditorUtility.SetDirty(this);
#endif
    }
    
    public void GenerateFromLevel(LevelData level)
    {
        ClearGrid();
        
        string[] lines = level.levelLayout.Split('\n');
        width = level.width;
        height = lines.Length;
        tiles = new Tile[width, height];        for (int z = 0; z < height; z++)
        {
            if (z >= lines.Length) break;
            string line = lines[z].Trim();
            
            for (int x = 0; x < width && x < line.Length; x++)
            {
                char tileChar = line[x];
                TileData tileData = level.GetTileData(tileChar);
                
                if (tileData?.prefab != null)
                {
                    // Flip X-axis so left side of text layout matches left side of world space
                    CreateTile(width - 1 - x, z, tileData);
                }
            }
        }
    }      private void CreateTile(int x, int z, TileData tileData)
    {
        Vector3 localPos = new Vector3(
            x * (cellSize + cellGapX),
            0,
            z * (cellSize + cellGapZ));        
        GameObject tileGO = Instantiate(tileData.prefab, transform);
        tileGO.transform.localPosition = localPos;
        tileGO.name = $"{tileData.tileName} ({x},{z})";
        tileGO.transform.localScale = new Vector3(cellSize, 1f, cellSize);

        Tile tile = tileGO.GetComponent<Tile>();
        if (tile != null)
        {
            tile.gridX = x;
            tile.gridZ = z;
            tile.cellSize = cellSize;

            // NEW: Assign gameplay definition based on tile type
            tile.definition = gameplayDatabase.GetGameplayDefinition(tileData);
        }

        tiles[x, z] = tile;
        
#if UNITY_EDITOR
        // Register the created tile for undo operations
        Undo.RegisterCreatedObjectUndo(tileGO, "Create Tile");
#endif
    }
    
    public Tile GetTileAt(int x, int z)
    {
        if (x >= 0 && x < width && z >= 0 && z < height)
        { 
            return tiles[x, z]; 
        }
        return null;
    }

    public void GenerateDefaultGrid()
    {
        ClearGrid();
        tiles = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {            for (int z = 0; z < height; z++)
            {
                Vector3 localPos = new Vector3(
                    x * (cellSize + cellGapX),
                    0,
                    z * (cellSize + cellGapZ));
                      GameObject tileGO = Instantiate(tilePrefab, transform);
                tileGO.transform.localPosition = localPos;
                tileGO.name = $"Tile ({x},{z})";
                tileGO.transform.localScale = new Vector3(cellSize, 1f, cellSize);

#if UNITY_EDITOR
                // Register the created tile for undo operations
                Undo.RegisterCreatedObjectUndo(tileGO, "Create Default Tile");
#endif

                Tile tile = tileGO.GetComponent<Tile>();
                if (tile != null)
                {
                    tile.gridX = x;
                    tile.gridZ = z;
                    tile.cellSize = cellSize;
                }
                
                tiles[x, z] = tile;
            }
        }
    }
}
