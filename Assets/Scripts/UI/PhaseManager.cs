using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public enum Phase { PREP, ACTION, SCORE };
public class PhaseManager : MonoBehaviour
{

    [SerializeField] public Phase currentPhase;
    [SerializeField] private FireManager fireManager;
    //UI Elements to activate/deactivate: phase panel, fire progress bar, timer, ability buttons, final score panel
    [SerializeField] private GameObject phasePanel, fireProgressBar, timer, finalScorePanel;
    [SerializeField] private TextMeshProUGUI phaseText;
    [SerializeField] private GameObject phasePanelWarningObjects;
    [SerializeField] private Button firefighterButton;
    [SerializeField] private Button speedBoostButton;
    [SerializeField] private Button fireRetardantButton;
    [SerializeField] private Button waterTankerButton;
    [SerializeField] private Button breakLineButton;

    private void Start()
    {
        StartPrep(); 
    }


    public void StartPrep()
    {
        currentPhase = Phase.PREP;
        fireProgressBar.SetActive(false);
        finalScorePanel.SetActive(false);

        speedBoostButton.gameObject.SetActive(false);
        fireRetardantButton.gameObject.SetActive(false);
        waterTankerButton.gameObject.SetActive(false);

        IEnumerator displayCoroutine = DisplayPhasePanel(); 
        StartCoroutine(displayCoroutine); 

    }

    IEnumerator DisplayPhasePanel()
    {
        phasePanel.SetActive(true);
        if (currentPhase == Phase.PREP)
        {
            phaseText.text = "Prep Phase";
            phasePanelWarningObjects.SetActive(false);
        }
        else if (currentPhase == Phase.ACTION)
        {
            phaseText.text = "Action Phase";
            phasePanelWarningObjects.SetActive(true);
        }
        else
        {
            phaseText.text = "Final Results";
            phasePanelWarningObjects.SetActive(false);
        }
        yield return new WaitForSecondsRealtime(2.0f);
        phasePanel.SetActive(false);
        if (currentPhase == Phase.PREP)
        {
            timer.SetActive(true);
        }
        else if (currentPhase == Phase.ACTION)
        {
            fireProgressBar.SetActive(true);
            fireManager.StartFireSpread();
        }
        else
        {
            finalScorePanel.SetActive(true);
        }
    }

    public void TransitionToAction()
    {
        currentPhase = Phase.ACTION;

        speedBoostButton.gameObject.SetActive(true);
        fireRetardantButton.gameObject.SetActive(true);
        waterTankerButton.gameObject.SetActive(true);


        timer.SetActive(false);
        finalScorePanel.SetActive(false);
        IEnumerator displayCoroutine = DisplayPhasePanel();
        StartCoroutine(displayCoroutine);
    }

    public void TransitionToScore()
    {
        currentPhase = Phase.SCORE;
        fireProgressBar.SetActive(false); 
        timer.SetActive(false);
        IEnumerator displayCoroutine = DisplayPhasePanel();
        StartCoroutine(displayCoroutine);
    }
}
