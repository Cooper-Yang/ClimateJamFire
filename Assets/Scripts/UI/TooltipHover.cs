using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; 
public class TooltipHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject tooltipObject;
    [SerializeField] public Ability ability; 
    [SerializeField] private TextMeshProUGUI abilityTitle;
    [SerializeField] private TextMeshProUGUI abilityDescription;

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltipObject.SetActive(true);
        UpdateTooltip(); 
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipObject.SetActive(false); 
    }

    public void UpdateTooltip()
    {
        abilityTitle.text = ability.abilityName; 
        abilityDescription.text = ability.abilityDescription;
    }
}
