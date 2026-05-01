//using System.Collections;
//using UnityEngine;

//public class GridSelector : MonoBehaviour
//{
//    [Header("Selection")]
//    public Tile currentTile;

//    [Header("Movement")]
//    [SerializeField] private float moveCooldown = 0.22f;
//    [SerializeField] private float inputDeadZone = 0.55f;

//    private float lastMoveTime;

//    private IEnumerator Start()
//    {
//        yield return new WaitUntil(() => Tile.AllTiles != null && Tile.AllTiles.Count > 0);

//        currentTile = FindFirstTile();
//        UpdateHover();

//        Debug.Log("GridSelector started on tile: " + currentTile.name);
//    }

//    public void Move(Vector2 input)
//    {
//        if (currentTile == null)
//        {
//            Debug.LogWarning("No current tile selected.");
//            return;
//        }

//        if (Time.time < lastMoveTime + moveCooldown)
//            return;

//        if (input.magnitude < inputDeadZone)
//            return;

//        Tile nextTile = GetBestNeighbor(input);

//        if (nextTile == null)
//        {
//            Debug.Log("No neighbor found.");
//            return;
//        }

//        currentTile = nextTile;
//        lastMoveTime = Time.time;

//        UpdateHover();

//        Debug.Log("Moved to tile: " + currentTile.name);
//    }

//    private Tile FindFirstTile()
//    {
//        return Tile.AllTiles[0];
//    }

//    private Tile GetBestNeighbor(Vector2 input)
//    {
//        Tile bestTile = null;
//        float bestScore = 0.45f;

//        foreach (Tile tile in currentTile.allNeighbors)
//        {
//            if (tile == null)
//                continue;

//            Vector3 direction = tile.transform.position - currentTile.transform.position;
//            Vector2 direction2D = new Vector2(direction.x, direction.z).normalized;

//            float score = Vector2.Dot(input.normalized, direction2D);

//            if (score > bestScore)
//            {
//                bestScore = score;
//                bestTile = tile;
//            }
//        }

//        return bestTile;
//    }

//    private void UpdateHover()
//    {
//        if (currentTile == null)
//            return;

//        BuildingManager.Instance.SetHoveredTile(currentTile);
//    }
//}


using System.Collections;
using UnityEngine;

public class GridSelector : MonoBehaviour
{
    [Header("Selection")]
    public Tile currentTile;

    [Header("Movement")]
    [SerializeField] private float moveCooldown = 0.28f;
    [SerializeField] private float inputDeadZone = 0.75f;

    [Header("Camera Reference")]
    [SerializeField] private Camera mainCamera;

    private float lastMoveTime;

    private IEnumerator Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        yield return new WaitUntil(() => Tile.AllTiles != null && Tile.AllTiles.Count > 0);

        currentTile = FindFirstTile();
        UpdateHover();

        Debug.Log("GridSelector started on tile: " + currentTile.name);
    }

    public void Move(Vector2 input)
    {
        if (currentTile == null)
            return;

        if (Time.time < lastMoveTime + moveCooldown)
            return;

        if (input.magnitude < inputDeadZone)
            return;

        Tile nextTile = GetBestNeighbor(input);

        if (nextTile == null)
            return;

        currentTile = nextTile;
        lastMoveTime = Time.time;

        UpdateHover();

        Debug.Log("Moved to tile: " + currentTile.name);
    }

    private Tile FindFirstTile()
    {
        return Tile.AllTiles[0];
    }

    private Tile GetBestNeighbor(Vector2 input)
    {
        Vector3 desiredDirection = GetCameraRelativeDirection(input);

        Tile bestTile = null;
        float bestScore = 0.45f;

        foreach (Tile tile in currentTile.allNeighbors)
        {
            if (tile == null)
                continue;

            Vector3 tileDirection = tile.transform.position - currentTile.transform.position;
            tileDirection.y = 0f;
            tileDirection.Normalize();

            float score = Vector3.Dot(desiredDirection, tileDirection);

            if (score > bestScore)
            {
                bestScore = score;
                bestTile = tile;
            }
        }

        return bestTile;
    }

    private Vector3 GetCameraRelativeDirection(Vector2 input)
    {
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 direction = cameraRight * input.x + cameraForward * input.y;
        direction.Normalize();

        return direction;
    }

    private void UpdateHover()
    {
        if (currentTile == null)
            return;

        if (GameStateController.Instance.currentState == GameState.PlacingTower ||
            GameStateController.Instance.currentState == GameState.MovingTower)
        {
            BuildingManager.Instance.SetHoveredTile(currentTile);
        }
        else
        {
            BuildingManager.Instance.ClearHover();
        }
    }
}