using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using static DG.Tweening.DOTweenAnimation;

public class Firefighter : MonoBehaviour
{
    public float moveTimePerTile = 1.5f;
    private float baseMoveTimePerTile; // Store original speed
    private Tile currentTile;
    private GridManager gmm;
    public float cutTime = 2.5f; // Time to cut a tree
    public bool hasFlameRetardantBuff = false;
    public bool usesFastExtinguish = false;
    internal Vector3 positionOffset;

    public GameObject progressBarPrefab;
    private Slider progressSlider;
    private GameObject progressBarInstance;

    /*private void Start()
    {
        GridManager gm = FindObjectOfType<GridManager>();
        currentTile = gm.GetTileAtCoord(transform.position);
        //HighlightCuttableTrees();
        TileClickManager.Instance.SetActiveFirefighter(this);
        if (currentTile == null)
        {
            Debug.Log("Firefighter could not find its current tile.");
        }
        else
        {
            Debug.Log($"Firefighter current tile: ({currentTile.gridX}, {currentTile.gridZ})");
        }
    }
    */

    public void Init(GridManager gridManager, Vector3 defaultPositionOffset, Phase phase = Phase.PREP)
    {
        baseMoveTimePerTile = moveTimePerTile;
        currentTile = gridManager.GetTileAtCoord(transform.position);
        gmm = gridManager;
        positionOffset = defaultPositionOffset;
        SetupProgressBar();

        if (currentTile == null)
        {
            Debug.LogError("Firefighter could not find its current tile in Init().");
            return;
        }

        if (phase == Phase.PREP)
        {
            HighlightCuttableTrees();
        }
    }

    private void HighlightCuttableTrees()
    {
        Debug.Log("HighlightCuttableTrees called");
        GridManager gm = FindAnyObjectByType<GridManager>();
        Tile fireStation = currentTile;

        foreach (Tile tile in FindObjectsByType<Tile>(FindObjectsSortMode.None))
        {
            if (tile.IsTileType(TileType.Tree))
            {
                List<Tile> neighbors = gm.GetAdjacentTiles(tile);
                bool hasPlainNeighbor = neighbors.Exists(n => n.IsTileType(TileType.Plain));

                if (hasPlainNeighbor && Pathfinding.Exists(fireStation, tile))
                {
                    tile.Highlight(true);
                }
            }
        }
    }

    public void MoveToAndCut(Tile targetTile)
    {
        StartCoroutine(MoveToTreeAndCut(targetTile));
    }

    IEnumerator MoveToTreeAndCut(Tile target)
    {
        Debug.Log($"Firefighter moving to tile ({target.gridX}, {target.gridZ})");
        List<Tile> path = Pathfinding.FindPath(currentTile, target);

        foreach (Tile step in path)
        {
            // Smooth movement to the target position
            Vector3 startPos = transform.position;
            Vector3 targetPos = step.gameObject.transform.position + positionOffset;

            // Calculate movement direction and rotate towards it
            Vector3 moveDirection = (targetPos - startPos).normalized;
            if (moveDirection != Vector3.zero)
            {
                // Try one of these rotation fixes:
                // Option 1: Flip the direction
                Quaternion targetRotation = Quaternion.LookRotation(-moveDirection);

                // Option 2: Add 180 degree rotation
                // Quaternion targetRotation = Quaternion.LookRotation(moveDirection) * Quaternion.Euler(0, 180, 0);

                // Option 3: Use different axis (if model faces different direction)
                // Quaternion targetRotation = Quaternion.LookRotation(Vector3.Cross(moveDirection, Vector3.up));

                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }

            float elapsedTime = 0f;
            while (elapsedTime < moveTimePerTile)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / moveTimePerTile;
                transform.position = Vector3.Lerp(startPos, targetPos, t);

                // Continue rotating during movement for smoother turns
                if (moveDirection != Vector3.zero)
                {
                    // Use the same rotation fix here
                    Quaternion targetRotation = Quaternion.LookRotation(-moveDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 8f);
                }

                yield return null;
            }

            // Ensure we end up exactly at the target position
            transform.position = targetPos;
            currentTile = step;
        }

