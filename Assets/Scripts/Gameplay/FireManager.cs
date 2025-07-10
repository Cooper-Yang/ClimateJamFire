using UnityEngine;
using System.Collections.Generic;

public class FireManager : MonoBehaviour
{
    
    private GridManager gridManager;
    private List<Tile> smokeTiles;
    public List<Tile> fireTiles;
    public GameObject firePrefab;
    public PhaseManager phaseManager;
    private void Awake()
    {
        gridManager = GetComponent<GridManager>();
    }

    private void Start()
    {
        if (phaseManager.currentPhase == Phase.ACTION)
        {
            StartFireSpread();
        }
    }

    private void StartFireSpread()
    {
        smokeTiles = gridManager.GetSmokeTiles();
        for (int i = smokeTiles.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Tile temp = smokeTiles[i];
            smokeTiles[i] = smokeTiles[j];
            smokeTiles[j] = temp;
        }
        smokeTiles[0].OnFire(firePrefab);
        fireTiles.Add(smokeTiles[0]);
        smokeTiles[1].OnFire(firePrefab);
        fireTiles.Add(smokeTiles[1]);
    }

}
