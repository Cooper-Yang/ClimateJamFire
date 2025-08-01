using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Analytics;
using System.Collections;

public class FireManager : MonoBehaviour
{
    public static FireManager Instance { get; private set; }
    private GridManager gridManager;
    private List<Tile> smokeTiles;
    public List<Tile> fireTiles;
    public GameObject firePrefab;
    public PhaseManager phaseManager;
    // Win/Lose and Score System
    private bool gameEnded = false;
    public bool hasWon = false;
    public bool hasLost = false;
    public int finalScore = 0;
    private int initialHouseCount = 0;
    private float lastHouseDestroyedTime = -1f;
    public int burnedTileCount = 0;


    // Events for UI
    public static event Action<bool, int> OnGameEnded; // bool = hasWon, int = finalScore

    private void Awake()
    {
        gridManager = GetComponent<GridManager>();
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("More than one FireManager in the scene!");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Store initial house count for lose condition checking
        initialHouseCount = gridManager.numberOfHouses;

        if (phaseManager.currentPhase == Phase.ACTION)
        {
            StartFireSpread();
        }
    }

    public void StartFireSpread()
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
        gridManager.ReplaceTileWithTree(smokeTiles[2]);
    }

    public void NotifyTileBurnedDown(Tile tile)
    {
        burnedTileCount++;
    }

    private bool isCheckingConditions = false; // Flag to control delayed checks

    private void Update()
    {
        if (!gameEnded && phaseManager.currentPhase == Phase.ACTION && !isCheckingConditions)
        {
            StartCoroutine(DelayedConditionCheck());
        }
    }

    private IEnumerator DelayedConditionCheck()
    {
        isCheckingConditions = true; // Prevent multiple coroutines from starting
        yield return new WaitForSeconds(2f); // Add a 2-second delay (adjust as needed)
        CheckWinLoseConditions();
        isCheckingConditions = false; // Reset flag after check
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
        Tile[,] tiles = gridManager.tiles;
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
