using UnityEngine;
using UnityEngine.SceneManagement;

public class RTMM : MonoBehaviour
{
    public void BackToMain()
    {
        Debug.Log("Current Scene: " + SceneManager.GetActiveScene().name);
        if (SceneManager.GetActiveScene().name == "Start Menu")
        {
            Debug.Log("You are already in Start Menu!");
        }
        Debug.Log("BackToMain called - loading Start Menu");
        SceneManager.LoadScene(0);
    }
}
