using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class AbilityCooldown : MonoBehaviour
{
    [SerializeField] private ActionPoint AP;
    [SerializeField] private Button abilityButton;
    [SerializeField] private Image cooldownImage;
    [SerializeField] private int abilityCost = 0;
    private bool onCooldown = false;
    private float globalCooldown = 2.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cooldownImage.fillAmount = 0.0f;
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
    private void CooldownWheel()
    {
        if(AP.currentActionPoint/abilityCost < 1)
        {
            cooldownImage.fillAmount = AP.actionPointSlider.value / abilityCost; 
        }
    }

    private void Update()
    {
        CooldownWheel(); 
    }

}
