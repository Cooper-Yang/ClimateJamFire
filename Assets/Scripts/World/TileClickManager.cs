using UnityEngine;

public class TileClickManager : MonoBehaviour
{
    public static TileClickManager Instance { get; private set; }
    private Firefighter activeFirefighter;

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
        foreach (Tile t in FindObjectsOfType<Tile>())
        {
            t.Highlight(false);
        }
    }
}
