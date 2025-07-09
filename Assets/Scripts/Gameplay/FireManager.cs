using UnityEngine;
using System.Collections.Generic;
using System;

public class FireManager : MonoBehaviour
{
    
    private GridManager gridManager;
    private List<Tile> smokeTiles;
    public List<Tile> fireTiles;
    public GameObject firePrefab;
    
    // Win/Lose and Score System
    private bool gameEnded = false;
    public bool hasWon = false;
    public bool hasLost = false;
    public int finalScore = 0;
    private int initialHouseCount = 0;
    private float lastHouseDestroyedTime = -1f;
    
    // Events for UI
    public static event Action<bool, int> OnGameEnded; // bool = hasWon, int = finalScore
    
    private void Awake()
    {
        gridManager = GetComponent<GridManager>();
    }

    private void Start()
    {
        // Store initial house count for lose condition checking
        initialHouseCount = gridManager.numberOfHouses;
        
        if (gridManager.state == GameState.Action)
        {
            StartFireSpread();
        }
    }

    private void StartFireSpread()
    {
        smokeTiles = gridManager.GetSmokeTiles();
        for (int i = smokeTiles.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            Tile temp = smokeTiles[i];
            smokeTiles[i] = smokeTiles[j];
            smokeTiles[j] = temp;
        }
        smokeTiles[0].OnFire(firePrefab);
        fireTiles.Add(smokeTiles[0]);
        smokeTiles[1].OnFire(firePrefab);
        fireTiles.Add(smokeTiles[1]);
    }

    private void Update()
    {
        if (!gameEnded)
        {
            CheckWinLoseConditions();
        }
    }
    
    private void CheckWinLoseConditions()
    {
        // Debug current state
        int currentHouses = CountCurrentHouses();
        bool anyFires = !AreNoFireTilesRemaining();
        
        Debug.Log($"Win/Lose Check - Initial Houses: {initialHouseCount}, Current Houses: {currentHouses}, Fires Active: {anyFires}");
        
        // Check lose condition first: all houses are burnt down (if there were houses initially)
        if (initialHouseCount > 0 && currentHouses == 0)
        {
            Debug.Log("LOSE CONDITION MET: All houses burnt down!");
            TriggerLoseState();
            return;
        }
        
        // Check win condition: no fire tiles remaining, but wait a frame after last house destroyed to avoid timing conflicts
        if (AreNoFireTilesRemaining())
        {
            // If a house was just destroyed this frame, wait before declaring victory
            if (currentHouses < initialHouseCount && Time.time - lastHouseDestroyedTime < 0.1f)
            {
                Debug.Log("Delaying win check - house was just destroyed");
                return;
            }
            
            Debug.Log("WIN CONDITION MET: No fires remaining!");
            TriggerWinState();
            return;
        }
    }
    
    private int CountCurrentHouses()
    {
        int houseCount = 0;
        for (int x = 0; x < gridManager.width; x++)
        {
            for (int z = 0; z < gridManager.height; z++)
            {
                Tile tile = gridManager.GetTileAt(x, z);
                if (tile != null && tile.IsTileType(TileType.House))
                {
                    houseCount++;
                }
            }
        }
        return houseCount;
    }
    
    private bool AreAllHousesBurnt()
    {
        Tile[,] tiles = GetAllTiles();
        if (tiles == null) return false;
        
        for (int x = 0; x < gridManager.width; x++)
        {
            for (int z = 0; z < gridManager.height; z++)
            {
                Tile tile = gridManager.GetTileAt(x, z);
                if (tile != null && tile.IsTileType(TileType.House))
                {
                    return false; // Found an intact house
                }
            }
        }
        return true; // No houses found, all burnt
    }
    
    private bool AreNoFireTilesRemaining()
    {
        for (int x = 0; x < gridManager.width; x++)
        {
            for (int z = 0; z < gridManager.height; z++)
            {
                Tile tile = gridManager.GetTileAt(x, z);
                if (tile != null && tile.IsBurning())
                {
                    return false; // Found fire
                }
            }
        }
        return true; // No fire found
    }
    
    private void TriggerWinState()
    {
        if (gameEnded) return;
        
        gameEnded = true;
        hasWon = true;
        hasLost = false;
        
        CalculateFinalScore();
        
        Debug.Log($"You Won! Final Score: {finalScore}");
        
        // Fire event for UI
        OnGameEnded?.Invoke(true, finalScore);
    }
    
    private void TriggerLoseState()
    {
        if (gameEnded) return;
        
        gameEnded = true;
        hasWon = false;
        hasLost = true;
        
        CalculateFinalScore();
        
        Debug.Log($"You Lost! Final Score: {finalScore}");
        
        // Fire event for UI
        OnGameEnded?.Invoke(false, finalScore);
    }
    
    private void CalculateFinalScore()
    {
        finalScore = 0;
        
        for (int x = 0; x < gridManager.width; x++)
        {
            for (int z = 0; z < gridManager.height; z++)
            {
                Tile tile = gridManager.GetTileAt(x, z);
                if (tile != null)
                {
                    // +1 for every intact Tree tile
                    if (tile.IsTileType(TileType.Tree))
                    {
                        finalScore += 1;
                    }
                    // +3 for every house saved
                    else if (tile.IsTileType(TileType.House))
                    {
                        finalScore += 3;
                    }
                }
            }
        }
    }
    
    private Tile[,] GetAllTiles()
    {
        // Helper method to access the tiles array from GridManager
        // This assumes GridManager has a way to access its tiles
        return null; // GridManager would need to expose its tiles array
    }
    
    // Public methods for external access
    public bool IsGameEnded()
    {
        return gameEnded;
    }
    
    public bool HasPlayerWon()
    {
        return hasWon;
    }
    
    public bool HasPlayerLost()
    {
        return hasLost;
    }
    
    public int GetFinalScore()
    {
        return finalScore;
    }
    
    // Method to be called when a house is destroyed
    public void OnHouseDestroyed()
    {
        lastHouseDestroyedTime = Time.time;
        Debug.Log($"House destroyed at time: {lastHouseDestroyedTime}");
    }
}