        // Play tree chop sound when firefighter starts chopping
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayTreeChopSound();
        }

        yield return ShowProgressBar(cutTime);

        if (!target.IsTileType(TileType.Tree))
        {
            Debug.LogWarning("ReplaceTileWithPlain called on invalid tile.");
        }
        else
        {
            gmm.ReplaceTileWithPlain(target, gmm.choppedTilePrefab);
            gmm.numberOfTreesCutDownToPlains++;
            gmm.numberOfRemainingTree--;

            // Play despawn sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayDespawnSound();
            }

            Destroy(gameObject);
        }
    }

    // Speed Boost Methods
    public void ApplySpeedBoost(float speedMultiplier)
    {
        moveTimePerTile = baseMoveTimePerTile / (1 + speedMultiplier);
        Debug.Log($"Speed boost applied! Move time reduced from {baseMoveTimePerTile} to {moveTimePerTile}");
    }

    public void ResetSpeed()
    {
        moveTimePerTile = baseMoveTimePerTile;
        Debug.Log($"Speed reset to normal: {moveTimePerTile}");
    }

    public void BeginFirefightingMode()
    {
        Tile smokeTarget = FindClosestSmokeTile();
        if (smokeTarget != null)
        {
            StartCoroutine(MoveToAndExtinguish(smokeTarget));
        }
        else
        {
            Debug.Log("No smoke tile found for firefighter to move to.");

            // Play despawn sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayDespawnSound();
            }

            Destroy(gameObject);
        }
    }

    private Tile FindClosestSmokeTile()
    {
        List<Tile> smokeTiles = gmm.GetSmokeTiles();

        if (smokeTiles == null || smokeTiles.Count == 0)
        {
            Debug.Log("No smoke tiles found in grid manager.");
            return null;
        }

        Tile closest = null;
        float minDist = float.MaxValue;

        foreach (Tile smoke in smokeTiles)
        {
            Debug.Log($"Checking smoke tile at ({smoke.gridX}, {smoke.gridZ}) | isBurning: {smoke.IsBurning()}");

            if (!smoke.IsBurning()) continue;
            if (!Pathfinding.Exists(currentTile, smoke)) continue;

            float dist = Vector3.Distance(transform.position, smoke.transform.position);
            if (dist < minDist)
            {
                minDist = dist;
                closest = smoke;
            }
        }
        return closest;
    }

    private IEnumerator MoveToAndExtinguish(Tile target)
    {
        Transform targetTransform = target != null ? target.transform : null;

        List<Tile> path = Pathfinding.FindPath(currentTile, target);
        foreach (Tile step in path)
        {
            if (step == null || step.gameObject == null)
            {
                Debug.LogWarning("Firefighter path interrupted – step destroyed.");

                // Play despawn sound
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayDespawnSound();
                }

                Destroy(gameObject);
                yield break;
            }

            // Smooth movement to the target position
            Vector3 startPos = transform.position;
            Vector3 targetPos = step.transform.position + positionOffset;

            // Calculate movement direction and rotate towards it
            Vector3 moveDirection = (targetPos - startPos).normalized;
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(-moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
            }

            float elapsedTime = 0f;
            while (elapsedTime < moveTimePerTile)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / moveTimePerTile;
                transform.position = Vector3.Lerp(startPos, targetPos, t);

                // Continue rotating during movement for smoother turns
                if (moveDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(-moveDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 8f);
                }

                yield return null;
            }

            // Ensure we end up exactly at the target position
            transform.position = targetPos;
            currentTile = step;
        }

        // Play extinguish sound when firefighter starts extinguishing
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayExtinguishSound();
        }

        if (hasFlameRetardantBuff) usesFastExtinguish = true;
        float extinguishTime = usesFastExtinguish ? 5f : 10f;
        yield return ShowProgressBar(extinguishTime);

        if (target == null || targetTransform == null || targetTransform.gameObject == null)
        {
            Debug.LogWarning("Firefighter's target was destroyed mid-extinguish.");

            // Play despawn sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayDespawnSound();
            }

            Destroy(gameObject);
            yield break;
        }

        if (target.IsBurning())
        {
            gmm.ReplaceTileWithTree(target);
        }

        // Play despawn sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayDespawnSound();
        }

        Destroy(gameObject);
    }

    private void SetupProgressBar()
    {
        if (progressBarPrefab != null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas == null)
            {
                Debug.LogError("No Canvas found in scene.");
                return;
            }

            progressBarInstance = Instantiate(progressBarPrefab, canvas.transform);

            Billboard billboard = progressBarInstance.GetComponent<Billboard>();
            if (billboard != null)
            {
                billboard.targetWorldObject = this.transform;
            }

            progressSlider = progressBarInstance.GetComponentInChildren<Slider>();
            progressBarInstance.SetActive(false);
        }
    }

    private IEnumerator ShowProgressBar(float duration)
    {
        if (progressBarInstance == null || progressSlider == null) yield break;

        progressBarInstance.SetActive(true);
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            progressSlider.value = elapsed / duration;
            yield return null;
        }

        progressSlider.value = 1f;
        progressBarInstance.SetActive(false);
    }
}