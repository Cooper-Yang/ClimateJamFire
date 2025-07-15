using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static DG.Tweening.DOTweenAnimation;

public class Firefighter : MonoBehaviour
{
    public float moveTimePerTile = 1.5f;
    private float baseMoveTimePerTile; // Store original speed
    private Tile currentTile;
    private GridManager gmm;
    public float cutTime = 2.5f; // Time to cut a tree
    public bool hasFlameRetardantBuff = false;
    public bool usesFastExtinguish = false;

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
    public void Init(GridManager gridManager, Phase phase = Phase.PREP)
    {
        baseMoveTimePerTile = moveTimePerTile;
        currentTile = gridManager.GetTileAtCoord(transform.position);
        gmm = gridManager;

        if (currentTile == null)
        {
            Debug.LogError("Firefighter could not find its current tile in Init().");
            return;
        }

        if (phase == Phase.PREP)
        {
            HighlightCuttableTrees();
        }
    }

    private void HighlightCuttableTrees()
    {
        Debug.Log("HighlightCuttableTrees called");
        GridManager gm = FindAnyObjectByType<GridManager>();
        Tile fireStation = currentTile;

        foreach (Tile tile in FindObjectsByType<Tile>(FindObjectsSortMode.None))
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

        // Play tree chop sound when firefighter starts chopping
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayTreeChopSound();
        }

        yield return new WaitForSeconds(cutTime);

        if (!target.IsTileType(TileType.Tree))
        {
            Debug.LogWarning("ReplaceTileWithPlain called on invalid tile.");
        }
        else
        {
            gmm.ReplaceTileWithPlain(target);
            gmm.numberOfTreesCutDownToPlains++;
            gmm.numberOfRemainingTree--;

            // Play despawn sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayDespawnSound();
            }
            
            Destroy(gameObject);
        }
    }

    // Speed Boost Methods
    public void ApplySpeedBoost(float speedMultiplier)
    {
        moveTimePerTile = baseMoveTimePerTile / (1 + speedMultiplier);
        Debug.Log($"Speed boost applied! Move time reduced from {baseMoveTimePerTile} to {moveTimePerTile}");
    }

    public void ResetSpeed()
    {
        moveTimePerTile = baseMoveTimePerTile;
        Debug.Log($"Speed reset to normal: {moveTimePerTile}");
    }

    public void BeginFirefightingMode()
    {
        Tile smokeTarget = FindClosestSmokeTile();
        if (smokeTarget != null)
        {
            StartCoroutine(MoveToAndExtinguish(smokeTarget));
        }
        else
        {
            Debug.Log("No smoke tile found for firefighter to move to.");
            
            // Play despawn sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayDespawnSound();
            }
            
            Destroy(gameObject);
        }
    }

    private Tile FindClosestSmokeTile()
    {
        List<Tile> smokeTiles = gmm.GetSmokeTiles();

        if (smokeTiles == null || smokeTiles.Count == 0)
        {
            Debug.Log("No smoke tiles found in grid manager.");
            return null;
        }

        Tile closest = null;
        float minDist = float.MaxValue;

        foreach (Tile smoke in smokeTiles)
        {
            Debug.Log($"Checking smoke tile at ({smoke.gridX}, {smoke.gridZ}) | isBurning: {smoke.IsBurning()}");

            if (!smoke.IsBurning()) continue;
            if (!Pathfinding.Exists(currentTile, smoke)) continue;

            float dist = Vector3.Distance(transform.position, smoke.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = smoke;
            }
        }
        return closest;
    }

    private IEnumerator MoveToAndExtinguish(Tile target)
    {
        Transform targetTransform = target != null ? target.transform : null;

        List<Tile> path = Pathfinding.FindPath(currentTile, target);
        foreach (Tile step in path)
        {
            if (step == null || step.gameObject == null)
            {
                Debug.LogWarning("Firefighter path interrupted – step destroyed.");
                
                // Play despawn sound
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayDespawnSound();
                }
                
                Destroy(gameObject);
                yield break;
            }

            transform.position = step.transform.position;
            currentTile = step;
            yield return new WaitForSeconds(moveTimePerTile);
        }

        // Play extinguish sound when firefighter starts extinguishing
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayExtinguishSound();
        }

        if (hasFlameRetardantBuff) usesFastExtinguish = true;
        float extinguishTime = usesFastExtinguish ? 5f : 10f;
        yield return new WaitForSeconds(extinguishTime);

        if (target == null || targetTransform == null || targetTransform.gameObject == null)
        {
            Debug.LogWarning("Firefighter's target was destroyed mid-extinguish.");
            
            // Play despawn sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayDespawnSound();
            }
            
            Destroy(gameObject);
            yield break;
        }
        
        if (target.IsBurning())
        {
            gmm.ReplaceTileWithTree(target);
        }

        // Play despawn sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDespawnSound();
        }

        Destroy(gameObject);
    }
}