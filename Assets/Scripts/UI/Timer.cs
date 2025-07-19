using UnityEngine;
using UnityEngine.UI;
using TMPro; 
public class Timer : MonoBehaviour
{
    [SerializeField] private bool timerOn = true;
    private bool warning = false; 
    [SerializeField] private Image timerFillImage;
    private float maxTime = 30.0f; //Max time 
    private float warningTime = 5.0f; //Time when the timer goes red
    [SerializeField] private TextMeshProUGUI timerText; 
    private float currentTime = 0.0f;
    [SerializeField] private PhaseManager phaseManager;
    private int lastSecond = -1; // Track the last second to play tick sound
    
    private void Start()
    {
        currentTime = maxTime;
        lastSecond = Mathf.CeilToInt(currentTime);
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
            
            int currentSecond = Mathf.CeilToInt(currentTime);
            timerText.text = currentSecond.ToString();
            
            // Play clock tick sound when second changes
            if (currentSecond != lastSecond && currentSecond > 0)
            {
                if (AudioManager.Instance != null)
                {
                    // Store original volume
                    float originalVolume = AudioManager.Instance.sfxSource.volume;
                    
                    // Set volume to half
                    AudioManager.Instance.sfxSource.volume = originalVolume * 0.5f;
                    
                    // Play clock tick sound
                    AudioManager.Instance.PlayUIClockTickSound();
                    
                    // Restore original volume
                    AudioManager.Instance.sfxSource.volume = originalVolume;
                }
                lastSecond = currentSecond;
            }
        }

        if(currentTime <= warningTime && !warning)
        {
            warning = true; 
            timerFillImage.color = Color.red;
            
            // Play warning sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayUIWarningFiveSecsSound();
            }
        }

        if(currentTime <= 0.0f)
        {
            timerOn = false;
            gameObject.SetActive(false);
            phaseManager.TransitionToAction(); 
        }
    }
}
