using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class ActionPoint : MonoBehaviour
{
    //AP Max should always be 10
    //[SerializeField] public int currentActionPoint = 10;
    [Header("Action Point Variables")]
    public float actionPointRegenTime = 6.0f;
    public Slider actionPointSlider;
    private bool regenActive = true;
    public GameObject firefighterPrefab;
    public GridManager gridManager;
    private Transform fireStationTransform;

    // Speed Boost Ability Variables
    [Header("Speed Boost Ability")]
    public int speedBoostCost = 3;
    public float speedBoostDuration = 10.0f;
    public float speedBoostMultiplier = 0.5f; // 50% speed increase
    private bool speedBoostActive = false;

    // Break Line Ability Variables
    [Header("Break Line Ability")]
    public int breakLineCost = 8;
    private bool breakLineActive = false;

    [Header("Flame Retardant Ability")]
    public int flameRetardantCost = 4;
    public float flameRetardantDuration = 10f;
    private bool flameRetardantActive = false;

    [Header("Water Tanker Ability")]
    public int waterTankerCost = 6;
    private bool waterTankerActive = false;

    [Header("Phase Transition")]
    [SerializeField] public PhaseManager phaseManager; 

    public int currentActionPoint
    {
        get
        {
            return Mathf.FloorToInt(actionPointSlider.value);
        }
        set
        {

        }
    }

    private void Start()
    {
#if UNITY_EDITOR
        if (gridManager == null)
        {
            EditorUtility.DisplayDialog("Warning", "The grid manager in the Action Point is null", "OK");
            return;
        }

#endif
        Tile fireStationTile = gridManager.GetFireStationTile();
#if UNITY_EDITOR
        if (fireStationTile == null)
        {
            EditorUtility.DisplayDialog("Warning", "There's not fire station tile.", "OK");
            return;
        }
#endif
        fireStationTransform = fireStationTile.transform;

    }


    public void SpendActionPoint(int actionPointValue)
    {
        if (currentActionPoint >= 0.0f && currentActionPoint >= actionPointValue)
        {
            currentActionPoint -= actionPointValue;
            actionPointSlider.value = currentActionPoint;
        }
    }

    public void SpawnFirefighter()
    {
        if (currentActionPoint >= 1)
        {
            GameObject ff = Instantiate(firefighterPrefab, fireStationTransform.position, Quaternion.identity);
            Firefighter firefighter = ff.GetComponent<Firefighter>();
            firefighter.Init(gridManager, phaseManager.currentPhase);
            SpendActionPoint(1);

            if (phaseManager.currentPhase == Phase.PREP)
            {
                TileClickManager.Instance.SetActiveFirefighter(firefighter);
            }
            else if (phaseManager.currentPhase == Phase.ACTION)
            {
                firefighter.BeginFirefightingMode();
            }
        }
    }

    public void StartRegen()
    {
        regenActive = true;
    }

    public void IncrementBar()
    {
        actionPointSlider.value += (1 / actionPointRegenTime) * Time.deltaTime;
        currentActionPoint = Mathf.FloorToInt(actionPointSlider.value);
    }

    private void Update()
    {
        if (regenActive)
        {
            if (currentActionPoint < 10)
            {
                IncrementBar();
            }
        }
    }

    // Speed Boost Ability
    public void ActivateSpeedBoost()
    {
        if (currentActionPoint >= speedBoostCost && !speedBoostActive)
        {
            SpendActionPoint(speedBoostCost);
            speedBoostActive = true;

            // Apply speed boost to all active firefighters
            Firefighter[] allFirefighters = FindObjectsByType<Firefighter>(FindObjectsSortMode.None);
            foreach (Firefighter firefighter in allFirefighters)
            {
                firefighter.ApplySpeedBoost(speedBoostMultiplier);
            }

            StartCoroutine(SpeedBoostCoroutine());
            Debug.Log($"Speed Boost activated for {speedBoostDuration} seconds!");
        }
        else if (speedBoostActive)
        {
            Debug.Log("Speed Boost is already active!");
        }
        else
        {
            Debug.Log($"Not enough action points! Need {speedBoostCost}, have {currentActionPoint}");
        }
    }

    private IEnumerator SpeedBoostCoroutine()
    {
        yield return new WaitForSeconds(speedBoostDuration);

        // Reset speed for all firefighters
        Firefighter[] allFirefighters = FindObjectsByType<Firefighter>(FindObjectsSortMode.None);
        foreach (Firefighter firefighter in allFirefighters)
        {
            if (firefighter != null) // Check if firefighter still exists
            {
                firefighter.ResetSpeed();
            }
        }

        speedBoostActive = false;
        Debug.Log("Speed Boost effect has ended.");
    }

    // Public method to check if speed boost is active (for UI purposes)
    public bool IsSpeedBoostActive()
    {
        return speedBoostActive;
    }

    // Public method to get speed boost cost (for UI purposes)
    public int GetSpeedBoostCost()
    {
        return speedBoostCost;
    }

    // Break Line Ability
    public void ActivateBreakLine()
    {
        if (currentActionPoint >= breakLineCost && !breakLineActive)
        {
            breakLineActive = true;
            Debug.Log("Break Line ability activated! Click on any tile to clear that entire column of trees.");

            // Enable column selection mode
            EnableColumnSelection();
        }
        else if (breakLineActive)
        {
            Debug.Log("Break Line is already active! Click on any tile to clear that column.");
        }
        else
        {
            Debug.Log($"Not enough action points! Need {breakLineCost}, have {currentActionPoint}");
        }
    }

    private void EnableColumnSelection()
    {
        // Subscribe to tile click events to detect column selection
        if (TileClickManager.Instance != null)
        {
            TileClickManager.Instance.OnColumnSelectionMode(true, OnColumnSelected);
        }
    }

    private void OnColumnSelected(int columnX)
    {
        if (breakLineActive)
        {
            // Spend action points
            SpendActionPoint(breakLineCost);

            // Clear all non-burning trees in the selected column
            ClearTreesInColumn(columnX);

            // Deactivate break line mode
            breakLineActive = false;
            if (TileClickManager.Instance != null)
            {
                TileClickManager.Instance.OnColumnSelectionMode(false, null);
            }

            Debug.Log($"Break Line used on column {columnX}!");
        }
    }

    private void ClearTreesInColumn(int columnX)
    {
        Tile[] allTiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);
        int treesCleared = 0;

        foreach (Tile tile in allTiles)
        {
            // Check if tile is in the selected column and is a non-burning tree
            if (tile.gridX == columnX && tile.IsTileType(TileType.Tree) && !tile.IsBurning())
            {
                // Convert tree to plain
                gridManager.ReplaceTileWithPlain(tile);
                gridManager.numberOfTreesCutDownToPlains++;
                gridManager.numberOfRemainingTree--;
                treesCleared++;
            }
        }

        Debug.Log($"Cleared {treesCleared} trees from column {columnX}");
    }

    // Public method to check if break line is active (for UI purposes)
    public bool IsBreakLineActive()
    {
        return breakLineActive;
    }

    // Public method to get break line cost (for UI purposes)
    public int GetBreakLineCost()
    {
        return breakLineCost;
    }

    // Public method to cancel break line selection
    public void CancelBreakLine()
    {
        if (breakLineActive)
        {
            breakLineActive = false;
            if (TileClickManager.Instance != null)
            {
                TileClickManager.Instance.OnColumnSelectionMode(false, null);
            }
            Debug.Log("Break Line ability cancelled.");
        }
    }

    public void ActivateFlameRetardant()
    {
        if (currentActionPoint >= flameRetardantCost && !flameRetardantActive)
        {
            SpendActionPoint(flameRetardantCost);
            flameRetardantActive = true;

            Firefighter[] allFirefighters = FindObjectsByType<Firefighter>(FindObjectsSortMode.None);
            foreach (Firefighter ff in allFirefighters)
            {
                ff.hasFlameRetardantBuff = true;
            }

            StartCoroutine(FlameRetardantDuration());
            Debug.Log("Flame Retardant activated!");
        }
        else
        {
            Debug.Log("Not enough AP or already active!");
        }
    }

    private IEnumerator FlameRetardantDuration()
    {
        yield return new WaitForSeconds(flameRetardantDuration);

        Firefighter[] allFirefighters = FindObjectsByType<Firefighter>(FindObjectsSortMode.None);
        foreach (Firefighter ff in allFirefighters)
        {
            ff.hasFlameRetardantBuff = false;
        }

        flameRetardantActive = false;
        Debug.Log("Flame Retardant expired.");
    }

    public void ActivateWaterTanker()
    {
        if (currentActionPoint >= waterTankerCost && !waterTankerActive)
        {
            waterTankerActive = true;
            Debug.Log("Water Tanker activated! Select center of 2x2 area.");
            TileClickManager.Instance.OnTileSelectionMode(true, OnWaterTankerTargetSelected);
        }
        else
        {
            Debug.Log("Not enough AP or already active.");
        }
    }

    private void OnWaterTankerTargetSelected(Tile centerTile)
    {
        SpendActionPoint(waterTankerCost);
        waterTankerActive = false;
        TileClickManager.Instance.OnTileSelectionMode(false, null);

        StartCoroutine(DropWaterBombAfterDelay(centerTile, 3f));
    }

    private IEnumerator DropWaterBombAfterDelay(Tile center, float delay)
    {
        yield return new WaitForSeconds(delay);

        List<Tile> targetTiles = gridManager.GetTilesInSquare(center.gridX, center.gridZ, 2);
        foreach (Tile t in targetTiles)
        {
            if (t != null && t.IsBurning())
            {
                gridManager.ReplaceTileWithTree(t);
            }
        }

        Debug.Log("Water bomb dropped!");
    }
}
