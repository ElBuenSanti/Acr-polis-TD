using UnityEngine;

// Debug input used to test upgrade mode flow
public class UpgradeModeDebugInput : MonoBehaviour
{
    [SerializeField] private UpgradeModeSystem upgradeModeSystem;

    private void Update()
    {
        if (upgradeModeSystem == null || !upgradeModeSystem.IsUpgradeOpen)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            upgradeModeSystem.ConfirmUpgrade();
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            upgradeModeSystem.CancelUpgrade();
        }
    }
}