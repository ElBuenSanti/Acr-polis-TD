using System.Collections.Generic;
using UnityEngine;

// Stores upgrade costs for a structure
public class StructureUpgradeCosts : MonoBehaviour
{
    [Header("Upgrade Costs")]
    [SerializeField] private List<UpgradeCostData> upgradeCosts = new List<UpgradeCostData>();

    // Return the resource cost list for a branch and target level
    public List<ResourceAmount> GetUpgradeCost(UpgradeBranchType branchType, int targetLevel)
    {
        foreach (UpgradeCostData costData in upgradeCosts)
        {
            if (costData.branchType == branchType && costData.targetLevel == targetLevel)
            {
                return costData.costs;
            }
        }

        return null;
    }
}