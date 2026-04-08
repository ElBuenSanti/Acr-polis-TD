using System.Collections.Generic;
using UnityEngine;

// Handles upgrade business logic for the currently selected structure
public class StructureUpgradeSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SelectionSystem selectionSystem;
    [SerializeField] private ResourceSystem resourceSystem;

    // Return true if the currently selected structure has valid upgrade data
    public bool TryGetSelectedUpgradeData(out MonoBehaviour selectedBehaviour, out StructureUpgradeProgress upgradeProgress, out StructureUpgradeCosts upgradeCosts)
    {
        selectedBehaviour = null;
        upgradeProgress = null;
        upgradeCosts = null;

        if (selectionSystem == null || !selectionSystem.HasSelection())
        {
            return false;
        }

        selectedBehaviour = selectionSystem.CurrentSelected as MonoBehaviour;

        if (selectedBehaviour == null)
        {
            return false;
        }

        upgradeProgress = selectedBehaviour.GetComponent<StructureUpgradeProgress>();
        upgradeCosts = selectedBehaviour.GetComponent<StructureUpgradeCosts>();

        if (upgradeProgress == null)
        {
            Debug.LogWarning("StructureUpgradeSystem: Selected structure is missing StructureUpgradeProgress.");
            return false;
        }

        if (upgradeCosts == null)
        {
            Debug.LogWarning("StructureUpgradeSystem: Selected structure is missing StructureUpgradeCosts.");
            return false;
        }

        return true;
    }

    // Get the current preview data for a branch upgrade
    public bool TryGetBranchPreviewData(
        UpgradeBranchType branchType,
        out int currentLevel,
        out bool isLocked,
        out bool isMaxed,
        out List<ResourceAmount> costList,
        out bool canAfford)
    {
        currentLevel = 0;
        isLocked = false;
        isMaxed = false;
        costList = null;
        canAfford = false;

        if (!TryGetSelectedUpgradeData(out _, out StructureUpgradeProgress upgradeProgress, out StructureUpgradeCosts upgradeCosts))
        {
            return false;
        }

        currentLevel = upgradeProgress.GetBranchLevel(branchType);
        isLocked = upgradeProgress.IsBranchLocked(branchType);
        isMaxed = currentLevel >= 3;

        if (isLocked || isMaxed)
        {
            canAfford = false;
            return true;
        }

        int targetLevel = currentLevel + 1;
        costList = upgradeCosts.GetUpgradeCost(branchType, targetLevel);

        if (costList == null || costList.Count == 0)
        {
            canAfford = true;
            return true;
        }

        if (resourceSystem == null)
        {
            Debug.LogWarning("StructureUpgradeSystem: ResourceSystem reference is missing.");
            canAfford = false;
            return true;
        }

        canAfford = resourceSystem.CanAfford(costList);
        return true;
    }

    // Try to apply the selected branch upgrade
    public bool TryApplyUpgrade(UpgradeBranchType branchType)
    {
        if (!TryGetSelectedUpgradeData(out _, out StructureUpgradeProgress upgradeProgress, out StructureUpgradeCosts upgradeCosts))
        {
            return false;
        }

        if (resourceSystem == null)
        {
            Debug.LogWarning("StructureUpgradeSystem: ResourceSystem reference is missing.");
            return false;
        }

        int currentLevel = upgradeProgress.GetBranchLevel(branchType);
        int targetLevel = currentLevel + 1;

        List<ResourceAmount> costList = upgradeCosts.GetUpgradeCost(branchType, targetLevel);

        if (costList == null)
        {
            Debug.LogWarning("StructureUpgradeSystem: No upgrade cost found for " + branchType + " level " + targetLevel);
            return false;
        }

        if (!resourceSystem.CanAfford(costList))
        {
            Debug.Log("StructureUpgradeSystem: Not enough resources to upgrade " + branchType + " to level " + targetLevel);
            return false;
        }

        bool resourcesSpent = resourceSystem.SpendResources(costList);

        if (!resourcesSpent)
        {
            Debug.Log("StructureUpgradeSystem: Failed to spend upgrade resources.");
            return false;
        }

        bool upgraded = upgradeProgress.ApplyUpgrade(branchType);

        if (upgraded)
        {
            Debug.Log("StructureUpgradeSystem: Upgrade applied to " + branchType + " | New Level: " + targetLevel);
        }

        return upgraded;
    }
}