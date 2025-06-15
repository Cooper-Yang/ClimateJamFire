using UnityEngine;
using UnityEditor;

public class GridManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;
    public float cellGapX = 0.1f;
    public float cellGapZ = 0.1f;
    private Tile[,] tiles;


    public void ClearGrid()
    {
#if UNITY_EDITOR
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
#else
    foreach (Transform child in transform)
    {
        Destroy(child.gameObject);
    }
#endif
    }

    public void GenerateGrid()
    {
        ClearGrid();
        tiles = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                float offsetX = 0.5f * (cellSize + cellGapX);
                float offsetZ =  0.5f * (cellSize + cellGapZ);
                Vector3 pos = new Vector3(
                    x * (cellSize + cellGapX) + offsetX,
                    0,
                    z * (cellSize + cellGapZ) + offsetZ);
                GameObject tileGO = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
                tileGO.name = $"Tile ({x},{z})";
                tileGO.transform.localScale = new Vector3(cellSize, 1f, cellSize);

                Tile tile = tileGO.GetComponent<Tile>();
                tile.gridX = x;
                tile.gridZ = z;
                tile.cellSize = cellSize;
                tile.isWalkable = true; // set default or path logic here
                
                tiles[x, z] = tile;
            }
        }
    }

    public Tile GetTileAt(int x, int z)
    {
        if (x >= 0 && x < width && z >= 0 && z < height)
        { 
            return tiles[x, z]; 
        }
        return null;
    }


}
