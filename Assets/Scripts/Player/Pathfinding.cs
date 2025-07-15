using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance;

    private void Awake()
    {
        Instance = this;
    }

    public static List<Tile> FindPath(Tile startTile, Tile targetTile)
    {
        List<Tile> openSet = new List<Tile>();
        HashSet<Tile> closedSet = new HashSet<Tile>();
        Dictionary<Tile, Tile> cameFrom = new Dictionary<Tile, Tile>();

        Dictionary<Tile, float> gScore = new Dictionary<Tile, float>();
        Dictionary<Tile, float> fScore = new Dictionary<Tile, float>();

        openSet.Add(startTile);
        gScore[startTile] = 0;
        fScore[startTile] = Heuristic(startTile, targetTile);

        while (openSet.Count > 0)
        {
            Tile current = GetLowestFScoreTile(openSet, fScore);

            if (current == targetTile)
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (Tile neighbor in GetNeighbors(current))
            {
                if (!neighbor.definition.isWalkable || closedSet.Contains(neighbor))
                    continue;

                float tentativeG = gScore[current] + 1;

                if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    fScore[neighbor] = tentativeG + Heuristic(neighbor, targetTile);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return new List<Tile>();
    }

    private static Tile GetLowestFScoreTile(List<Tile> openSet, Dictionary<Tile, float> fScore)
    {
        Tile bestTile = openSet[0];
        float bestScore = fScore.ContainsKey(bestTile) ? fScore[bestTile] : Mathf.Infinity;

        foreach (Tile tile in openSet)
        {
            float score = fScore.ContainsKey(tile) ? fScore[tile] : Mathf.Infinity;
            if (score < bestScore)
            {
                bestTile = tile;
                bestScore = score;
            }
        }

        return bestTile;
    }

    private static float Heuristic(Tile a, Tile b)
    {
        return Mathf.Abs(a.gridX - b.gridX) + Mathf.Abs(a.gridZ - b.gridZ);
    }

    private static List<Tile> GetNeighbors(Tile tile)
    {
        List<Tile> neighbors = new List<Tile>();
        GridManager gm = FindFirstObjectByType<GridManager>();

        int[,] directions = new int[,] { { 0, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };

        for (int i = 0; i < directions.GetLength(0); i++)
        {
            int newX = tile.gridX + directions[i, 0];
            int newZ = tile.gridZ + directions[i, 1];

            Tile neighbor = gm.GetTileAt(newX, newZ);
            if (neighbor != null)
                neighbors.Add(neighbor);
        }

        return neighbors;
    }

    private static List<Tile> ReconstructPath(Dictionary<Tile, Tile> cameFrom, Tile current)
    {
        List<Tile> path = new List<Tile> { current };

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            path.Add(current);
        }

        path.Reverse();
        return path;
    }

    public static bool Exists(Tile start, Tile end)
    {
        return FindPath(start, end).Count > 0;
    }
}
