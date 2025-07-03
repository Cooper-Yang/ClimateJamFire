using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum TileType { Plain, Tree, FireStation, Mountain, House, Smoke, River }


public class Tile : MonoBehaviour
{
    public int gridX;
    public int gridZ;
    internal float cellSize;
    public GameplayTileDefinition definition;
    private Material originalMaterial;
    public Material highlightMaterial;
    private Renderer tileRenderer;
    public bool isBurning = false;
    private GameObject fireObject;
    internal GridManager gridManager;

    private void Start()
    {
        tileRenderer = GetComponent<Renderer>();
        originalMaterial = tileRenderer.material;
    }

    public void Highlight(bool active)
    {
        if (tileRenderer == null)
        {
            tileRenderer = GetComponent<Renderer>();
        }
        tileRenderer.material = active ? highlightMaterial : originalMaterial;
    }

    private void OnMouseDown()
    {
        //Debug.Log($"Tile clicked: ({gridX}, {gridZ})");
        // You can call PlaceTower() here for testing
        
        Debug.Log($"Tile clicked: ({gridX}, {gridZ}) � Type: {definition.tileType}");
        if (TileClickManager.Instance != null)
        {
            TileClickManager.Instance.OnTileClicked(this);
        }
    }

    public void OnFire(GameObject firePrefab)
    {
        if (!definition.canBurn)
        {
            Debug.Log("Burn a unburnable tile :  ({gridX},{gridZ})");
            return;
        }
        isBurning = true;
        fireObject = Instantiate(firePrefab, transform);
        StartCoroutine(SpreadFireAfterDelay(firePrefab));
        StartCoroutine(FireTurnTileToPlain());
    }

    private IEnumerator FireTurnTileToPlain()
    {
        yield return new WaitForSeconds(15f); // Wait 15 seconds
        if (fireObject != null)
        {
            DestroyImmediate(fireObject);
            gridManager.ReplaceTileWithPlain(this);
        }
    }

    private IEnumerator SpreadFireAfterDelay(GameObject firePrefab)
    {
        yield return new WaitForSeconds(10f); // Wait 10 seconds

        if (isBurning)
        {
            List<Tile> neighbors = gridManager.GetAdjacentTiles(this);

            foreach (Tile neighbor in neighbors)
            {
                if (neighbor.IsBurnable() && !neighbor.isBurning)
                {
                    neighbor.OnFire(firePrefab);
                }
            }
        }
    }

    public bool IsWalkable()
    {
        if (definition == null)
        {

            return false;
        }
        return definition.isWalkable;
    }

    public bool IsTileType(TileType type)
    {
        if (definition == null)
        {

            return false;
        }
        return definition.tileType == type;
    }

    public bool IsBurnable()
    {
        return definition.canBurn;
    }

    public bool IsBurning()
    {
        return isBurning;
    }

}
