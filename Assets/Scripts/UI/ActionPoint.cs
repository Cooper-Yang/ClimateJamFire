using UnityEngine;
using UnityEngine.UI; 
public class ActionPoint : MonoBehaviour
{
    //AP Max should always be 10
    [SerializeField] public int currentActionPoint = 0;
    [SerializeField] private float actionPointRegenTime = 6.0f;
    [SerializeField] private Slider actionPointSlider;

    private bool regenActive = true;
    private float timer = 0.0f; 

    private void Start()
    {
        actionPointSlider.value = currentActionPoint; 
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

    private void Update()
    {
        if(regenActive)
        {
            timer += Time.deltaTime; 
            if(timer >= actionPointRegenTime)
            {
                if(currentActionPoint < 10)
                {
                    timer = timer - actionPointRegenTime;
                    currentActionPoint++;
                    actionPointSlider.value = currentActionPoint; 
                }
            }
        }
    }


}
