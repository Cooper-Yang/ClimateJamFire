using UnityEngine;
using UnityEngine.UI; 
public class ActionPoint : MonoBehaviour
{
    //AP Max should always be 10
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
    [SerializeField] private float actionPointRegenTime = 6.0f;
    [SerializeField] public Slider actionPointSlider;

    private bool regenActive = true;
    private void Start()
    {
        
    }

    
    public void SpendActionPoint(int actionPointValue)
    {
        if(currentActionPoint >= 0.0f && currentActionPoint >= actionPointValue)
        {
            currentActionPoint -= actionPointValue; 
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
