using UnityEngine;
using UnityEngine.UI;
using TMPro; 
public class ScoreManager : MonoBehaviour
{
    [SerializeField] private GridManager gridmanager;
    [SerializeField] private int houseScore, treeScore, cutdownScore = 0;
    [SerializeField] private GameObject starImage1, starImage2, starImage3;
    private int totalScore; 
    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private int treeScoreFirstThreshold = 5; //Number of whole trees remaining required for 1 star
    [SerializeField] private int treeScoreSecondThreshold = 15; //2 stars
    [SerializeField] private int treeScoreThirdThreshold = 25; //3 stars
    public void UpdateScore()
    {
        houseScore = 300 * gridmanager.numberOfHouses;
        treeScore = 100 * gridmanager.numberOfRemainingTree;
        cutdownScore = 50 * gridmanager.numberOfTreesCutDownToPlains;

        totalScore = houseScore + treeScore + cutdownScore; 
        scoreText.text = totalScore.ToString();
    }

    public void ShowStars()
    {
        if(houseScore >= 300)
        {
            if(houseScore >= 600)
            {
                if(houseScore >= 900)
                {
                    if(totalScore >= treeScoreThirdThreshold * treeScore)
                    {
                        starImage1.SetActive(true);
                        starImage2.SetActive(true);
                        starImage3.SetActive(true);
                    }
                }
                starImage1.SetActive(true); 
                starImage2.SetActive(true);
                starImage3.SetActive(false);
            }
            starImage1.SetActive(true); 
            starImage2.SetActive(false);
            starImage3.SetActive(false);
        }
    }
}
