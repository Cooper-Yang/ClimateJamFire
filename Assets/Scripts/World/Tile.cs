using UnityEngine;

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
//     public void PlaceTower(GameObject towerPrefab)
//     {
//         if (hasTower)
//         {
//             return;
//         }
//         //Instantiate(towerPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
//         hasTower = true;
//     }


    private void OnMouseDown()
    {
        Debug.Log($"Tile clicked: ({gridX}, {gridZ})");
        // You can call PlaceTower() here for testing
        
        Debug.Log($"Tile clicked: ({gridX}, {gridZ}) � Type: {definition.tileType}");
        if (TileClickManager.Instance != null)
        {
            TileClickManager.Instance.OnTileClicked(this);
        }
    }

    public bool IsWalkable()
    {
        return definition.isWalkable;
    }

    public bool IsTileType(TileType type)
    {
        return definition.tileType == type;
    }

    public bool IsBurnable()
    {
        return definition.canBurn;
    }
}
