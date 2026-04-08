using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// Debug keyboard navigation for build cards and related actions
public class BuildCardKeyboardNavigationDebug : MonoBehaviour
{
    [Header("Build Card Buttons")]
    [SerializeField] private Button templeButton;
    [SerializeField] private Button barracksButton;
    [SerializeField] private Button defenseButton;

    [Header("System References")]
    [SerializeField] private BuildCardSelectionMemory buildCardSelectionMemory;
    [SerializeField] private BuildCardInfoUI buildCardInfoUI;
    [SerializeField] private BuildPlacementSystem buildPlacementSystem;
    [SerializeField] private MoveModeSystem moveModeSystem;
    [SerializeField] private UpgradeModeSystem upgradeModeSystem;

    private Button currentSelectedButton;
    private BuildType currentSelectedBuildType;
    private bool hasSelectedBuildCard;

    private void Start()
    {
        SelectBuildCard(templeButton, BuildType.Temple);
    }

    private void Update()
    {
        HandleBuildCardSelectionInput();
        HandleActionInput();
    }

    private void HandleBuildCardSelectionInput()
    {
        if (IsGameplayActionActive())
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            SelectBuildCard(templeButton, BuildType.Temple);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            SelectBuildCard(barracksButton, BuildType.Barracks);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            SelectBuildCard(defenseButton, BuildType.Defense);
        }
    }

    private void HandleActionInput()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            OpenInfoForCurrentCard();
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (buildCardInfoUI != null && buildCardInfoUI.IsOpen)
            {
                buildCardInfoUI.ClosePopup();
                return;
            }

            if (IsGameplayActionActive())
            {
                return;
            }

            TriggerCurrentCardButton();
        }

        // B closes info popup
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (buildCardInfoUI != null && buildCardInfoUI.IsOpen)
            {
                buildCardInfoUI.ClosePopup();
                return;
            }
        }

        // Backspace remains only for placement/move cancel in their own scripts
    }

    private bool IsGameplayActionActive()
    {
        bool isPlacing = buildPlacementSystem != null && buildPlacementSystem.IsPlacing;
        bool isMoving = moveModeSystem != null && moveModeSystem.IsMoving;
        bool isUpgrading = upgradeModeSystem != null && upgradeModeSystem.IsUpgradeOpen;

        return isPlacing || isMoving || isUpgrading;
    }

    private void SelectBuildCard(Button targetButton, BuildType buildType)
    {
        if (targetButton == null)
        {
            Debug.LogWarning("BuildCardKeyboardNavigationDebug: Target button is missing for " + buildType);
            return;
        }

        currentSelectedButton = targetButton;
        currentSelectedBuildType = buildType;
        hasSelectedBuildCard = true;

        if (buildCardSelectionMemory != null)
        {
            buildCardSelectionMemory.SetSelectedBuildType(buildType);
        }

        ForceButtonSelection(targetButton);

        Debug.Log("Keyboard selected build card: " + buildType);
    }

    private void TriggerCurrentCardButton()
    {
        if (!hasSelectedBuildCard || currentSelectedButton == null)
        {
            Debug.Log("BuildCardKeyboardNavigationDebug: No build card selected.");
            return;
        }

        if (!currentSelectedButton.interactable)
        {
            Debug.Log("BuildCardKeyboardNavigationDebug: Selected build card button is not interactable.");
            return;
        }

        ForceButtonSelection(currentSelectedButton);
        currentSelectedButton.onClick.Invoke();

        Debug.Log("Keyboard triggered build card button: " + currentSelectedBuildType);
    }

    private void OpenInfoForCurrentCard()
    {
        if (!hasSelectedBuildCard)
        {
            Debug.Log("BuildCardKeyboardNavigationDebug: No build card selected for info.");
            return;
        }

        if (buildCardInfoUI == null)
        {
            Debug.LogWarning("BuildCardKeyboardNavigationDebug: BuildCardInfoUI reference is missing.");
            return;
        }

        if (IsGameplayActionActive())
        {
            Debug.Log("BuildCardKeyboardNavigationDebug: Cannot open build card info during placement, move, or upgrade.");
            return;
        }

        buildCardInfoUI.OpenForBuildType(currentSelectedBuildType);
    }

    private void ForceButtonSelection(Button targetButton)
    {
        if (targetButton == null || EventSystem.current == null)
        {
            return;
        }

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(targetButton.gameObject);
        targetButton.Select();
    }
}