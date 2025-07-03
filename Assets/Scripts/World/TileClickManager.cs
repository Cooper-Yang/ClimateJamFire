using UnityEngine;

public class TileClickManager : MonoBehaviour
{
    public static TileClickManager Instance { get; private set; }
    private Firefighter activeFirefighter;
    
    // Column selection mode for Break Line ability
    private bool columnSelectionMode = false;
    private System.Action<int> onColumnSelectedCallback;

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
        
        // Check if we're in column selection mode
        if (columnSelectionMode)
        {
            Debug.Log($"Column {tile.gridX} selected for Break Line ability!");
            onColumnSelectedCallback?.Invoke(tile.gridX);
            return;
        }
        
        // Normal firefighter movement logic
        if (tile.IsTileType(TileType.Tree) && tile.IsWalkable() && activeFirefighter != null)
        {
            Debug.Log("Sending firefighter to tree!");
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
}
