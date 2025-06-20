using UnityEngine;



[CreateAssetMenu(fileName = "GameplayTileDefinition", menuName = "Gameplay System/Gameplay Tile Definition")]
public class GameplayTileDefinition : ScriptableObject
{
    [Header("Basic Properties")]
    public TileType tileType;

    [Header("Fire Properties")]
    public bool canBurn = false;
    public float burnDuration = 5f;
 //   public TileState burnedState; // Reference to what this becomes when burned

    [Header("Movement")]
    public bool isWalkable = true;
    [Range(0.1f, 2f)]
    public float movementSpeedMultiplier = 1f;

    [Header("Actions")]
    public bool canBeCutDown = false;
 //   public TileState cutDownState; // Reference to what this becomes when cut

    [Header("Special Properties")]
    public bool isSpawnPoint = false;
    public bool isFireSource = false;
    public bool blocksMovementWhenBurning = true;
}
