using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Runtime.CompilerServices;
public class AbilityCooldown : MonoBehaviour
{
    [SerializeField] private ActionPoint AP;
    [SerializeField] private Button abilityButton;
    [SerializeField] private Image cooldownImage;
    //[SerializeField] private Ability ability; 
    private bool onCooldown = false;
    private float globalCooldown = 2.0f;
    private int abilityCost;
    [SerializeField] private TextMeshProUGUI abilityCostText; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cooldownImage.fillAmount = 0.0f;
        //abilityCost = ability.abilityCost;
        abilityCostText.text = abilityCost.ToString(); 
    }

    public void ActivateAbility(int APCost)
    {
        onCooldown = true;
        StartCoroutine(GlobalCooldown());
        //Convert later to a virtual function for different abilities(?) 
    }

    IEnumerator GlobalCooldown()
    {
        abilityButton.enabled = false;
        abilityButton.image.color = Color.gray;
        yield return new WaitForSeconds(globalCooldown);
        abilityButton.enabled = true;
    }
    /*private void CooldownWheel()
    {
        if(AP.currentActionPoint/abilityCost < 1)
        {
            cooldownImage.fillAmount = AP.actionPointSlider.value / abilityCost; 
        }
    }

    private void Update()
    {
        CooldownWheel(); 
    }*/

}
