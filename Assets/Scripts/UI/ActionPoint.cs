using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ActionPoint : MonoBehaviour
{
    //AP Max should always be 10
    //[SerializeField] public int currentActionPoint = 10;
    [SerializeField] public float actionPointRegenTime = 6.0f;
    [SerializeField] public Slider actionPointSlider;
    [SerializeField] public GameObject firefighterPrefab;
    [SerializeField] GridManager gridManager;
    private Transform fireStationTransform;


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
            ff.GetComponent<Firefighter>().Init(FindObjectOfType<GridManager>());
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


}
