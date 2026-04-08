using UnityEngine;

// Debug input used to test radial menu flow
public class RadialMenuDebugInput : MonoBehaviour
{
    [SerializeField] private RadialMenuUI radialMenuUI;
    [SerializeField] private UpgradeModeSystem upgradeModeSystem;

    private void Update()
    {
        if (radialMenuUI == null)
        {
            Debug.LogWarning("RadialMenuDebugInput: RadialMenuUI reference is missing.");
            return;
        }

        if (upgradeModeSystem == null)
        {
            Debug.LogWarning("RadialMenuDebugInput: UpgradeModeSystem reference is missing.");
            return;
        }

        if (GameStateManager.Instance == null)
        {
            Debug.LogWarning("RadialMenuDebugInput: GameStateManager instance is missing.");
            return;
        }

        bool radialStateActive = GameStateManager.Instance.CurrentState == GameState.RadialUpgradeOpen;
        bool upgradeFlowActive = upgradeModeSystem.IsUpgradeOpen;

        if (!radialStateActive && !upgradeFlowActive)
        {
            return;
        }

        // N cancels the whole upgrade flow
        if (Input.GetKeyDown(KeyCode.N))
        {
            Debug.Log("RadialMenuDebugInput: N pressed. Cancelling radial upgrade.");

            radialMenuUI.CancelSelection();
            upgradeModeSystem.CancelUpgrade();
        }
    }
}