using UnityEngine;
using UnityEngine.UI;
using TMPro; 
public class Timer : MonoBehaviour
{
    [SerializeField] private bool timerOn = true;
    private bool warning = false; 
    [SerializeField] private Image timerFillImage;
    private float maxTime = 15.0f; //Max time 
    private float warningTime = 5.0f; //Time when the timer goes red
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
        if(!gameObject.activeSelf)
        {
            return;
        }

        if (timerOn)
        {
            currentTime -= Time.deltaTime;
            timerFillImage.fillAmount = currentTime / maxTime;
            timerText.text = Mathf.CeilToInt(currentTime).ToString();
        }

        if(currentTime <= warningTime && !warning)
        {
            warning = true; 
            timerFillImage.color = Color.red;
        }

        if(currentTime <= 0.0f)
        {
            timerOn = false;
            gameObject.SetActive(false);
            phaseManager.TransitionToAction(); 
        }
    }
}
