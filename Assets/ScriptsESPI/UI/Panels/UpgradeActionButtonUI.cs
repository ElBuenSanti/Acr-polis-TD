using UnityEngine;
using UnityEngine.UI;

// Handles the upgrade action button behavior
public class UpgradeActionButtonUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button upgradeButton;
    [SerializeField] private CooldownSystem cooldownSystem;
    [SerializeField] private SelectionSystem selectionSystem;
    [SerializeField] private UpgradeModeSystem upgradeModeSystem;

    private void Start()
    {
        if (upgradeButton != null)
        {
            upgradeButton.onClick.AddListener(OnUpgradeButtonPressed);
        }
    }

    private void OnDestroy()
    {
        if (upgradeButton != null)
        {
            upgradeButton.onClick.RemoveListener(OnUpgradeButtonPressed);
        }
    }

    // Called when the upgrade button is pressed
    private void OnUpgradeButtonPressed()
    {
        if (GameStateManager.Instance == null)
        {
            Debug.LogWarning("UpgradeActionButtonUI: GameStateManager instance is missing.");
            return;
        }

        if (cooldownSystem == null)
        {
            Debug.LogWarning("UpgradeActionButtonUI: CooldownSystem reference is missing.");
            return;
        }

        if (selectionSystem == null || !selectionSystem.HasSelection())
        {
            Debug.Log("Upgrade action cancelled: no structure is selected.");
            return;
        }

        if (upgradeModeSystem == null)
        {
            Debug.LogWarning("UpgradeActionButtonUI: UpgradeModeSystem reference is missing.");
            return;
        }

        GameState currentState = GameStateManager.Instance.CurrentState;

        if (!cooldownSystem.CanUse(CooldownType.UpgradeStructure))
        {
            Debug.Log("Upgrade action is currently on cooldown.");
            return;
        }

        Debug.Log("Upgrade action triggered for selected structure.");

        if (currentState == GameState.Combat)
        {
            cooldownSystem.StartCooldown(CooldownType.UpgradeStructure);
        }

        upgradeModeSystem.OpenUpgradeMode();
    }
}