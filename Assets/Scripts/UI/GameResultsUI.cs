using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameResultsUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject resultPanel;
    public TextMeshProUGUI resultTitleText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI scoringRulesText;
    public Button restartButton;
    
    [Header("Game Manager Reference")]
    public FireManager fireManager;
    public PhaseManager phaseManager;

    private bool uiShown = false;
    
    private void Start()
    {
        // Hide the results panel at start
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
        
        // Find FireManager if not assigned
        if (fireManager == null)
        {
            fireManager = FindFirstObjectByType<FireManager>();
        }
        
        // Setup restart button
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        
        // Subscribe to game end event
        FireManager.OnGameEnded += OnGameEndedHandler;
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events to prevent memory leaks
        FireManager.OnGameEnded -= OnGameEndedHandler;
    }
    
    private void Update()
    {
        // Fallback check if event system fails
        if (fireManager != null && fireManager.IsGameEnded() && !uiShown)
        {
            ShowResults(fireManager.HasPlayerWon(), fireManager.GetFinalScore());
            uiShown = true;
        }
    }
    
    private void OnGameEndedHandler(bool hasWon, int finalScore)
    {
        if (!uiShown)
        {
            ShowResults(hasWon, finalScore);
            uiShown = true;
        }
    }
    
    private void ShowResults(bool hasWon, int finalScore)
    {
        if (resultPanel == null) return;
        
        // Show the panel
        resultPanel.SetActive(true);
        
        // Set title based on win/lose
        if (resultTitleText != null)
        {
            if (hasWon)
            {
                resultTitleText.text = "YOU WON!";
                resultTitleText.color = Color.green;
            }
            else
            {
                resultTitleText.text = "YOU LOST!";
                resultTitleText.color = Color.red;
            }
        }
        
        // Set score
        if (scoreText != null)
        {
            scoreText.text = $"Final Score: {finalScore}";
            scoreText.color = hasWon ? Color.white : Color.gray;
        }
        
        // Set scoring rules (only show on win)
        if (scoringRulesText != null)
        {
            if (hasWon)
            {
                scoringRulesText.text = "Scoring Rules:\n+1 point for each intact Tree\n+3 points for each saved House";
                scoringRulesText.gameObject.SetActive(true);
            }
            else
            {
                scoringRulesText.text = "All houses were destroyed!\nBetter luck next time!";
                scoringRulesText.gameObject.SetActive(true);
                scoringRulesText.color = Color.gray;
            }
        }
    }
    
    private void RestartGame()
    {
        // Hide the panel
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
        
        uiShown = false;

        phaseManager.currentPhase = Phase.PREP;
        // Reload the current scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
        );

    }
    
    // Public method to manually show results (can be called from other scripts)
    public void ForceShowResults()
    {
        if (fireManager != null && !uiShown)
        {
            ShowResults(fireManager.HasPlayerWon(), fireManager.GetFinalScore());
            uiShown = true;
        }
    }
}
