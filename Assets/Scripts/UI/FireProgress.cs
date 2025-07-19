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
        int burned = fireManager.burnedTileCount;
        int total = gridManager.totalBurnableTiles;

        if (total > 0)
        {
            fireProgressSlider.value = (float)burned / total;
        }
        else
        {
            fireProgressSlider.value = 0f;
        }
    }
}
