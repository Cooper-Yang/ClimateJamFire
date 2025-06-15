using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public int width = 10;
    public int height = 10;
    public float cellSize = 1f;

    private Tile[,] tiles;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        tiles = new Tile[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                Vector3 pos = new Vector3(x * cellSize, 0, z * cellSize);
                GameObject tileGO = Instantiate(tilePrefab, pos, Quaternion.identity, transform);
                tileGO.name = $"Tile ({x},{z})";

                Tile tile = tileGO.GetComponent<Tile>();
                tile.gridX = x;
                tile.gridZ = z;
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
