using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;
    public AudioSource musicSource2; // Second music source for crossfading

    [Header("Background Music")]
    public AudioClip startSceneMusic;
    public AudioClip gameSceneMusic;
    public AudioClip gameSceneMusicAlt; // Alternative version of game music

    [Header("Crossfade Settings")]
    [Range(0.1f, 5f)]
    public float crossfadeDuration = 2f;

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
    public AudioClip FireBurstSound;
    public AudioClip TreeChopSound;

    [Header("UI Sounds")]
    public AudioClip[] UI_ClickSounds;
    public AudioClip[] UI_ClockTickSounds;
    public AudioClip UI_DenySound;
    public AudioClip[] UI_HoverSounds;
    public AudioClip[] UI_NewPhaseSounds;
    public AudioClip UI_StartSound;
    public AudioClip UI_WarningFiveSecsSound;
    public AudioClip UI_PauseSound;

    [Header("Audio Settings")]
    [Range(0f, 1f)]
    public float musicVolume = 0.5f;
    [Range(0f, 1f)]
    public float sfxVolume = 0.8f;

    // Music toggle state
    private bool useAlternativeGameMusic = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (musicSource == null) musicSource = gameObject.AddComponent<AudioSource>();
            if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();
            if (musicSource2 == null) musicSource2 = gameObject.AddComponent<AudioSource>();

            musicSource.loop = true;
            musicSource.volume = musicVolume;
            musicSource2.loop = true;
            musicSource2.volume = 0f; // Start at 0 for crossfading
            sfxSource.volume = sfxVolume;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Play start scene music initially
        if (startSceneMusic != null)
        {
            musicSource.clip = startSceneMusic;
            musicSource.Play();
        }
    }

    // Get current game music based on toggle state
    private AudioClip GetCurrentGameMusic()
    {
        return useAlternativeGameMusic ? gameSceneMusicAlt : gameSceneMusic;
    }

    // Toggle between game music versions (only changes selection, doesn't play)
    public void ToggleGameMusic()
    {
        useAlternativeGameMusic = !useAlternativeGameMusic;

        // Don't play music here - just change the selection
        // Music will play when CrossfadeToGameMusic() is called from StartGame()
    }

    // Check if we're currently playing game music (either version)
    private bool IsPlayingGameMusic()
    {
        return (musicSource.clip == gameSceneMusic || musicSource.clip == gameSceneMusicAlt) ||
               (musicSource2.clip == gameSceneMusic || musicSource2.clip == gameSceneMusicAlt);
    }

    // Get current toggle state
    public bool IsUsingAlternativeGameMusic()
    {
        return useAlternativeGameMusic;
    }

    // Set toggle state directly (only changes selection, doesn't play)
    public void SetGameMusicToggle(bool useAlternative)
    {
        useAlternativeGameMusic = useAlternative;

        // Don't play music here - just change the selection
    }

    // Modified crossfade to game music to use current selection
    public void CrossfadeToGameMusic()
    {
        AudioClip currentGameMusic = GetCurrentGameMusic();
        if (currentGameMusic != null)
        {
            StartCoroutine(CrossfadeMusic(musicSource, musicSource2, currentGameMusic));
        }
    }

    // Crossfade back to start scene music
    public void CrossfadeToStartMusic()
    {
        if (startSceneMusic != null)
        {
            StartCoroutine(CrossfadeMusic(musicSource, musicSource2, startSceneMusic));
        }
    }

    private IEnumerator CrossfadeMusic(AudioSource fromSource, AudioSource toSource, AudioClip newClip)
    {
        // Set up the new music source
        toSource.clip = newClip;
        toSource.volume = 0f;
        toSource.Play();

        float timer = 0f;
        float fromStartVolume = fromSource.volume;

        // Crossfade
        while (timer < crossfadeDuration)
        {
            timer += Time.unscaledDeltaTime;
            float progress = timer / crossfadeDuration;

            fromSource.volume = Mathf.Lerp(fromStartVolume, 0f, progress);
            toSource.volume = Mathf.Lerp(0f, musicVolume, progress);

            yield return null;
        }

        // Ensure final volumes are set correctly
        fromSource.volume = 0f;
        toSource.volume = musicVolume;

        // Stop the old source and swap references
        fromSource.Stop();

        // Swap the sources so musicSource is always the active one
        if (fromSource == musicSource)
        {
            AudioSource temp = musicSource;
            musicSource = musicSource2;
            musicSource2 = temp;
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

    public void PlayFireBurstSound()
    {
        if (FireBurstSound != null)
        {
            // Play at reduced volume directly (half of current SFX volume)
            sfxSource.PlayOneShot(FireBurstSound, sfxVolume * 0.5f);
        }
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

    // UI Sound Functions
    public void PlayUIClickSound()
    {
        if (UI_ClickSounds != null && UI_ClickSounds.Length > 0)
        {
            AudioClip randomClip = UI_ClickSounds[Random.Range(0, UI_ClickSounds.Length)];
            if (randomClip != null)
                sfxSource.PlayOneShot(randomClip);
        }
    }

    public void PlayUIClockTickSound()
    {
        if (UI_ClockTickSounds != null && UI_ClockTickSounds.Length > 0)
        {
            AudioClip randomClip = UI_ClockTickSounds[Random.Range(0, UI_ClockTickSounds.Length)];
            if (randomClip != null)
            {
                // Play at reduced volume directly (10% of current SFX volume)
                sfxSource.PlayOneShot(randomClip, sfxVolume * 0.1f);
            }
        }
    }

    public void PlayUIDenySound()
    {
        if (UI_DenySound != null)
        {
            // Play at reduced volume directly (half of current SFX volume)
            sfxSource.PlayOneShot(UI_DenySound, sfxVolume * 0.5f);
        }
    }

    public void PlayUIHoverSound()
    {
        if (UI_HoverSounds != null && UI_HoverSounds.Length > 0)
        {
            AudioClip randomClip = UI_HoverSounds[Random.Range(0, UI_HoverSounds.Length)];
            if (randomClip != null)
                sfxSource.PlayOneShot(randomClip);
        }
    }

    public void PlayUINewPhaseSound()
    {
        if (UI_NewPhaseSounds != null && UI_NewPhaseSounds.Length > 0)
        {
            AudioClip randomClip = UI_NewPhaseSounds[Random.Range(0, UI_NewPhaseSounds.Length)];
            if (randomClip != null)
                sfxSource.PlayOneShot(randomClip);
        }
    }

    public void PlayUIStartSound()
    {
        if (UI_StartSound != null)
            sfxSource.PlayOneShot(UI_StartSound);
    }

    public void PlayUIWarningFiveSecsSound()
    {
        if (UI_WarningFiveSecsSound != null)
            sfxSource.PlayOneShot(UI_WarningFiveSecsSound);
    }

    public void PlayUIPauseSound()
    {
        if (UI_PauseSound != null)
            sfxSource.PlayOneShot(UI_PauseSound);
    }

    // Simple controls
    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        musicSource.volume = volume;

        // Also update the second source if it's playing
        if (musicSource2.isPlaying)
        {
            musicSource2.volume = volume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        sfxSource.volume = volume;
    }
}
