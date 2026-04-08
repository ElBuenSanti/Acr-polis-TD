using System;
using System.Collections.Generic;

[Serializable]
public class UpgradeCostData
{
    public UpgradeBranchType branchType = UpgradeBranchType.BranchA;
    public int targetLevel = 1;
    public List<ResourceAmount> costs = new List<ResourceAmount>();
}