using UnityEngine;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject PausePanel; 

    public void PauseGame()
    {
        PausePanel.SetActive(true);
        Time.timeScale = 0f; 
    }

    public void ResumeGame()
    {
        PausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Application.Quit(); 
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
