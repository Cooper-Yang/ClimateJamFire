using UnityEngine;

public class TempAudioForUI : MonoBehaviour
{
    private AudioManager audioManager;

    void Start()
    {
        // Find the AudioManager instance
        audioManager = AudioManager.Instance;
        
        if (audioManager == null)
        {
            Debug.LogError("AudioManager instance not found! Make sure AudioManager is in the scene.");
        }
    }

    // Music Crossfade Functions for UI Buttons
    public void CrossfadeToGameMusic()
    {
        if (audioManager != null)
        {
            audioManager.CrossfadeToGameMusic();
        }
        else
        {
            Debug.LogWarning("AudioManager not found - cannot crossfade to game music");
        }
    }

    public void CrossfadeToStartMusic()
    {
        if (audioManager != null)
        {
            audioManager.CrossfadeToStartMusic();
        }
        else
        {
            Debug.LogWarning("AudioManager not found - cannot crossfade to start music");
        }
    }

    // Additional UI Sound Functions (optional - for complete UI audio support)
    public void PlayUIClickSound()
    {
        if (audioManager != null)
        {
            audioManager.PlayUIClickSound();
        }
    }

    public void PlayUIHoverSound()
    {
        if (audioManager != null)
        {
            audioManager.PlayUIHoverSound();
        }
    }

    public void PlayUIDenySound()
    {
        if (audioManager != null)
        {
            audioManager.PlayUIDenySound();
        }
    }

    public void PlayUIStartSound()
    {
        if (audioManager != null)
        {
            audioManager.PlayUIStartSound();
        }
    }

    public void PlayUIPauseSound()
    {
        if (audioManager != null)
        {
            audioManager.PlayUIPauseSound();
        }
    }
}