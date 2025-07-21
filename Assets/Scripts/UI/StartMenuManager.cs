using UnityEngine;
using UnityEngine.SceneManagement; 
public class StartMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject creditsPanel;
    [SerializeField] private GameObject MainPanel;
    [SerializeField] private string gameSceneName; 
    public void StartGame()
    {
        // // Play UI start sound
        // if (AudioManager.Instance != null)
        // {
        //     // Start crossfade to game music
        //     AudioManager.Instance.CrossfadeToGameMusic();
        // }
        
        SceneManager.LoadSceneAsync(gameSceneName); 
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OptionsActive()
    {
        optionsPanel.SetActive(true);
        MainPanel.SetActive(false);
    }

    public void OptionsDeactive()
    {
        optionsPanel.SetActive(false);
        MainPanel.SetActive(true);
    }

    public void CreditsActive()
    {
        creditsPanel.SetActive(true);
        MainPanel.SetActive(false);
    }
    public void CreditsDeactive()
    {
        creditsPanel.SetActive(false); 
        MainPanel.SetActive(true);
    }
}
