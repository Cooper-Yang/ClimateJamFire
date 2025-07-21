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
    private Material[] originalMaterials;
    public Material highlightMaterial;
    private Renderer tileRenderer;
    public bool isBurning = false;
    private GameObject fireObject;
    internal GridManager gridManager;

    private void Start()
    {
        InitializeMaterials();
    }

    private void Awake()
    {
        InitializeMaterials();
    }

    private void InitializeMaterials()
    {
        if (tileRenderer == null)
        {
            tileRenderer = GetComponent<Renderer>();
        }
        
        if (originalMaterials == null && tileRenderer != null)
        {
            originalMaterials = tileRenderer.materials;
        }
    }

    public void Highlight(bool active)
    {
        if (tileRenderer == null)
        {
            tileRenderer = GetComponent<Renderer>();
        }

        // Ensure originalMaterials is initialized
        if (originalMaterials == null && tileRenderer != null)
        {
            originalMaterials = tileRenderer.materials;
        }

        // Safety check to prevent null reference errors
        if (tileRenderer == null || originalMaterials == null)
        {
            Debug.LogWarning($"Tile ({gridX}, {gridZ}): Cannot highlight - missing Renderer or original materials");
            return;
        }

        if (active)
        {
            Material[] highlightMats = new Material[tileRenderer.materials.Length];
            for (int i = 0; i < highlightMats.Length; i++)
            {
                highlightMats[i] = highlightMaterial;
            }
            tileRenderer.materials = highlightMats;
        }
        else
        {
            // Create a copy of originalMaterials to avoid null assignment
            Material[] materialsToSet = new Material[originalMaterials.Length];
            for (int i = 0; i < originalMaterials.Length; i++)
            {
                materialsToSet[i] = originalMaterials[i];
            }
            tileRenderer.materials = materialsToSet;
        }
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
        
        // Play fire burst sound when tile catches fire
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayFireBurstSound();
        }
        
        if (definition.tileType == TileType.Tree)
        {
            gridManager.ReplaceTileWithSmoke(this);
            return;
        }
        
        fireObject = Instantiate(firePrefab, transform);
        StartCoroutine(SpreadFireAfterDelay(firePrefab));
        StartCoroutine(FireTurnTileToPlain());
    }

    private IEnumerator FireTurnTileToPlain()
    {
        yield return new WaitForSeconds(20f); // Wait 15 seconds
        if (fireObject != null)
        {
            DestroyImmediate(fireObject);
            if (FireManager.Instance != null)
            {
                FireManager.Instance.NotifyTileBurnedDown(this);
            }
            if (definition.tileType == TileType.Tree)
            {
                gridManager.ReplaceTileWithPlain(this, gridManager.burnedTilePrefab);
            }
            else if (definition.tileType == TileType.House)
            {
                gridManager.ReplaceTileWithPlain(this, gridManager.grassTilePrefab);
            }
            else
            {
                gridManager.ReplaceTileWithPlain(this, gridManager.plainTilePrefab);
            }
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
