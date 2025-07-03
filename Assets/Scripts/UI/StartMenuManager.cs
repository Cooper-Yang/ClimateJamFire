using UnityEngine;
using UnityEngine.SceneManagement; 
public class StartMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private string gameSceneName; 
    public void StartGame()
    {
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

    //PUT AUDIO SETTINGS RELATED STUFF IN THIS SCRIPT TOO!!
}
