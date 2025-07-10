using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public enum Phase { PREP, ACTION, SCORE };
public class PhaseManager : MonoBehaviour
{

    [SerializeField] public Phase currentPhase;

    //UI Elements to activate/deactivate: phase panel, fire progress bar, timer, ability buttons, final score panel
    [SerializeField] private GameObject phasePanel, fireProgressBar, timer, finalScorePanel;
    [SerializeField] private TextMeshProUGUI phaseText;
    [SerializeField] private GameObject phasePanelWarningObjects; 
    [SerializeField] private Button[] actionAbilityButtons;  //Ability buttons that should only be active during action phase 

    private void Start()
    {
        StartPrep(); 
    }

    public void StartPrep()
    {
        currentPhase = Phase.PREP;
        fireProgressBar.SetActive(false);
        timer.SetActive(true);
        finalScorePanel.SetActive(false); 
        foreach(Button button in actionAbilityButtons)
        {
            button.enabled = false; 
        }
        IEnumerator displayCoroutine = DisplayPhasePanel(); 
        StartCoroutine(displayCoroutine); 

    }

    IEnumerator DisplayPhasePanel()
    {
        phasePanel.SetActive(true); 
        if(currentPhase == Phase.PREP)
        {
            phaseText.text = "Prep Phase";
            phasePanelWarningObjects.SetActive(false); 
        }
        else if(currentPhase == Phase.ACTION) 
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
    }
    public void TransitionToAction()
    {
        currentPhase = Phase.ACTION;
        fireProgressBar.SetActive(true);
        timer.SetActive(false);
        finalScorePanel.SetActive(false);
        foreach (Button button in actionAbilityButtons)
        {
            button.enabled = true;
        }
        IEnumerator displayCoroutine = DisplayPhasePanel();
        StartCoroutine(displayCoroutine);
    }

    public void TransitionToScore()
    {
        currentPhase = Phase.SCORE;
        fireProgressBar.SetActive(false); 
        timer.SetActive(false);
        finalScorePanel.SetActive(true);
        IEnumerator displayCoroutine = DisplayPhasePanel();
        StartCoroutine(displayCoroutine);
    }
}
