using UnityEngine;
using UnityEngine.EventSystems;

// Handles the basic flow for moving a selected structure
public class MoveModeSystem : MonoBehaviour
{
    public bool IsMoving { get; private set; }
    public Vector3 CurrentMovePreviewPosition { get; private set; }

    [Header("References")]
    [SerializeField] private SelectionSystem selectionSystem;
    [SerializeField] private MovePreview movePreview;
    [SerializeField] private HUDNavigationUI actionPanelNavigationUI;

    [Header("Preview Settings")]
    [SerializeField] private Vector3 defaultMovePreviewPosition = new Vector3(2f, 0f, 0f);

    [Header("Input Protection")]
    [SerializeField] private float inputBlockDuration = 0.15f;

    private ISelectable selectedStructure;
    private MonoBehaviour selectedStructureBehaviour;
    private GameObject selectedStructureObject;
    private GameState returnStateAfterMove = GameState.Preparation;
    private float inputUnlockTime;

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

        if (movePreview == null)
        {
            Debug.LogWarning("MoveModeSystem: MovePreview reference is missing.");
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
        selectedStructureBehaviour = selectedStructure as MonoBehaviour;

        if (selectedStructureBehaviour == null)
        {
            Debug.LogWarning("Move mode cancelled: selected structure is not a MonoBehaviour.");
            return;
        }

        selectedStructureObject = selectedStructureBehaviour.gameObject;

        IsMoving = true;
        inputUnlockTime = Time.unscaledTime + inputBlockDuration;

        CurrentMovePreviewPosition = selectedStructureObject.transform.position;

        movePreview.gameObject.SetActive(true);
        movePreview.SetPosition(CurrentMovePreviewPosition);

        // Hide the original structure during move mode to avoid duplicate visuals
        selectedStructureObject.SetActive(false);

        // Clear current UI focus so movement keys control the preview instead of the menu
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        Debug.Log("Move mode started for selected structure at: " + CurrentMovePreviewPosition);
        GameStateManager.Instance.ChangeState(GameState.MoveStructure);
    }

    // Confirm the move action
    public void ConfirmMove()
    {
        if (!IsMoving)
        {
            return;
        }

        if (!CanReceiveMoveInput())
        {
            return;
        }

        if (selectedStructureObject != null && movePreview != null)
        {
            selectedStructureObject.transform.position = CurrentMovePreviewPosition;
            selectedStructureObject.SetActive(true);
        }

        Debug.Log("Move confirmed for selected structure.");

        GameState nextState = returnStateAfterMove;
        ResetMoveMode();

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(nextState);
        }

        RestoreActionPanelFocus();
    }

    // Cancel the move action
    public void CancelMove()
    {
        if (!IsMoving)
        {
            return;
        }

        if (!CanReceiveMoveInput())
        {
            return;
        }

        if (selectedStructureObject != null)
        {
            selectedStructureObject.SetActive(true);
        }

        Debug.Log("Move cancelled for selected structure.");

        GameState nextState = returnStateAfterMove;
        ResetMoveMode();

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(nextState);
        }

        RestoreActionPanelFocus();
    }

    // Move the preview to a new absolute position
    public void MovePreviewTo(Vector3 newPosition)
    {
        if (!IsMoving || movePreview == null)
        {
            return;
        }

        if (!CanReceiveMoveInput())
        {
            return;
        }

        CurrentMovePreviewPosition = newPosition;
        movePreview.SetPosition(CurrentMovePreviewPosition);
    }

    // Move the preview by offset
    public void MovePreviewBy(Vector3 offset)
    {
        if (!IsMoving || movePreview == null)
        {
            return;
        }

        if (!CanReceiveMoveInput())
        {
            return;
        }

        CurrentMovePreviewPosition += offset;
        movePreview.SetPosition(CurrentMovePreviewPosition);

        Debug.Log("Move preview moved to: " + CurrentMovePreviewPosition);
    }

    // Return true when move input can be used
    public bool CanReceiveMoveInput()
    {
        return IsMoving && Time.unscaledTime >= inputUnlockTime;
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

            if (selectedStructureObject != null)
            {
                selectedStructureObject.SetActive(true);
            }

            ResetMoveMode();
        }
    }

    // Restore default focus to the action panel after exiting move mode
    private void RestoreActionPanelFocus()
    {
        if (actionPanelNavigationUI != null)
        {
            actionPanelNavigationUI.SelectDefault();
        }
    }

    // Reset the move mode state
    private void ResetMoveMode()
    {
        IsMoving = false;
        CurrentMovePreviewPosition = defaultMovePreviewPosition;
        selectedStructure = null;
        selectedStructureBehaviour = null;
        selectedStructureObject = null;
        returnStateAfterMove = GameState.Preparation;
        inputUnlockTime = 0f;

        if (movePreview != null)
        {
            movePreview.gameObject.SetActive(false);
        }
    }
}