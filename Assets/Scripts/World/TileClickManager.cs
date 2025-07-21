using UnityEngine;

public class TileClickManager : MonoBehaviour
{
    public static TileClickManager Instance { get; private set; }
    private Firefighter activeFirefighter;
    [SerializeField] public PhaseManager phaseManager;

    // Column selection mode for Break Line ability
    private bool columnSelectionMode = false;
    private System.Action<int> onColumnSelectedCallback;
    private int lastHighlightedColumn = -1; // Track the last highlighted column

    private bool tileSelectionMode = false;
    private System.Action<Tile> onTileSelectedCallback;
    private Tile lastHighlightedTile = null; // Track the last highlighted tile for water tanker

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void Update()
    {
        // Handle column highlighting on mouse hover during break line mode
        if (columnSelectionMode)
        {
            HandleColumnHover();
        }
        // Handle 2x2 area highlighting on mouse hover during water tanker mode
        else if (tileSelectionMode)
        {
            HandleTileHover();
        }
    }

    private void HandleColumnHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Tile hoveredTile = hit.collider.GetComponent<Tile>();
            if (hoveredTile != null)
            {
                int hoveredColumn = hoveredTile.gridX;

                // Only update highlighting if we're hovering over a different column
                if (hoveredColumn != lastHighlightedColumn)
                {
                    ClearAllHighlights();
                    HighlightColumn(hoveredColumn);
                    lastHighlightedColumn = hoveredColumn;
                }
            }
        }
    }

    private void HandleTileHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Tile hoveredTile = hit.collider.GetComponent<Tile>();
            if (hoveredTile != null)
            {
                // Only update highlighting if we're hovering over a different tile
                if (hoveredTile != lastHighlightedTile)
                {
                    ClearAllHighlights();
                    HighlightWaterTankerArea(hoveredTile);
                    lastHighlightedTile = hoveredTile;
                }
            }
        }
    }

    private void HighlightColumn(int columnX)
    {
        GridManager gridManager = FindFirstObjectByType<GridManager>();
        if (gridManager == null) return;

        // Highlight only tree tiles in the specified column that can be cut
        for (int z = 0; z < gridManager.height; z++)
        {
            Tile tile = gridManager.GetTileAt(columnX, z);
            if (tile != null && tile.IsTileType(TileType.Tree) && !tile.IsBurning())
            {
                tile.Highlight(true);
            }
        }
    }

    private void HighlightWaterTankerArea(Tile centerTile)
    {
        GridManager gridManager = FindFirstObjectByType<GridManager>();
        if (gridManager == null) return;

        // Highlight 2x2 area starting from the clicked tile
        for (int dx = 0; dx < 2; dx++)
        {
            for (int dz = 0; dz < 2; dz++)
            {
                Tile tile = gridManager.GetTileAt(centerTile.gridX + dx, centerTile.gridZ + dz);
                if (tile != null)
                {
                    // Highlight burning tiles with a different visual indicator
                    // or highlight all tiles to show the area of effect
                    tile.Highlight(true);
                }
            }
        }
    }

    

    public void SetActiveFirefighter(Firefighter ff)
    {
        activeFirefighter = ff;
    }

    public void OnTileClicked(Tile tile)
    {
        Debug.Log($"TileClickManager received click on ({tile.gridX}, {tile.gridZ})");

        if (columnSelectionMode)
        {
            // Clear highlights when column is selected
            ClearAllHighlights();
            onColumnSelectedCallback?.Invoke(tile.gridX);
            return;
        }

        if (tileSelectionMode)
        {
            // Clear highlights when tile is selected for water tanker
            ClearAllHighlights();
            onTileSelectedCallback?.Invoke(tile);
            return;
        }

        if (tile.IsTileType(TileType.Tree) && tile.IsWalkable() && activeFirefighter != null && phaseManager.currentPhase == Phase.PREP)
        {
            activeFirefighter.MoveToAndCut(tile);

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayFirefighterSpawnSound();
            }

            ClearAllHighlights();
            activeFirefighter = null;
        }

        else if (tile.IsTileType(TileType.Smoke) && tile.IsWalkable() && activeFirefighter != null && phaseManager.currentPhase == Phase.ACTION)
        {
            activeFirefighter.BeginFirefightingMode(tile);

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayFirefighterSpawnSound();
            }

            ClearAllHighlights();
            activeFirefighter = null;
        }
    }

    public void ClearAllHighlights()
    {
        foreach (Tile t in FindObjectsByType<Tile>(FindObjectsSortMode.None))
        {
            t.Highlight(false);
        }
    }

    // Column Selection Mode for Break Line Ability
    public void OnColumnSelectionMode(bool enabled, System.Action<int> callback)
    {
        columnSelectionMode = enabled;
        onColumnSelectedCallback = callback;
        lastHighlightedColumn = -1; // Reset tracking

        if (enabled)
        {
            Debug.Log("Column selection mode enabled. Hover over tiles to see which column will be cleared.");
        }
        else
        {
            Debug.Log("Column selection mode disabled.");
            ClearAllHighlights();
        }
    }

    public void OnTileSelectionMode(bool enabled, System.Action<Tile> callback)
    {
        tileSelectionMode = enabled;
        onTileSelectedCallback = callback;
        lastHighlightedTile = null; // Reset tracking

        if (enabled)
        {
            Debug.Log("Tile selection mode enabled. Hover over tiles to see 2x2 area, click to confirm target.");
        }
        else
        {
            Debug.Log("Tile selection mode disabled.");
            ClearAllHighlights();
        }
    }

    private void HighlightAllTiles()
    {
        foreach (Tile t in FindObjectsByType<Tile>(FindObjectsSortMode.None))
        {
            t.Highlight(true);
        }
    }
}
