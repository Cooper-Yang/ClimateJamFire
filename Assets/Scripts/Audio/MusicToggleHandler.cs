using UnityEngine;
using UnityEngine.UI;

public class MusicToggleHandler : MonoBehaviour
{
    private Toggle musicToggle;
    
    private void Start()
    {
        musicToggle = GetComponent<Toggle>();
        
        if (musicToggle != null && AudioManager.Instance != null)
        {
            // Set initial toggle state
            musicToggle.isOn = AudioManager.Instance.IsUsingAlternativeGameMusic();
            
            // Add listener for toggle changes
            musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
        }
    }
    
    private void OnMusicToggleChanged(bool isOn)
    {
        if (AudioManager.Instance != null)
        {
            // Just set the toggle state - don't play game music yet
            AudioManager.Instance.SetGameMusicToggle(isOn);
        }
    }
    
    private void OnDestroy()
    {
        if (musicToggle != null)
        {
            musicToggle.onValueChanged.RemoveListener(OnMusicToggleChanged);
        }
    }
}