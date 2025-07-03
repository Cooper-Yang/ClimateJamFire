using UnityEngine;

public class TileClickManager : MonoBehaviour
{
    public static TileClickManager Instance { get; private set; }
    private Firefighter activeFirefighter;
    
    // Column selection mode for Break Line ability
    private bool columnSelectionMode = false;
    private System.Action<int> onColumnSelectedCallback;

    private bool tileSelectionMode = false;
    private System.Action<Tile> onTileSelectedCallback;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
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
            onColumnSelectedCallback?.Invoke(tile.gridX);
            return;
        }

        if (tileSelectionMode)
        {
            onTileSelectedCallback?.Invoke(tile);
            return;
        }

        if (tile.IsTileType(TileType.Tree) && tile.IsWalkable() && activeFirefighter != null)
        {
            activeFirefighter.MoveToAndCut(tile);
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
        
        if (enabled)
        {
            Debug.Log("Column selection mode enabled. Click on any tile in the column you want to clear.");
            HighlightAllColumns();
        }
        else
        {
            Debug.Log("Column selection mode disabled.");
            ClearAllHighlights();
        }
    }

    private void HighlightAllColumns()
    {
        // Highlight all tiles to indicate they're selectable for column selection
        foreach (Tile t in FindObjectsByType<Tile>(FindObjectsSortMode.None))
        {
            t.Highlight(true);
        }
    }

    public void OnTileSelectionMode(bool enabled, System.Action<Tile> callback)
    {
        tileSelectionMode = enabled;
        onTileSelectedCallback = callback;

        if (enabled)
        {
            Debug.Log("Tile selection mode enabled. Click on a tile to confirm target.");
            HighlightAllTiles();
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
