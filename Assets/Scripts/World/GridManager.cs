using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class GridManager : MonoBehaviour
{


    public int width = 10;
    public int height = 10;
    public const float cellSize = 1f;
    public float cellGapX = 0.1f;
    public float cellGapZ = 0.1f;
    public int numberOfRemainingTree = 0; 
    public int numberOfTreesCutDownToPlains = 0;
    public int numberOfHouses = 0;
    internal Tile[,] tiles;


    [Header("Level System")]
    public LevelData currentLevel;
    [Header("Tile Replacement Prefabs")]
    public GameObject plainTilePrefab;
    public GameObject smokeTreeTilePrefab;
    public GameObject smokeHouseTilePrefab;
    public GameObject treeTilePrefab;
    public GameObject burnedTreeTilePrefab;
    public GameObject burnedHouseTilePrefab;
    public GameObject choppedTilePrefab;
    public GameObject grassTilePrefab;
    public GameObject houseTilePrefab;

    [Header("Gameplay Data System")]
    public GameplayTileDatabase gameplayDatabase;

    public int totalBurnableTiles { get; private set; }

    private void Start()
    {
        RebuildTileMapFromScene();

        totalBurnableTiles = 0;

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Tile tile = GetTileAt(x, z);
                if (tile != null && (tile.IsTileType(TileType.Tree) || tile.IsTileType(TileType.House)))
                {
                    totalBurnableTiles++;
                }
            }
        }
    }
    /*
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Debug.Log("You clicked on: " + hit.collider.gameObject.name);
            }
            else
            {
                Debug.Log("Clicked, but hit nothing.");
            }
        }
    }*/
     
    public void ClearGrid()
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
        numberOfRemainingTree = 0;
        numberOfTreesCutDownToPlains = 0;
        numberOfHouses = 0;

    }
    
    public void GenerateGrid()
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
        tiles = new Tile[width, height]; for (int z = 0; z < height; z++)
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
    }
    private void CreateTile(int x, int z, TileData tileData)

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
            tile.definition = gameplayDatabase.GetGameplayDefinition(tileData);
            tile.gridManager = this;
        }

        tiles[x, z] = tile;
        if (tile.IsTileType(TileType.House))
        {
            numberOfHouses++;
        }
        else if (tile.IsTileType(TileType.Tree))
        {
            numberOfRemainingTree++;
        }
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

                GameObject tileGO = Instantiate(plainTilePrefab, transform);
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
                    tile.gridManager = this;
                    tile.definition = gameplayDatabase.GetGameplayDefinition(TileType.Plain);
                }

                tiles[x, z] = tile;
            }
        }
    }

    public List<Tile> GetAdjacentTiles(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();
        int x = tile.gridX;
        int z = tile.gridZ;
        int[,] directions = new int[,] { { 0, 1 }, { 0, -1 }, { -1, 0 }, { 1, 0 } };
        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int dx = directions[i, 0];
            int dz = directions[i, 1];
            Tile neighbor = GetTileAt(x + dx, z + dz);
            if (neighbor != null)
            {
                neighbors.Add(neighbor);
            }
        }
        return neighbors;
    }

    public Tile GetTileAtCoord(Vector3 worldPosition)
    {
        Tile closestTile = null;
        float closestDistance = float.MaxValue;

        foreach (Tile tile in GetComponentsInChildren<Tile>())
        {
            float dist = Vector3.Distance(tile.transform.position, worldPosition);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestTile = tile;
            }
        }
        return closestTile;
    }

    public void RebuildTileMapFromScene()
    {
        numberOfRemainingTree = 0;
        numberOfTreesCutDownToPlains = 0;
        numberOfHouses = 0;
        tiles = new Tile[width, height];
        Tile[] allTiles = GetComponentsInChildren<Tile>();

        foreach (Tile tile in allTiles)
        {
            if (tile.gridX >= 0 && tile.gridX < width && tile.gridZ >= 0 && tile.gridZ < height)
            {
                tile.gridManager = this;
                tiles[tile.gridX, tile.gridZ] = tile;
                if (tile.IsTileType(TileType.House))
                {
                    numberOfHouses++;
                }
                else if (tile.IsTileType(TileType.Tree))
                {
                    numberOfRemainingTree++;
                }
            }
            else
            {
                Debug.LogWarning($"Tile at ({tile.gridX}, {tile.gridZ}) is out of bounds.");
            }
        }
    }

    public void ReplaceTileWithPlain(Tile tile, GameObject tilePrefab)
    {
        if (tile == null)
        {
            Debug.LogWarning("ReplaceTileWithPlain called on invalid tile.");
            return;
        }

        // Debug logging for house destruction
        if (tile.IsTileType(TileType.House))
        {
            numberOfHouses--;
            Debug.Log($"HOUSE DESTROYED! Houses remaining: {numberOfHouses}. Tile at ({tile.gridX}, {tile.gridZ}) was a house.");
            
            // Notify FireManager that a house was destroyed
            FireManager fireManager = FindFirstObjectByType<FireManager>();
            if (fireManager != null)
            {
                fireManager.OnHouseDestroyed();
            }
        }
        else if (tile.IsTileType(TileType.Tree))
        {
            numberOfRemainingTree--;
            numberOfTreesCutDownToPlains++;
        }

        int x = tile.gridX;
        int z = tile.gridZ;
        Vector3 pos = tile.transform.position;
        Quaternion rotation = tile.transform.localRotation;
        Destroy(tile.gameObject);

        GameObject newTileGO = Instantiate(tilePrefab, pos, rotation, transform);
        newTileGO.transform.localScale = new Vector3(GridManager.cellSize, 1f, GridManager.cellSize);

        Tile newTile = newTileGO.GetComponent<Tile>();
        if (newTile != null)
        {
            newTile.gridX = x;
            newTile.gridZ = z;
            newTile.definition = gameplayDatabase.GetGameplayDefinition(TileType.Plain);
            newTile.cellSize = GridManager.cellSize;
            newTile.gridManager = this;
            tiles[x, z] = newTile;
        }
        else
        {
            Debug.LogError("New Plain tile prefab missing Tile component.");
        }
    }

    public void ReplaceTileWithSmoke(Tile tile)
    {
        if (tile == null)
        {
            Debug.LogWarning("ReplaceTileWithSmoke called on null tile.");
            return;
        }

        int x = tile.gridX;
        int z = tile.gridZ;
        Vector3 pos = tile.transform.position;
        Quaternion rotation = tile.transform.localRotation;
        bool isTileHouse = tile.IsTileType(TileType.House);
        Destroy(tile.gameObject);

        GameObject newTileGO = Instantiate( isTileHouse? smokeHouseTilePrefab:smokeTreeTilePrefab, pos, rotation, transform);
        newTileGO.transform.localScale = new Vector3(cellSize, 1f, cellSize);

        Tile newTile = newTileGO.GetComponent<Tile>();
        if (newTile != null)
        {
            newTile.gridX = x;
            newTile.gridZ = z;
            newTile.definition = gameplayDatabase.GetGameplayDefinition(TileType.Smoke);
            newTile.cellSize = cellSize;
            newTile.gridManager = this;
            newTile.isHouse = isTileHouse;
            tiles[x, z] = newTile;

            newTile.OnFire(firePrefab: FindFirstObjectByType<FireManager>().firePrefab);
        }
        else
        {
            Debug.LogError("New SmokeTree tile prefab missing Tile component.");
        }
    }
    public void ReplaceTileWithTree(Tile tile)
    {
        if (tile == null)
        {
            Debug.LogWarning("ReplaceTileWithTree called on invalid tile.");
            return;
        }

        int x = tile.gridX;
        int z = tile.gridZ;
        Vector3 pos = tile.transform.position;
        Quaternion rotation = tile.transform.localRotation;
        Destroy(tile.gameObject);

        GameObject newTileGO = Instantiate(treeTilePrefab, pos, rotation, transform);
        newTileGO.transform.localScale = new Vector3(cellSize, 1f, cellSize);

        Tile newTile = newTileGO.GetComponent<Tile>();
        if (newTile != null)
        {
            newTile.gridX = x;
            newTile.gridZ = z;
            newTile.definition = gameplayDatabase.GetGameplayDefinition(TileType.Tree);
            newTile.cellSize = cellSize;
            newTile.gridManager = this;
            tiles[x, z] = newTile;
            numberOfRemainingTree++;
            
        }
        else
        {
            Debug.LogError("New Tree tile prefab missing Tile component.");
        }
    }

    public void ReplaceTileWithHouse(Tile tile)
    {
        if (tile == null)
        {
            Debug.LogWarning("ReplaceTileWithTree called on invalid tile.");
            return;
        }

        int x = tile.gridX;
        int z = tile.gridZ;
        Vector3 pos = tile.transform.position;
        Quaternion rotation = tile.transform.localRotation;
        Destroy(tile.gameObject);

        GameObject newTileGO = Instantiate(houseTilePrefab, pos, rotation, transform);
        newTileGO.transform.localScale = new Vector3(cellSize, 1f, cellSize);

        Tile newTile = newTileGO.GetComponent<Tile>();
        if (newTile != null)
        {
            newTile.gridX = x;
            newTile.gridZ = z;
            newTile.definition = gameplayDatabase.GetGameplayDefinition(TileType.House);
            newTile.cellSize = cellSize;
            newTile.gridManager = this;
            tiles[x, z] = newTile;
            //numberOfHouses++;
        }
        else
        {
            Debug.LogError("New Tree tile prefab missing Tile component.");
        }
    }

    public Tile GetFireStationTile()
    {
        foreach (Tile tile in tiles)
        {
            if (tile.IsTileType(TileType.FireStation))
            {
                return tile;
            }
        }
        return null;
    }

    public List<Tile> GetSmokeTiles()
    {
        List<Tile> smokeTiles = new List<Tile>();
        foreach (Tile tile in tiles)
        {
            if (tile.IsTileType(TileType.Smoke))
            {
                smokeTiles.Add(tile);
            }
        }
        return smokeTiles;
    }

    public List<Tile> GetTilesInSquare(int centerX, int centerZ, int size)
    {
        List<Tile> tiles = new List<Tile>();
        for (int dx = 0; dx < size; dx++)
        {
            for (int dz = 0; dz < size; dz++)
            {
                Tile t = GetTileAt(centerX + dx, centerZ + dz);
                if (t != null) tiles.Add(t);
            }
        }
        return tiles;
    }
}
