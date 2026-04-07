using System.Collections;
using TMPro;
using UnityEngine;

// Handles the center radial upgrade menu UI
public class RadialMenuUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject contentRoot;
    [SerializeField] private RadialOptionButtonUI optionA;
    [SerializeField] private RadialOptionButtonUI optionB;
    [SerializeField] private RadialOptionButtonUI optionC;
    [SerializeField] private TMP_Text selectedOptionText;
    [SerializeField] private UpgradeModeSystem upgradeModeSystem;
    [SerializeField] private SelectionSystem selectionSystem;

    [Header("Selection Feedback")]
    [SerializeField] private float selectedOptionVisibleTime = 3f;

    private RadialOptionButtonUI currentSelectedOption;
    private Coroutine selectionRoutine;

    private void Awake()
    {
        SetContentVisible(false);
        SetSelectedOptionVisible(false);
    }

    private void OnEnable()
    {
        GameEvents.StartListening(EventNames.StateChanged, OnStateChanged);
        GameEvents.StartListening(EventNames.ResourcesChanged, OnResourcesChanged);
    }

    private void OnDisable()
    {
        GameEvents.StopListening(EventNames.StateChanged, OnStateChanged);
        GameEvents.StopListening(EventNames.ResourcesChanged, OnResourcesChanged);
    }

    private void Start()
    {
        SetupOptions();
        ApplyCurrentState();
        ClearSelection();
    }


    // Refresh radial option states when resources change
    private void OnResourcesChanged(object eventData)
    {
        if (GameStateManager.Instance == null)
        {
            return;
        }

        if (GameStateManager.Instance.CurrentState == GameState.RadialUpgradeOpen)
        {
            RefreshBranchAvailability();
        }
    }

    // Assign this radial menu to all option buttons
    private void SetupOptions()
    {
        if (optionA != null) optionA.SetRadialMenu(this);
        if (optionB != null) optionB.SetRadialMenu(this);
        if (optionC != null) optionC.SetRadialMenu(this);
    }

    // Sync the radial menu visibility with the current game state
    private void ApplyCurrentState()
    {
        if (GameStateManager.Instance == null)
        {
            SetContentVisible(false);
            SetSelectedOptionVisible(false);
            return;
        }

        bool shouldShow = GameStateManager.Instance.CurrentState == GameState.RadialUpgradeOpen;
        SetContentVisible(shouldShow);

        if (shouldShow)
        {
            RefreshBranchAvailability();
        }
        else
        {
            ClearSelection();
        }
    }

    // Called when the game state changes
    private void OnStateChanged(object eventData)
    {
        if (!(eventData is GameState newState))
        {
            return;
        }

        bool shouldShow = newState == GameState.RadialUpgradeOpen;
        SetContentVisible(shouldShow);

        if (shouldShow)
        {
            RefreshBranchAvailability();
        }
        else
        {
            ClearSelection();
        }
    }

    // Select one of the radial options
    public void SelectOption(RadialOptionButtonUI selectedOption)
    {
        if (selectedOption == null)
        {
            return;
        }

        currentSelectedOption = selectedOption;
        RefreshOptionVisuals();

        if (selectedOptionText != null)
        {
            selectedOptionText.text = "Selected: " + selectedOption.OptionTitle;
        }

        SetSelectedOptionVisible(true);

        Debug.Log("Radial option selected: " + selectedOption.OptionTitle);

        // Hide the radial content after selecting an option
        SetContentVisible(false);

        if (selectionRoutine != null)
        {
            StopCoroutine(selectionRoutine);
        }

        selectionRoutine = StartCoroutine(HandleSelectedOptionFlow());
    }

    // Wait a few seconds, apply the upgrade, then close upgrade mode
    private IEnumerator HandleSelectedOptionFlow()
    {
        yield return new WaitForSeconds(selectedOptionVisibleTime);

        ApplySelectedUpgrade();

        if (upgradeModeSystem != null)
        {
            upgradeModeSystem.ConfirmUpgrade();
        }

        ClearSelection();
    }

    // Apply the selected branch upgrade to the selected structure
    // Apply the selected branch upgrade to the selected structure
    private void ApplySelectedUpgrade()
    {
        if (currentSelectedOption == null || selectionSystem == null || !selectionSystem.HasSelection())
        {
            return;
        }

        if (GameStateManager.Instance == null)
        {
            return;
        }

        MonoBehaviour selectedBehaviour = selectionSystem.CurrentSelected as MonoBehaviour;

        if (selectedBehaviour == null)
        {
            return;
        }

        StructureUpgradeProgress upgradeProgress = selectedBehaviour.GetComponent<StructureUpgradeProgress>();
        StructureUpgradeCosts upgradeCosts = selectedBehaviour.GetComponent<StructureUpgradeCosts>();
        ResourceSystem resourceSystem = FindAnyObjectByType<ResourceSystem>();

        if (upgradeProgress == null)
        {
            Debug.LogWarning("Selected structure is missing StructureUpgradeProgress.");
            return;
        }

        if (upgradeCosts == null)
        {
            Debug.LogWarning("Selected structure is missing StructureUpgradeCosts.");
            return;
        }

        if (resourceSystem == null)
        {
            Debug.LogWarning("ResourceSystem was not found in the scene.");
            return;
        }

        UpgradeBranchType branchType = currentSelectedOption.BranchType;
        int currentLevel = upgradeProgress.GetBranchLevel(branchType);
        int targetLevel = currentLevel + 1;

        var costList = upgradeCosts.GetUpgradeCost(branchType, targetLevel);

        if (costList == null)
        {
            Debug.LogWarning("No upgrade cost found for " + branchType + " level " + targetLevel);
            return;
        }

        if (!resourceSystem.CanAfford(costList))
        {
            Debug.Log("Not enough resources to upgrade " + branchType + " to level " + targetLevel);
            return;
        }

        bool resourcesSpent = resourceSystem.SpendResources(costList);

        if (!resourcesSpent)
        {
            Debug.Log("Upgrade failed because resources could not be spent.");
            return;
        }

        bool upgraded = upgradeProgress.ApplyUpgrade(branchType);

        if (upgraded)
        {
            Debug.Log("Upgrade applied to branch: " + branchType + " | New Level: " + targetLevel);
        }
    }
    private void RefreshBranchAvailability()
    {
        if (selectionSystem == null || !selectionSystem.HasSelection())
        {
            return;
        }

        MonoBehaviour selectedBehaviour = selectionSystem.CurrentSelected as MonoBehaviour;

        if (selectedBehaviour == null)
        {
            return;
        }

        StructureUpgradeProgress upgradeProgress = selectedBehaviour.GetComponent<StructureUpgradeProgress>();

        if (upgradeProgress == null)
        {
            return;
        }

        RefreshSingleBranch(optionA, upgradeProgress);
        RefreshSingleBranch(optionB, upgradeProgress);
        RefreshSingleBranch(optionC, upgradeProgress);
    }

    // Update a single branch visual state
    private void RefreshSingleBranch(RadialOptionButtonUI option, StructureUpgradeProgress upgradeProgress)
    {
        if (option == null || upgradeProgress == null)
        {
            return;
        }

        MonoBehaviour selectedBehaviour = selectionSystem.CurrentSelected as MonoBehaviour;

        if (selectedBehaviour == null)
        {
            return;
        }

        StructureUpgradeCosts upgradeCosts = selectedBehaviour.GetComponent<StructureUpgradeCosts>();
        ResourceSystem resourceSystem = FindAnyObjectByType<ResourceSystem>();

        int currentLevel = upgradeProgress.GetBranchLevel(option.BranchType);
        bool isLocked = upgradeProgress.IsBranchLocked(option.BranchType);
        bool isMaxed = currentLevel >= 3;

        option.SetLocked(isLocked);
        option.SetMaxed(isMaxed);

        if (isLocked)
        {
            option.SetLevelText("Locked");
            option.SetCostText("");
            option.SetAffordable(false);
            return;
        }

        if (isMaxed)
        {
            option.SetLevelText("Lv.3");
            option.SetCostText("Max");
            option.SetAffordable(false);
            return;
        }

        option.SetLevelText("Lv." + currentLevel);

        if (upgradeCosts == null)
        {
            option.SetCostText("No Cost");
            option.SetAffordable(true);
            return;
        }

        int targetLevel = currentLevel + 1;
        var costList = upgradeCosts.GetUpgradeCost(option.BranchType, targetLevel);

        if (costList == null || costList.Count == 0)
        {
            option.SetCostText("Free");
            option.SetAffordable(true);
            return;
        }

        option.SetCostText(BuildCostText(costList));

        if (resourceSystem == null)
        {
            option.SetAffordable(true);
            return;
        }

        bool canAfford = resourceSystem.CanAfford(costList);
        option.SetAffordable(canAfford);

        if (!canAfford)
        {
            option.SetCostText(BuildCostText(costList) + " - Not enough");
        }
    }

    // Build a readable cost string from a resource list
    private string BuildCostText(System.Collections.Generic.List<ResourceAmount> costList)
    {
        if (costList == null || costList.Count == 0)
        {
            return "Free";
        }

        string result = "Cost: ";

        for (int i = 0; i < costList.Count; i++)
        {
            ResourceAmount cost = costList[i];
            result += cost.amount + " " + cost.resourceType;

            if (i < costList.Count - 1)
            {
                result += " | ";
            }
        }

        return result;
    }


    // Update all option visual states
    private void RefreshOptionVisuals()
    {
        if (optionA != null) optionA.SetSelected(optionA == currentSelectedOption);
        if (optionB != null) optionB.SetSelected(optionB == currentSelectedOption);
        if (optionC != null) optionC.SetSelected(optionC == currentSelectedOption);
    }

    // Clear the current option selection
    private void ClearSelection()
    {
        currentSelectedOption = null;
        RefreshOptionVisuals();

        if (selectedOptionText != null)
        {
            selectedOptionText.text = "";
        }

        SetSelectedOptionVisible(false);
    }

    // Show or hide the radial menu content
    private void SetContentVisible(bool isVisible)
    {
        if (contentRoot == null)
        {
            Debug.LogWarning("RadialMenuUI: Content Root is missing.");
            return;
        }

        contentRoot.SetActive(isVisible);
    }

    // Show or hide the selected option text
    private void SetSelectedOptionVisible(bool isVisible)
    {
        if (selectedOptionText != null)
        {
            selectedOptionText.gameObject.SetActive(isVisible);
        }
    }

    public void CancelSelection()
    {
        if (selectionRoutine != null)
        {
            StopCoroutine(selectionRoutine);
            selectionRoutine = null;
        }

        ClearSelection();
        SetContentVisible(true);
    }
}