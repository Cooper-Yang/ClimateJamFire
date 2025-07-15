using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    
    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    
    [Header("Background Music")]
    public AudioClip backgroundMusic;
    
    [Header("Ability Sounds")]
    public AudioClip firefighterSpawnSound;
    public AudioClip speedBoostSound;
    public AudioClip fireRetardantSound;
    public AudioClip waterTankerSound;
    public AudioClip breakLineSound;
    [Header("Other Sounds")]
    public AudioClip DespawnSound;
    
    public AudioClip ExtinguishSound;
    public AudioClip FireBurnLoopSound;
    public AudioClip TreeChopSound;
    
    [Header("Audio Settings")]
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;
    [Range(0f, 1f)]
    public float sfxVolume = 0.8f;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            if (musicSource == null) musicSource = gameObject.AddComponent<AudioSource>();
            if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();
            
            musicSource.loop = true;
            musicSource.volume = musicVolume;
            sfxSource.volume = sfxVolume;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        if (backgroundMusic != null)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }
    
    // Ability Sound Functions
    public void PlayFirefighterSpawnSound()
    {
        if (firefighterSpawnSound != null)
            sfxSource.PlayOneShot(firefighterSpawnSound);
    }
    
    public void PlaySpeedBoostSound()
    {
        if (speedBoostSound != null)
            sfxSource.PlayOneShot(speedBoostSound);
    }
    
    public void PlayFireRetardantSound()
    {
        if (fireRetardantSound != null)
            sfxSource.PlayOneShot(fireRetardantSound);
    }
    
    public void PlayWaterTankerSound()
    {
        if (waterTankerSound != null)
            sfxSource.PlayOneShot(waterTankerSound);
    }
    
    public void PlayBreakLineSound()
    {
        if (breakLineSound != null)
            sfxSource.PlayOneShot(breakLineSound);
    }

    // Other Sound Functions
    public void PlayDespawnSound()
    {
        if (DespawnSound != null)
            sfxSource.PlayOneShot(DespawnSound);
    }

    public void PlayExtinguishSound()
    {
        if (ExtinguishSound != null)
            sfxSource.PlayOneShot(ExtinguishSound);
    }

    public void PlayTreeChopSound()
    {
        if (TreeChopSound != null)
            sfxSource.PlayOneShot(TreeChopSound);
    }

    // For looping fire burn sound
    public void PlayFireBurnLoopSound()
    {
        if (FireBurnLoopSound != null && !sfxSource.isPlaying)
        {
            sfxSource.clip = FireBurnLoopSound;
            sfxSource.loop = true;
            sfxSource.Play();
        }
    }

    public void StopFireBurnLoopSound()
    {
        if (sfxSource.clip == FireBurnLoopSound)
        {
            sfxSource.Stop();
            sfxSource.loop = false;
            sfxSource.clip = null;
        }
    }

    // Simple controls
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        musicSource.volume = volume;
    }
    
    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        sfxSource.volume = volume;
    }
}
