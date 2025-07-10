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
    private Ability ability;
    private TooltipHover tooltipHover;
    private bool onCooldown = false;
    [SerializeField] private TextMeshProUGUI abilityCostText; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        tooltipHover = GetComponent<TooltipHover>();
        ability = tooltipHover.ability;
        cooldownImage.fillAmount = 0.0f;
        //abilityCost = ability.abilityCost;
        abilityCostText.text = ability.abilityCost.ToString(); 
    }

    public void ActivateAbility(int APCost)
    {
        StartCoroutine(OnAbilityCooldown());
        //Convert later to a virtual function for different abilities(?) 
    }

    IEnumerator OnAbilityCooldown()
    {
        onCooldown = true;
        abilityButton.enabled = false;
        abilityButton.image.color = Color.gray;
        yield return new WaitForSeconds(ability.abilityCooldown);
        abilityButton.enabled = true;
        onCooldown = false;
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
