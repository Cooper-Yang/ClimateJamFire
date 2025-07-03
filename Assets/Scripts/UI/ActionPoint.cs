using UnityEngine;
using UnityEngine.UI;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ActionPoint : MonoBehaviour
{
    //AP Max should always be 10
    //[SerializeField] public int currentActionPoint = 10;
    public float actionPointRegenTime = 6.0f;
    public Slider actionPointSlider;
    public GameObject firefighterPrefab;
    public GridManager gridManager;
    private Transform fireStationTransform;

    // Speed Boost Ability Variables
    [Header("Speed Boost Ability")]
    public int speedBoostCost = 3;
    public float speedBoostDuration = 10.0f;
    public float speedBoostMultiplier = 0.5f; // 50% speed increase
    private bool speedBoostActive = false;

    private bool regenActive = true;
    private float timer = 0.0f;
    public int currentPhase = 1;

    
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
        if(currentActionPoint >= 0.0f && currentActionPoint >= actionPointValue)
        {
            currentActionPoint -= actionPointValue;
            actionPointSlider.value = currentActionPoint;
        }
    }

    public void SpawnFirefighter()
    {
        if (currentActionPoint >= 1 && currentPhase == 1)
        {
            GameObject ff = Instantiate(firefighterPrefab, fireStationTransform.position, Quaternion.identity);
            ff.GetComponent<Firefighter>().Init(gridManager);
            SpendActionPoint(1);
            //currentActionPoint--;
            //StartCoroutine(StartRegen()); 
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
        if(regenActive)
        {
            if(currentActionPoint < 10)
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


}
