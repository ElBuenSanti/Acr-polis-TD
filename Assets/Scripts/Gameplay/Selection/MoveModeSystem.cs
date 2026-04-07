using UnityEngine;

// Handles the basic flow for moving a selected structure
public class MoveModeSystem : MonoBehaviour
{
    public bool IsMoving { get; private set; }

    [Header("References")]
    [SerializeField] private SelectionSystem selectionSystem;
    [SerializeField] private MovePreview movePreview;

    [Header("Preview Settings")]
    [SerializeField] private Vector3 defaultMovePreviewPosition = new Vector3(2f, 0f, 0f);

    private ISelectable selectedStructure;
    private GameState returnStateAfterMove = GameState.Preparation;

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
        ResetMoveMode();
    }

    // Start move mode for the currently selected structure
    public void StartMoveMode()
    {
        if (selectionSystem == null || !selectionSystem.HasSelection())
        {
            Debug.Log("Move mode cancelled: no structure is selected.");
            return;
        }

        if (GameStateManager.Instance == null)
        {
            Debug.LogWarning("MoveModeSystem: GameStateManager instance is missing.");
            return;
        }

        GameState currentState = GameStateManager.Instance.CurrentState;

        if (currentState == GameState.Combat)
        {
            returnStateAfterMove = GameState.Combat;
        }
        else
        {
            returnStateAfterMove = GameState.StructureSelected;
        }

        selectedStructure = selectionSystem.CurrentSelected;
        IsMoving = true;

        if (movePreview != null)
        {
            movePreview.gameObject.SetActive(true);
            movePreview.SetPosition(defaultMovePreviewPosition);
        }

        Debug.Log("Move mode started for selected structure.");
        GameStateManager.Instance.ChangeState(GameState.BuildingPlacement);
    }

    // Confirm the move action
    public void ConfirmMove()
    {
        if (!IsMoving)
        {
            return;
        }

        Debug.Log("Move confirmed for selected structure.");

        GameState nextState = returnStateAfterMove;
        ResetMoveMode();

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(nextState);
        }
    }

    // Cancel the move action
    public void CancelMove()
    {
        if (!IsMoving)
        {
            return;
        }

        Debug.Log("Move cancelled for selected structure.");

        GameState nextState = returnStateAfterMove;
        ResetMoveMode();

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(nextState);
        }
    }

    // Move the preview to a new position
    public void MovePreviewTo(Vector3 newPosition)
    {
        if (!IsMoving || movePreview == null)
        {
            return;
        }

        movePreview.SetPosition(newPosition);
    }

    // React to state changes and stop move mode if needed
    private void OnStateChanged(object eventData)
    {
        if (!(eventData is GameState newState))
        {
            return;
        }

        if (!IsMoving)
        {
            return;
        }

        bool shouldStopMove =
            newState == GameState.Paused ||
            newState == GameState.BlessingSelection ||
            newState == GameState.Victory ||
            newState == GameState.Defeat;

        if (shouldStopMove)
        {
            Debug.Log("Move mode stopped because of state change: " + newState);
            ResetMoveMode();
        }
    }

    // Reset the move mode state
    private void ResetMoveMode()
    {
        IsMoving = false;
        selectedStructure = null;
        returnStateAfterMove = GameState.Preparation;

        if (movePreview != null)
        {
            movePreview.gameObject.SetActive(false);
        }
    }
}