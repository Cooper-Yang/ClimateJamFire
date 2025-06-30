using UnityEngine;

public class FireManager : MonoBehaviour
{
    private GridManager gridManager;

    private void Awake()
    {
        gridManager = GetComponent<GridManager>();
    }

    private void Start()
    {
        if (gridManager.state == GameState.Action)
        {
            OnFireSpread();
        }
    }

    private void OnFireSpread()
    {

    }

    private void Update()
    {
        
    }
}
