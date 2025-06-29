using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static DG.Tweening.DOTweenAnimation;

public class Firefighter : MonoBehaviour
{
    public float moveTimePerTile = 1.5f;
    private Tile currentTile;
    private GridManager gmm;
    public float cutTime = 2.5f; // Time to cut a tree

    /*private void Start()
    {
        GridManager gm = FindObjectOfType<GridManager>();
        currentTile = gm.GetTileAtCoord(transform.position);
        //HighlightCuttableTrees();
        TileClickManager.Instance.SetActiveFirefighter(this);
        if (currentTile == null)
        {
            Debug.Log("Firefighter could not find its current tile.");
        }
        else
        {
            Debug.Log($"Firefighter current tile: ({currentTile.gridX}, {currentTile.gridZ})");
        }
    }
    */
    public void Init(GridManager gridManager)
    {
        currentTile = gridManager.GetTileAtCoord(transform.position);
        gmm = gridManager;

        if (currentTile == null)
        {
            Debug.LogError("Firefighter could not find its current tile in Init().");
            return;
        }

        HighlightCuttableTrees();
        TileClickManager.Instance.SetActiveFirefighter(this);
    }

    private void HighlightCuttableTrees()
    {
        Debug.Log("HighlightCuttableTrees called");
        GridManager gm = FindObjectOfType<GridManager>();
        Tile fireStation = currentTile;

        foreach (Tile tile in FindObjectsOfType<Tile>())
        {
            if (tile.IsTileType(TileType.Tree) )
            {
                List<Tile> neighbors = gm.GetAdjacentTiles(tile);
                bool hasPlainNeighbor = neighbors.Exists(n => n.IsTileType(TileType.Plain));

                if (hasPlainNeighbor && Pathfinding.Exists(fireStation, tile)) 
                {
                    tile.Highlight(true);
                }
            }
        }
    }

    public void MoveToAndCut(Tile targetTile)
    {
        StartCoroutine(MoveToTreeAndCut(targetTile));
    }

    IEnumerator MoveToTreeAndCut(Tile target)
    {
        Debug.Log($"Firefighter moving to tile ({target.gridX}, {target.gridZ})");
        List<Tile> path = Pathfinding.FindPath(currentTile, target); 

        foreach (Tile step in path)
        {
            transform.position = step.transform.position;
            yield return new WaitForSeconds(moveTimePerTile);
        }

        yield return new WaitForSeconds(cutTime); 

        //target.type = TileType.Plain;
        //target.Highlight(false);
        gmm.ReplaceTileWithPlain(target);

        Destroy(gameObject);
    }
}