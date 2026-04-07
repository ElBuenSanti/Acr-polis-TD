using UnityEngine;

// Handles the basic upgrade flow for the selected structure
public class UpgradeModeSystem : MonoBehaviour
{
    public bool IsUpgradeOpen { get; private set; }

    [Header("References")]
    [SerializeField] private SelectionSystem selectionSystem;

    private ISelectable selectedStructure;
    private GameState returnStateAfterUpgrade = GameState.Preparation;

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
        ResetUpgradeMode();
    }

    // Open upgrade mode for the currently selected structure
    public void OpenUpgradeMode()
    {
        if (selectionSystem == null || !selectionSystem.HasSelection())
        {
            Debug.Log("Upgrade mode cancelled: no structure is selected.");
            return;
        }

        if (GameStateManager.Instance == null)
        {
            Debug.LogWarning("UpgradeModeSystem: GameStateManager instance is missing.");
            return;
        }

        GameState currentState = GameStateManager.Instance.CurrentState;

        if (currentState == GameState.Combat)
        {
            returnStateAfterUpgrade = GameState.Combat;
        }
        else
        {
            returnStateAfterUpgrade = GameState.StructureSelected;
        }

        selectedStructure = selectionSystem.CurrentSelected;
        IsUpgradeOpen = true;

        Debug.Log("Upgrade mode opened for selected structure.");
        GameStateManager.Instance.ChangeState(GameState.RadialUpgradeOpen);
    }

    // Confirm the upgrade selection
    public void ConfirmUpgrade()
    {
        if (!IsUpgradeOpen)
        {
            return;
        }

        Debug.Log("Upgrade confirmed for selected structure.");

        GameState nextState = returnStateAfterUpgrade;
        ResetUpgradeMode();

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(nextState);
        }
    }

    // Cancel the current upgrade flow
    public void CancelUpgrade()
    {
        if (!IsUpgradeOpen)
        {
            return;
        }

        Debug.Log("Upgrade cancelled for selected structure.");

        GameState nextState = returnStateAfterUpgrade;
        ResetUpgradeMode();

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(nextState);
        }
    }

    // Stop upgrade mode if the game enters an incompatible state
    private void OnStateChanged(object eventData)
    {
        if (!(eventData is GameState newState))
        {
            return;
        }

        if (!IsUpgradeOpen)
        {
            return;
        }

        bool shouldCloseUpgrade =
            newState == GameState.Paused ||
            newState == GameState.BlessingSelection ||
            newState == GameState.Victory ||
            newState == GameState.Defeat;

        if (shouldCloseUpgrade)
        {
            Debug.Log("Upgrade mode closed because of state change: " + newState);
            ResetUpgradeMode();
        }
    }

    // Reset the upgrade mode state
    private void ResetUpgradeMode()
    {
        IsUpgradeOpen = false;
        selectedStructure = null;
        returnStateAfterUpgrade = GameState.Preparation;
    }
}