using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameResultsUI : MonoBehaviour
{
    [Header("UI References")]
    public GameObject resultPanel;
    public TextMeshProUGUI resultTitleText;
    public TextMeshProUGUI scoreText;
    public Button restartButton;
    public Button quitButton;
    
    [Header("Star Rating UI (from ScoreManager)")]
    public GameObject starImage1;
    public GameObject starImage2;
    public GameObject starImage3;
    
    [Header("Game Manager References")]
    public FireManager fireManager;
    public PhaseManager phaseManager;
    public GridManager gridManager;

    [Header("Scoring System (migrated from ScoreManager)")]
    [SerializeField] private int houseScoreMultiplier = 300;
    [SerializeField] private int treeScoreMultiplier = 100;
    [SerializeField] private int cutdownScoreMultiplier = 50;
    [SerializeField] private int treeScoreFirstThreshold = 5;  // 1 star
    [SerializeField] private int treeScoreSecondThreshold = 15; // 2 stars
    [SerializeField] private int treeScoreThirdThreshold = 25;  // 3 stars

    private bool uiShown = false;
    private int houseScore, treeScore, cutdownScore, totalScore;
    
    private void Start()
    {
        // Hide the results panel at start
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }
        
        // Find managers if not assigned
        if (fireManager == null)
        {
            fireManager = FindFirstObjectByType<FireManager>();
        }
        
        if (gridManager == null)
        {
            gridManager = FindFirstObjectByType<GridManager>();
        }
        
        // Setup buttons
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
        
        // Subscribe to game end event
        FireManager.OnGameEnded += OnGameEndedHandler;
        
        // Hide stars initially
        HideAllStars();
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
        
        // Calculate detailed scores using ScoreManager logic
        CalculateDetailedScore();
        
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
        
        // Set detailed score breakdown
        if (scoreText != null)
        {
            if (hasWon)
            {
                scoreText.text = $"Final Score: {totalScore}\n" +
                               $"Houses Saved: {houseScore} ({gridManager.numberOfHouses} × {houseScoreMultiplier})\n" +
                               $"Trees Remaining: {treeScore} ({gridManager.numberOfRemainingTree} × {treeScoreMultiplier})\n" +
                               $"Trees Cut Down: {cutdownScore} ({gridManager.numberOfTreesCutDownToPlains} × {cutdownScoreMultiplier})";
                scoreText.color = Color.white;
            }
            else
            {
                scoreText.text = "All houses were destroyed!\nBetter luck next time!";
                scoreText.color = Color.gray;
            }
        }
        
        // Show star rating if player won
        if (hasWon)
        {
            ShowStars();
        }
        else
        {
            HideAllStars();
        }
    }
    
    private void CalculateDetailedScore()
    {
        if (gridManager == null) return;
        
        houseScore = houseScoreMultiplier * gridManager.numberOfHouses;
        treeScore = treeScoreMultiplier * gridManager.numberOfRemainingTree;
        cutdownScore = cutdownScoreMultiplier * gridManager.numberOfTreesCutDownToPlains;
        
        totalScore = houseScore + treeScore + cutdownScore;
    }
    
    private void ShowStars()
    {
        // Hide all stars first
        HideAllStars();
        
        // Star rating logic (migrated from ScoreManager)
        if (houseScore >= houseScoreMultiplier) // At least 1 house saved
        {
            if (starImage1 != null) starImage1.SetActive(true);
            
            if (houseScore >= 2 * houseScoreMultiplier) // At least 2 houses saved
            {
                if (starImage2 != null) starImage2.SetActive(true);
                
                if (houseScore >= 3 * houseScoreMultiplier) // At least 3 houses saved
                {
                    // Check if we also have enough trees for 3rd star
                    if (gridManager.numberOfRemainingTree >= treeScoreThirdThreshold)
                    {
                        if (starImage3 != null) starImage3.SetActive(true);
                    }
                }
            }
        }
    }
    
    private void HideAllStars()
    {
        if (starImage1 != null) starImage1.SetActive(false);
        if (starImage2 != null) starImage2.SetActive(false);
        if (starImage3 != null) starImage3.SetActive(false);
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
    
    private void QuitGame()
    {
        // Return to main menu
        UnityEngine.SceneManagement.SceneManager.LoadScene("StartScene");
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
    
    // Public method to get current total score (for external access)
    public int GetTotalScore()
    {
        CalculateDetailedScore();
        return totalScore;
    }
}
