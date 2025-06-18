using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
public class AbilityCooldown : MonoBehaviour
{
    [SerializeField] private Button abilityButton; 
    [SerializeField] private Image cooldownImage;
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
    private void PutOnCooldown()
    {
        //This functions like clash royale, so 'cooldown' is fake. 
        //Cooldown image's fill should be based on the AP cost of the ability & current AP
    }    
}
