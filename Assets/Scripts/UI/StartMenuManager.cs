using UnityEngine;
using UnityEngine.SceneManagement; 
public class StartMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject creditsPanel; 
    [SerializeField] private string gameSceneName; 
    public void StartGame()
    {
        // Play UI start sound
        if (AudioManager.Instance != null)
        {
            // Start crossfade to game music
            AudioManager.Instance.CrossfadeToGameMusic();
        }
        
        SceneManager.LoadSceneAsync(gameSceneName); 
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OptionsActive()
    {
        optionsPanel.SetActive(true);
    }

    public void OptionsDeactive()
    {
        optionsPanel.SetActive(false);
    }

    public void CreditsActive()
    {
        creditsPanel.SetActive(true);
    }
    public void CreditsDeactive()
    {
        creditsPanel.SetActive(false); 
    }
}
