using UnityEngine;

// Stores upgrade progress for a single structure using the custom 2-2-3 rule
public class StructureUpgradeProgress : MonoBehaviour
{
    public int BranchALevel { get; private set; }
    public int BranchBLevel { get; private set; }
    public int BranchCLevel { get; private set; }

    // Check if a branch can be upgraded under the 2-2-3 rule
    public bool CanUpgradeBranch(UpgradeBranchType branchType)
    {
        int levelA = BranchALevel;
        int levelB = BranchBLevel;
        int levelC = BranchCLevel;

        int selectedLevel = GetBranchLevel(branchType);

        if (selectedLevel >= 3)
        {
            return false;
        }

        int branchesAtTwoOrMore = CountBranchesAtTwoOrMore();

        // Before two branches reach level 2, any branch can be upgraded up to level 2
        if (branchesAtTwoOrMore < 2)
        {
            return selectedLevel < 2;
        }

        // Once two branches are already at level 2 or more, the third branch is locked
        if (IsLockedThirdBranch(branchType))
        {
            return false;
        }

        // Of the two surviving branches, only one can go from 2 to 3
        if (selectedLevel < 2)
        {
            return false;
        }

        if (selectedLevel == 2)
        {
            return !HasAnyBranchReachedLevelThree();
        }

        return false;
    }

    // Apply one upgrade level to the selected branch
    public bool ApplyUpgrade(UpgradeBranchType branchType)
    {
        if (!CanUpgradeBranch(branchType))
        {
            Debug.Log("Upgrade blocked for branch: " + branchType);
            return false;
        }

        switch (branchType)
        {
            case UpgradeBranchType.BranchA:
                BranchALevel++;
                Debug.Log("Branch A upgraded to level " + BranchALevel);
                return true;

            case UpgradeBranchType.BranchB:
                BranchBLevel++;
                Debug.Log("Branch B upgraded to level " + BranchBLevel);
                return true;

            case UpgradeBranchType.BranchC:
                BranchCLevel++;
                Debug.Log("Branch C upgraded to level " + BranchCLevel);
                return true;
        }

        return false;
    }

    // Return the level for a given branch
    public int GetBranchLevel(UpgradeBranchType branchType)
    {
        switch (branchType)
        {
            case UpgradeBranchType.BranchA:
                return BranchALevel;

            case UpgradeBranchType.BranchB:
                return BranchBLevel;

            case UpgradeBranchType.BranchC:
                return BranchCLevel;
        }

        return 0;
    }

    // Return true if the branch is locked by the current progression rules
    public bool IsBranchLocked(UpgradeBranchType branchType)
    {
        return !CanUpgradeBranch(branchType) && GetBranchLevel(branchType) == 0;
    }

    // Count how many branches are already at level 2 or more
    private int CountBranchesAtTwoOrMore()
    {
        int count = 0;

        if (BranchALevel >= 2) count++;
        if (BranchBLevel >= 2) count++;
        if (BranchCLevel >= 2) count++;

        return count;
    }

    // Return true if any branch has already reached level 3
    private bool HasAnyBranchReachedLevelThree()
    {
        return BranchALevel >= 3 || BranchBLevel >= 3 || BranchCLevel >= 3;
    }

    // Check if this branch is the third branch that should now be locked
    private bool IsLockedThirdBranch(UpgradeBranchType branchType)
    {
        switch (branchType)
        {
            case UpgradeBranchType.BranchA:
                return BranchALevel == 0 && BranchBLevel >= 2 && BranchCLevel >= 2;

            case UpgradeBranchType.BranchB:
                return BranchBLevel == 0 && BranchALevel >= 2 && BranchCLevel >= 2;

            case UpgradeBranchType.BranchC:
                return BranchCLevel == 0 && BranchALevel >= 2 && BranchBLevel >= 2;
        }

        return false;
    }
}