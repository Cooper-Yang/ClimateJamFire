using UnityEngine;
using UnityEngine.UI; 
public class ActionPoint : MonoBehaviour
{
    //AP Max should always be 10
    [SerializeField] public int currentActionPoint = 10;
    [SerializeField] private float actionPointRegenTime = 6.0f;
    [SerializeField] private Slider actionPointSlider;
    [SerializeField] public GameObject firefighterPrefab;
    [SerializeField] public Transform fireStationTile;


    private bool regenActive = true;
    private float timer = 0.0f;
    public int currentPhase = 1;

    /*
    public int currentActionPoint
    {
        get
        {
            return Mathf.FloorToInt(actionPointSlider.value); 
        }
        set
        {

        }
    } */

    private void Start()
    {
        
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
            GameObject ff = Instantiate(firefighterPrefab, fireStationTile.position, Quaternion.identity);
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
