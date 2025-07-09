using UnityEngine;
using UnityEngine.UI;
using TMPro; 
public class Timer : MonoBehaviour
{
    [SerializeField] private GameObject TimerObject; 
    [SerializeField] private bool timerOn = true;
    private bool warning = false; 
    [SerializeField] private Image timerFillImage;
    [SerializeField] private float maxTime = 60.0f; //Max time 
    [SerializeField] private float warningTime = 15.0f; //Time when the timer goes red
    [SerializeField] private TextMeshProUGUI timerText; 
    private float currentTime = 0.0f;

    [SerializeField] private PhaseManager phaseManager; 
    private void Start()
    {
        currentTime = maxTime; 
    }

    // Update is called once per frame
    void Update()
    {
        if (timerOn)
        {
            currentTime -= Time.deltaTime;
            timerFillImage.fillAmount = currentTime / maxTime;
            timerText.text = Mathf.FloorToInt(currentTime).ToString();
        }
        if(currentTime <= warningTime && !warning)
        {
            warning = true; 
            timerFillImage.color = Color.red; 
        }
        if(currentTime <= 0.0f)
        {
            timerOn = false;
            TimerObject.SetActive(false);
            phaseManager.TransitionToAction(); 
        }
    }
}
