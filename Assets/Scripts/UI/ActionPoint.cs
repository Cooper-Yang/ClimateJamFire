using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using static UnityEngine.InputSystem.UI.VirtualMouseInput;



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

    [Header("Fire Fighter Ability")]
    public Ability fireFighterAbility;
    public Vector3 fireFighterPositionOffset;

    [Header("Speed Boost Ability")]
    public Ability speedBoostAbility;
    public float speedBoostDuration = 10.0f;
    public float speedBoostMultiplier = 0.5f; // 50% speed increase
    private bool speedBoostActive = false;

    // Break Line Ability Variables
    [Header("Break Line Ability")]
    public Ability breakLineAbility;
    private bool breakLineActive = false;

    [Header("Flame Retardant Ability")]
    public Ability flameRetardantAbility;
    public float flameRetardantDuration = 10f;
    private bool flameRetardantActive = false;

    [Header("Water Tanker Ability")]
    public Ability waterTankerAbility;
    private bool waterTankerActive = false;

    [Header("Phase Transition")]
    [SerializeField] public PhaseManager phaseManager;

    public float currentActionPoint = 10;

    [Header("Cursor Textures")]
    [SerializeField] private Texture2D defualtCursor;
    [SerializeField] private Texture2D waterTankerCursor;
    [SerializeField] private Texture2D cutTreeCursor;

    [Header("Water Tanker Plane")]
    public GameObject waterTankerPlanePrefab;  
    public float planeFlyDuration = 8f;
    public Vector3 planeStartOffset = new Vector3(1.25f, 5f, 50f); //-50 to 50 z, X to -20
    public Vector3 planeEndOffset = new Vector3(1.25f, 5f, -50f);

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
        }
    }

    public void SpawnFirefighter()
    {
        if (currentActionPoint >= fireFighterAbility.abilityCost)
        {
            GameObject ff = Instantiate(firefighterPrefab, fireStationTransform.position + fireFighterPositionOffset, Quaternion.identity);
            Firefighter firefighter = ff.GetComponent<Firefighter>();
            firefighter.Init(gridManager, fireFighterPositionOffset, phaseManager.currentPhase);
            SpendActionPoint(fireFighterAbility.abilityCost);

            // Play firefighter spawn sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayFirefighterSpawnSound();
            }

            if (phaseManager.currentPhase == Phase.PREP)
            {
                TileClickManager.Instance.SetActiveFirefighter(firefighter);
            }
            else if (phaseManager.currentPhase == Phase.ACTION)
            {
                TileClickManager.Instance.SetActiveFirefighter(firefighter);
            }
        }
        else
        {
            // Play deny sound when not enough AP
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayUIDenySound();
            }
        }
    }

    public void StartRegen()
    {
        regenActive = true;
    }

    public void IncrementBar()
    {
        currentActionPoint += (1f / actionPointRegenTime) * Time.deltaTime;
    }

    private void Update()
    {
        if (regenActive && phaseManager.currentPhase == Phase.ACTION)
        {
            if (currentActionPoint < 10)
            {
                IncrementBar();
            }
        }
        currentActionPoint = Mathf.Min(10, currentActionPoint);
        actionPointSlider.value = currentActionPoint;
    }

    // Speed Boost Ability
    public void ActivateSpeedBoost()
    {
        if (currentActionPoint >= speedBoostAbility.abilityCost && !speedBoostActive)
        {
            SpendActionPoint(speedBoostAbility.abilityCost);
            speedBoostActive = true;

            // Apply speed boost to all active firefighters
            Firefighter[] allFirefighters = FindObjectsByType<Firefighter>(FindObjectsSortMode.None);
            foreach (Firefighter firefighter in allFirefighters)
            {
                firefighter.ApplySpeedBoost(speedBoostMultiplier);
            }

            // Play speed boost sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySpeedBoostSound();
            }

            StartCoroutine(SpeedBoostCoroutine());
            Debug.Log($"Speed Boost activated for {speedBoostDuration} seconds!");
        }
        else if (speedBoostActive)
        {
            Debug.Log("Speed Boost is already active!");
            // Play deny sound when already active
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayUIDenySound();
            }
        }
        else
        {
            Debug.Log($"Not enough action points! Need {speedBoostAbility.abilityCost}, have {currentActionPoint}");
            // Play deny sound when not enough AP
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayUIDenySound();
            }
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
    public int GetSpeedBoostAbilityCost()
    {
        return speedBoostAbility.abilityCost;
    }

    // Break Line Ability
    public void ActivateBreakLine()
    {
        if (currentActionPoint >= breakLineAbility.abilityCost && !breakLineActive)
        {
            breakLineActive = true;
            Debug.Log("Break Line ability activated! Click on any tile to clear that entire column of trees.");

            // Enable column selection mode
            EnableColumnSelection();
        }
        else if (breakLineActive)
        {
            Debug.Log("Break Line is already active! Click on any tile to clear that column.");
            // Play deny sound when already active
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayUIDenySound();
            }
        }
        else
        {
            Debug.Log($"Not enough action points! Need {breakLineAbility.abilityCost}, have {currentActionPoint}");
            // Play deny sound when not enough AP
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayUIDenySound();
            }
        }
    }

    private void EnableColumnSelection()
    {
        // Subscribe to tile click events to detect column selection
        if (TileClickManager.Instance != null)
        {
            Cursor.SetCursor(cutTreeCursor, GetTextureCenter(cutTreeCursor), UnityEngine.CursorMode.Auto);
            TileClickManager.Instance.OnColumnSelectionMode(true, OnColumnSelected);
        }
    }

    private void OnColumnSelected(int columnX)
    {
        if (breakLineActive)
        {
            // Spend action points
            SpendActionPoint(breakLineAbility.abilityCost);
            Cursor.SetCursor(defualtCursor, GetTextureCenter(defualtCursor), UnityEngine.CursorMode.Auto);
            
            // Clear all highlights before chopping
            TileClickManager.Instance.ClearAllHighlights();
            
            // Clear all non-burning trees in the selected column
            ChopTreesInColumn(columnX);

            // Play break line sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayBreakLineSound();
            }

            // Deactivate break line mode
            breakLineActive = false;
            if (TileClickManager.Instance != null)
            {
                TileClickManager.Instance.OnColumnSelectionMode(false, null);
            }

            Debug.Log($"Break Line used on column {columnX}!");
        }
    }

    private void ChopTreesInColumn(int columnX)
    {
        Tile[] allTiles = FindObjectsByType<Tile>(FindObjectsSortMode.None);
        int treesCleared = 0;

        foreach (Tile tile in allTiles)
        {
            // Check if tile is in the selected column and is a non-burning tree
            if (tile.gridX == columnX && tile.IsTileType(TileType.Tree) && !tile.IsBurning())
            {
                // Convert tree to plain
                gridManager.ReplaceTileWithPlain(tile, gridManager.choppedTilePrefab);
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
        return breakLineAbility.abilityCost;
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
        if (currentActionPoint >= flameRetardantAbility.abilityCost && !flameRetardantActive)
        {
            SpendActionPoint(flameRetardantAbility.abilityCost);
            flameRetardantActive = true;

            Firefighter[] allFirefighters = FindObjectsByType<Firefighter>(FindObjectsSortMode.None);
            foreach (Firefighter ff in allFirefighters)
            {
                ff.hasFlameRetardantBuff = true;
            }

            // Play fire retardant sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayFireRetardantSound();
            }

            StartCoroutine(FlameRetardantDuration());
            Debug.Log("Flame Retardant activated!");
        }
        else
        {
            Debug.Log("Not enough AP or already active!");
            // Play deny sound when not enough AP or already active
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayUIDenySound();
            }
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
        if (currentActionPoint >= waterTankerAbility.abilityCost && !waterTankerActive)
        {
            Cursor.SetCursor(waterTankerCursor, GetTextureCenter(waterTankerCursor), UnityEngine.CursorMode.Auto);
            waterTankerActive = true;
            Debug.Log("Water Tanker activated! Select center of 2x2 area.");
            TileClickManager.Instance.OnTileSelectionMode(true, OnWaterTankerTargetSelected);
        }
        else
        {
            Debug.Log("Not enough AP or already active.");
            // Play deny sound when not enough AP or already active
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayUIDenySound();
            }
        }
    }

    private void OnWaterTankerTargetSelected(Tile centerTile)
    {
        Cursor.SetCursor(defualtCursor, GetTextureCenter(defualtCursor), UnityEngine.CursorMode.Auto);
        SpendActionPoint(waterTankerAbility.abilityCost);
        waterTankerActive = false;
        TileClickManager.Instance.OnTileSelectionMode(false, null);

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayWaterTankerSound();
        }

        //StartCoroutine(DropWaterBombAfterDelay(centerTile, 1.5f));
        StartCoroutine(SpawnAndAnimateWaterTanker(centerTile));
    }
    private IEnumerator SpawnAndAnimateWaterTanker(Tile centerTile)
    {
        Vector3 targetPosition = centerTile.transform.position;
        float targetX = targetPosition.x;
        Vector3 startPosition =  planeStartOffset;
        Vector3 endPosition = planeEndOffset;
        startPosition.x = targetX;
        endPosition.x = targetX;
        Debug.Log("Target" + targetPosition);
        Debug.Log("Start" + startPosition);
        Debug.Log("End" + endPosition);
        GameObject plane = Instantiate(waterTankerPlanePrefab, startPosition, Quaternion.identity);
        plane.transform.LookAt(endPosition);
        /*Animation anim = plane.GetComponent<Animation>();
        if (anim != null)
        {
            anim.Play(Take 001);
        }*/

        float elapsedTime = 0f;

        while (elapsedTime < planeFlyDuration)
        {
            float t = elapsedTime / planeFlyDuration;
            plane.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            if (t >= 0.5f && t - Time.deltaTime / planeFlyDuration < 0.5f)
            {
                
                DropWaterOnTarget(centerTile);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        plane.transform.position = endPosition;

        Destroy(plane);
    }

    private void DropWaterOnTarget(Tile center)
    {
        List<Tile> targetTiles = gridManager.GetTilesInSquare(center.gridX, center.gridZ, 2);
        foreach (Tile t in targetTiles)
        {
            if (t != null && t.IsBurning())
            {
                if (t.isHouse)
                {
                    gridManager.ReplaceTileWithHouse(t);
                }
                else
                {
                    gridManager.ReplaceTileWithTree(t);
                }
            }
        }
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayExtinguishSound();
        }

        Debug.Log("Water dropped!");
    }

    private Vector2 GetTextureCenter(Texture2D texture)
    {
        if (texture == null) return Vector2.zero;

        float centerX = texture.width / 2f;
        float centerY = texture.height / 2f;

        return new Vector2(centerX, centerY);
    }
}
