using UnityEngine;
using UnityEngine.UI;

// Handles the move action button behavior
public class MoveActionButtonUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button moveButton;
    [SerializeField] private CooldownSystem cooldownSystem;
    [SerializeField] private SelectionSystem selectionSystem;
    [SerializeField] private MoveModeSystem moveModeSystem;

    private void Start()
    {
        if (moveButton != null)
        {
            moveButton.onClick.AddListener(OnMoveButtonPressed);
        }
    }

    private void OnDestroy()
    {
        if (moveButton != null)
        {
            moveButton.onClick.RemoveListener(OnMoveButtonPressed);
        }
    }

    // Called when the move button is pressed
    private void OnMoveButtonPressed()
    {
        if (GameStateManager.Instance == null)
        {
            Debug.LogWarning("MoveActionButtonUI: GameStateManager instance is missing.");
            return;
        }

        if (cooldownSystem == null)
        {
            Debug.LogWarning("MoveActionButtonUI: CooldownSystem reference is missing.");
            return;
        }

        if (selectionSystem == null || !selectionSystem.HasSelection())
        {
            Debug.Log("Move action cancelled: no structure is selected.");
            return;
        }

        if (moveModeSystem == null)
        {
            Debug.LogWarning("MoveActionButtonUI: MoveModeSystem reference is missing.");
            return;
        }

        GameState currentState = GameStateManager.Instance.CurrentState;

        if (!cooldownSystem.CanUse(CooldownType.MoveStructure))
        {
            Debug.Log("Move action is currently on cooldown.");
            return;
        }

        Debug.Log("Move action triggered for selected structure.");

        if (currentState == GameState.Combat)
        {
            cooldownSystem.StartCooldown(CooldownType.MoveStructure);
        }

        moveModeSystem.StartMoveMode();
    }
}