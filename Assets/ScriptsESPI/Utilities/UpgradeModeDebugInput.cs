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

        // Let UI submit / radial selection handle Enter.
        // This script no longer uses Backspace to avoid conflicts.
    }
}