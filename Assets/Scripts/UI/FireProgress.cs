using UnityEngine;
using UnityEngine.UI; 
public class FireProgress : MonoBehaviour
{
    [SerializeField] private Slider fireProgressSlider;
    [SerializeField] private FireManager fireManager;
    [SerializeField] private GridManager gridManager; 

    // Update is called once per frame
    void Update()
    {
        int fire = fireManager.fireTiles.Count;
        int trees = gridManager.numberOfRemainingTree;
        fireProgressSlider.value = fire / trees; 
    }
}
