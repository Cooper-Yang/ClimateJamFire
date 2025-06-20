using UnityEngine;

public enum TileType
{
    Mountain,
    Tree,
    Smoke,
    Plains,
    House,
    FireStation,
    River,
}

public class Tile : MonoBehaviour
{
    public int gridX;
    public int gridZ;
    public float cellSize;
    public GameplayTileDefinition definition;

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
    }
}
