using UnityEngine;

// Handles the basic building placement flow
public class BuildPlacementSystem : MonoBehaviour
{
    public BuildType CurrentBuildType { get; private set; }
    public bool IsPlacing { get; private set; }

    [Header("Preview Reference")]
    [SerializeField] private BuildPreview buildPreview;

    [Header("Preview Settings")]
    [SerializeField] private Vector3 defaultPreviewPosition = new Vector3(0f, 0f, 0f);

    private GameState returnStateAfterPlacement = GameState.Preparation;

    private void OnEnable()
    {
        GameEvents.StartListening(EventNames.StateChanged, OnStateChanged);
    }

    private void OnDisable()
    {
        GameEvents.StopListening(EventNames.StateChanged, OnStateChanged);
    }

    private void Start()
    {
        ResetPlacement();
    }

    // Start placing a selected build type
    public void StartPlacement(BuildType buildType)
    {
        if (GameStateManager.Instance == null)
        {
            Debug.LogWarning("BuildPlacementSystem: GameStateManager instance is missing.");
            return;
        }

        if (!CanStartPlacement())
        {
            Debug.Log("Build placement cannot start in the current state.");
            return;
        }

        GameState currentState = GameStateManager.Instance.CurrentState;

        if (currentState == GameState.Combat)
        {
            returnStateAfterPlacement = GameState.Combat;
        }
        else
        {
            returnStateAfterPlacement = GameState.Preparation;
        }

        CurrentBuildType = buildType;
        IsPlacing = true;

        if (buildPreview != null)
        {
            buildPreview.gameObject.SetActive(true);
            buildPreview.SetBuildType(buildType);
            buildPreview.SetPosition(defaultPreviewPosition);
        }

        Debug.Log("Started placement for: " + buildType);
        GameStateManager.Instance.ChangeState(GameState.BuildingPlacement);
    }

    // Confirm the current placement
    public void ConfirmPlacement()
    {
        if (!IsPlacing)
        {
            return;
        }

        Debug.Log("Placement confirmed for: " + CurrentBuildType);

        GameState nextState = returnStateAfterPlacement;
        ResetPlacement();

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(nextState);
        }
    }

    // Cancel the current placement
    public void CancelPlacement()
    {
        if (!IsPlacing)
        {
            return;
        }

        Debug.Log("Placement cancelled for: " + CurrentBuildType);

        GameState nextState = returnStateAfterPlacement;
        ResetPlacement();

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(nextState);
        }
    }

    // Move the preview object to a new world position
    public void MovePreview(Vector3 newPosition)
    {
        if (!IsPlacing || buildPreview == null)
        {
            return;
        }

        buildPreview.SetPosition(newPosition);
    }

    // Check if placement can begin in the current game state
    private bool CanStartPlacement()
    {
        GameState currentState = GameStateManager.Instance.CurrentState;

        return currentState == GameState.Preparation ||
               currentState == GameState.StructureSelected ||
               currentState == GameState.Combat;
    }

    // Called when the game state changes
    private void OnStateChanged(object eventData)
    {
        if (!(eventData is GameState newState))
        {
            return;
        }

        HandleStateChanged(newState);
    }

    // React to the current game state and cancel invalid placement flows
    private void HandleStateChanged(GameState newState)
    {
        if (!IsPlacing)
        {
            return;
        }

        bool shouldStopPlacement =
            newState == GameState.Paused ||
            newState == GameState.BlessingSelection ||
            newState == GameState.Victory ||
            newState == GameState.Defeat;

        if (shouldStopPlacement)
        {
            Debug.Log("Build placement stopped because of state change: " + newState);
            ResetPlacement();
        }
    }

    // Reset the current placement state
    private void ResetPlacement()
    {
        IsPlacing = false;
        CurrentBuildType = default;
        returnStateAfterPlacement = GameState.Preparation;

        if (buildPreview != null)
        {
            buildPreview.gameObject.SetActive(false);
        }
    }
}