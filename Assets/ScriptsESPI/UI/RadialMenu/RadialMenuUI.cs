using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private StructureUpgradeSystem structureUpgradeSystem;

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
        GameEvents.StartListening(EventNames.StructureSelected, OnStructureChanged);
        GameEvents.StartListening(EventNames.StructureDeselected, OnStructureChanged);
    }

    private void OnDisable()
    {
        GameEvents.StopListening(EventNames.StateChanged, OnStateChanged);
        GameEvents.StopListening(EventNames.ResourcesChanged, OnResourcesChanged);
        GameEvents.StopListening(EventNames.StructureSelected, OnStructureChanged);
        GameEvents.StopListening(EventNames.StructureDeselected, OnStructureChanged);
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

    // Refresh radial option states when structure selection changes
    private void OnStructureChanged(object eventData)
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

    // Apply the selected branch upgrade through the upgrade system
    private void ApplySelectedUpgrade()
    {
        if (currentSelectedOption == null)
        {
            return;
        }

        if (structureUpgradeSystem == null)
        {
            Debug.LogWarning("RadialMenuUI: StructureUpgradeSystem reference is missing.");
            return;
        }

        structureUpgradeSystem.TryApplyUpgrade(currentSelectedOption.BranchType);
    }

    // Refresh all branch states
    private void RefreshBranchAvailability()
    {
        if (structureUpgradeSystem == null)
        {
            Debug.LogWarning("RadialMenuUI: StructureUpgradeSystem reference is missing.");
            return;
        }

        RefreshSingleBranch(optionA);
        RefreshSingleBranch(optionB);
        RefreshSingleBranch(optionC);
    }

    // Update a single branch visual state
    private void RefreshSingleBranch(RadialOptionButtonUI option)
    {
        if (option == null)
        {
            return;
        }

        bool hasData = structureUpgradeSystem.TryGetBranchPreviewData(
            option.BranchType,
            out int currentLevel,
            out bool isLocked,
            out bool isMaxed,
            out List<ResourceAmount> costList,
            out bool canAfford
        );

        if (!hasData)
        {
            option.SetLocked(true);
            option.SetMaxed(false);
            option.SetLevelText("N/A");
            option.SetCostText("");
            option.SetAffordable(false);
            return;
        }

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

        if (costList == null || costList.Count == 0)
        {
            option.SetCostText("Free");
            option.SetAffordable(true);
            return;
        }

        string builtCostText = BuildCostText(costList);
        option.SetCostText(builtCostText);
        option.SetAffordable(canAfford);

        if (!canAfford)
        {
            option.SetCostText(builtCostText + " - Not enough");
        }
    }

    // Build a readable cost string from a resource list
    private string BuildCostText(List<ResourceAmount> costList)
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