using UnityEngine;
using UnityEngine.SceneManagement; 
public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject PausePanel;
    [SerializeField] private GameObject tutorialPanel;
    //[SerializeField] private string gameSceneName;
    private float OriginalVol; 
    public void PauseGame()
    {
        PausePanel.SetActive(true);
        Time.timeScale = 0f;
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayUIPauseSound();
            // Store original volume
            OriginalVol = AudioManager.Instance.sfxSource.volume;
            // Set volume to half
            //AudioManager.Instance.sfxSource.volume = OriginalVol * 0.5f;
        }
    }
    private void Start()
    {
        //Activate tutorial
        tutorialPanel.SetActive(true);
        Time.timeScale = 0f; 
    }
    public void ResumeGame()
    {
        PausePanel.SetActive(false);
        tutorialPanel.SetActive(false);
        Time.timeScale = 1f;
        // Restore original volume
        //AudioManager.Instance.sfxSource.volume = OriginalVol;
    }

    public void QuitGame()
    {
        Application.Quit(); 
    }

    public void BackToMain()
    {
        Debug.Log("BackToMain called - loading Start Menu");
        SceneManager.LoadScene("Start Menu");
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(PausePanel.activeInHierarchy)
            {
                //This means pause is already active. so resume the game
                ResumeGame(); 
            }
            else
            {
                //Pause. 
                PauseGame(); 
            }
        }
    }
}
