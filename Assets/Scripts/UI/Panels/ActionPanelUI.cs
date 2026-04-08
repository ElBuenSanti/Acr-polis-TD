//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;

//// Handles the right action panel UI for the selected structure
//public class ActionPanelUI : MonoBehaviour
//{
//    [Header("Content Root")]
//    [SerializeField] private GameObject contentRoot;

//    [Header("Button References")]
//    [SerializeField] private Button upgradeButton;
//    [SerializeField] private Button moveButton;
//    [SerializeField] private Button infoButton;

//    [Header("Button Images")]
//    [SerializeField] private Image upgradeButtonImage;
//    [SerializeField] private Image moveButtonImage;
//    [SerializeField] private Image infoButtonImage;

//    [Header("Text References")]
//    [SerializeField] private TMP_Text cooldownText;

//    [Header("System References")]
//    [SerializeField] private CooldownSystem cooldownSystem;
//    [SerializeField] private SelectionSystem selectionSystem;
//    [SerializeField] private BuildCardInfoUI buildCardInfoUI;
//    [SerializeField] private BuildCardSelectionMemory buildCardSelectionMemory;

//    [Header("Visual Colors")]
//    [SerializeField] private Color freeColor = Color.white;
//    [SerializeField] private Color cooldownColor = new Color(0.7f, 0.7f, 0.7f, 1f);
//    [SerializeField] private Color blockedColor = new Color(0.45f, 0.45f, 0.45f, 1f);

//    private void OnEnable()
//    {
//        GameEvents.StartListening(EventNames.StructureSelected, OnStructureChanged);
//        GameEvents.StartListening(EventNames.StructureDeselected, OnStructureChanged);
//        GameEvents.StartListening(EventNames.StateChanged, OnStateChanged);

//        BindButtons();
//    }

//    private void OnDisable()
//    {
//        GameEvents.StopListening(EventNames.StructureSelected, OnStructureChanged);
//        GameEvents.StopListening(EventNames.StructureDeselected, OnStructureChanged);
//        GameEvents.StopListening(EventNames.StateChanged, OnStateChanged);

//        UnbindButtons();
//    }

//    private void Start()
//    {
//        RefreshPanel();
//    }

//    private void Update()
//    {
//        RefreshPanel();
//    }

//    // Called when structure selection changes
//    private void OnStructureChanged(object eventData)
//    {
//        RefreshPanel();
//    }

//    // Called when game state changes
//    private void OnStateChanged(object eventData)
//    {
//        RefreshPanel();
//    }

//    // Refresh the whole panel state
//    private void RefreshPanel()
//    {
//        bool hasSelection = HasSelectedStructure();

//        SetContentVisible(hasSelection);

//        if (!hasSelection)
//        {
//            ResetPanel();
//            return;
//        }

//        if (GameStateManager.Instance == null || cooldownSystem == null)
//        {
//            ResetPanel();
//            return;
//        }

//        GameState currentState = GameStateManager.Instance.CurrentState;

//        ActionAvailabilityState moveState = GetMoveState(currentState);
//        ActionAvailabilityState upgradeState = GetUpgradeState(currentState);
//        ActionAvailabilityState infoState = GetInfoState(currentState);

//        ApplyButtonState(moveButton, moveButtonImage, moveState);
//        ApplyButtonState(upgradeButton, upgradeButtonImage, upgradeState);
//        ApplyButtonState(infoButton, infoButtonImage, infoState);

//        UpdateCooldownText(currentState, moveState, upgradeState);
//    }

//    // Check if there is a valid selected structure
//    private bool HasSelectedStructure()
//    {
//        return selectionSystem != null && selectionSystem.HasSelection();
//    }

//    // Get move action state based on the current game state
//    private ActionAvailabilityState GetMoveState(GameState currentState)
//    {
//        if (!HasSelectedStructure())
//        {
//            return ActionAvailabilityState.Blocked;
//        }

//        if (currentState == GameState.Combat)
//        {
//            return cooldownSystem.IsOnCooldown(CooldownType.MoveStructure)
//                ? ActionAvailabilityState.Cooldown
//                : ActionAvailabilityState.Free;
//        }

//        if (currentState == GameState.Preparation || currentState == GameState.StructureSelected)
//        {
//            return ActionAvailabilityState.Free;
//        }

//        return ActionAvailabilityState.Blocked;
//    }

//    // Get upgrade action state based on the current game state
//    private ActionAvailabilityState GetUpgradeState(GameState currentState)
//    {
//        if (!HasSelectedStructure())
//        {
//            return ActionAvailabilityState.Blocked;
//        }

//        if (currentState == GameState.Combat)
//        {
//            return cooldownSystem.IsOnCooldown(CooldownType.UpgradeStructure)
//                ? ActionAvailabilityState.Cooldown
//                : ActionAvailabilityState.Free;
//        }

//        if (currentState == GameState.Preparation || currentState == GameState.StructureSelected)
//        {
//            return ActionAvailabilityState.Free;
//        }

//        return ActionAvailabilityState.Blocked;
//    }

//    // Get info action state based on the current game state
//    private ActionAvailabilityState GetInfoState(GameState currentState)
//    {
//        if (buildCardSelectionMemory == null || !buildCardSelectionMemory.HasSelectedBuildCard)
//        {
//            return ActionAvailabilityState.Blocked;
//        }

//        if (currentState == GameState.Preparation || currentState == GameState.StructureSelected || currentState == GameState.Combat)
//        {
//            return ActionAvailabilityState.Free;
//        }

//        return ActionAvailabilityState.Blocked;
//    }

//    // Called by the Info button
//    public void OpenBuildCardInfo()
//    {
//        if (buildCardInfoUI == null || buildCardSelectionMemory == null)
//        {
//            Debug.Log("ActionPanelUI: Missing build card info references.");
//            return;
//        }

//        if (!buildCardSelectionMemory.HasSelectedBuildCard)
//        {
//            Debug.Log("ActionPanelUI: No build card selected for info.");
//            return;
//        }

//        buildCardInfoUI.OpenForBuildType(buildCardSelectionMemory.CurrentBuildType);
//    }

//    // Bind UI button events
//    private void BindButtons()
//    {
//        if (infoButton != null)
//        {
//            infoButton.onClick.RemoveListener(OpenBuildCardInfo);
//            infoButton.onClick.AddListener(OpenBuildCardInfo);
//        }
//    }

//    // Unbind UI button events
//    private void UnbindButtons()
//    {
//        if (infoButton != null)
//        {
//            infoButton.onClick.RemoveListener(OpenBuildCardInfo);
//        }
//    }

//    // Apply the visual/interactable state to a button
//    private void ApplyButtonState(Button button, Image buttonImage, ActionAvailabilityState state)
//    {
//        if (button == null)
//        {
//            return;
//        }

//        switch (state)
//        {
//            case ActionAvailabilityState.Free:
//                button.interactable = true;
//                SetButtonColor(buttonImage, freeColor);
//                break;

//            case ActionAvailabilityState.Cooldown:
//                button.interactable = false;
//                SetButtonColor(buttonImage, cooldownColor);
//                break;

//            case ActionAvailabilityState.Blocked:
//                button.interactable = false;
//                SetButtonColor(buttonImage, blockedColor);
//                break;
//        }
//    }

//    // Set button image color
//    private void SetButtonColor(Image buttonImage, Color color)
//    {
//        if (buttonImage != null)
//        {
//            buttonImage.color = color;
//        }
//    }

//    // Update the panel helper text
//    private void UpdateCooldownText(GameState currentState, ActionAvailabilityState moveState, ActionAvailabilityState upgradeState)
//    {
//        if (cooldownText == null)
//        {
//            return;
//        }

//        if (!HasSelectedStructure())
//        {
//            cooldownText.text = "";
//            return;
//        }

//        if (moveState == ActionAvailabilityState.Cooldown)
//        {
//            cooldownText.text = "Move cooldown: " +
//                                cooldownSystem.GetRemainingCooldown(CooldownType.MoveStructure).ToString("F1") + "s";
//            return;
//        }

//        if (upgradeState == ActionAvailabilityState.Cooldown)
//        {
//            cooldownText.text = "Upgrade cooldown: " +
//                                cooldownSystem.GetRemainingCooldown(CooldownType.UpgradeStructure).ToString("F1") + "s";
//            return;
//        }

//        if (currentState == GameState.Preparation || currentState == GameState.StructureSelected)
//        {
//            cooldownText.text = "Preparation: actions ready";
//            return;
//        }

//        if (currentState == GameState.Combat)
//        {
//            cooldownText.text = "Combat: actions available";
//            return;
//        }

//        if (currentState == GameState.BuildingPlacement)
//        {
//            cooldownText.text = "Building placement active";
//            return;
//        }

//        if (currentState == GameState.MoveStructure)
//        {
//            cooldownText.text = "Move mode active";
//            return;
//        }

//        if (currentState == GameState.RadialUpgradeOpen)
//        {
//            cooldownText.text = "Upgrade menu active";
//            return;
//        }

//        if (currentState == GameState.Paused)
//        {
//            cooldownText.text = "Paused";
//            return;
//        }

//        if (currentState == GameState.BlessingSelection)
//        {
//            cooldownText.text = "Blessing selection active";
//            return;
//        }

//        cooldownText.text = "Actions blocked";
//    }

//    // Reset the panel state
//    private void ResetPanel()
//    {
//        if (upgradeButton != null) upgradeButton.interactable = false;
//        if (moveButton != null) moveButton.interactable = false;
//        if (infoButton != null) infoButton.interactable = false;

//        SetButtonColor(upgradeButtonImage, blockedColor);
//        SetButtonColor(moveButtonImage, blockedColor);
//        SetButtonColor(infoButtonImage, blockedColor);

//        if (cooldownText != null)
//        {
//            cooldownText.text = "";
//        }
//    }

//    // Show or hide the panel content
//    private void SetContentVisible(bool isVisible)
//    {
//        if (contentRoot != null)
//        {
//            contentRoot.SetActive(isVisible);
//        }
//    }
//}


using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Handles the right action panel UI for the selected structure
public class ActionPanelUI : MonoBehaviour
{
    [Header("Content Root")]
    [SerializeField] private GameObject contentRoot;

    [Header("Button References")]
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Button moveButton;
    [SerializeField] private Button infoButton;

    [Header("Button Images")]
    [SerializeField] private Image upgradeButtonImage;
    [SerializeField] private Image moveButtonImage;
    [SerializeField] private Image infoButtonImage;

    [Header("Text References")]
    [SerializeField] private TMP_Text cooldownText;

    [Header("System References")]
    [SerializeField] private CooldownSystem cooldownSystem;
    [SerializeField] private SelectionSystem selectionSystem;
    [SerializeField] private BuildCardInfoUI buildCardInfoUI;
    [SerializeField] private BuildCardSelectionMemory buildCardSelectionMemory;

    [Header("Visual Colors")]
    [SerializeField] private Color freeColor = Color.white;
    [SerializeField] private Color cooldownColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    [SerializeField] private Color blockedColor = new Color(0.45f, 0.45f, 0.45f, 1f);

    private void OnEnable()
    {
        GameEvents.StartListening(EventNames.StructureSelected, OnStructureChanged);
        GameEvents.StartListening(EventNames.StructureDeselected, OnStructureChanged);
        GameEvents.StartListening(EventNames.StateChanged, OnStateChanged);
        GameEvents.StartListening(EventNames.ResourcesChanged, OnResourcesChanged);

        BindButtons();
        RefreshPanel();
    }

    private void OnDisable()
    {
        GameEvents.StopListening(EventNames.StructureSelected, OnStructureChanged);
        GameEvents.StopListening(EventNames.StructureDeselected, OnStructureChanged);
        GameEvents.StopListening(EventNames.StateChanged, OnStateChanged);
        GameEvents.StopListening(EventNames.ResourcesChanged, OnResourcesChanged);

        UnbindButtons();
    }

    private void Start()
    {
        RefreshPanel();
    }

    // Called when structure selection changes
    private void OnStructureChanged(object eventData)
    {
        RefreshPanel();
    }

    // Called when game state changes
    private void OnStateChanged(object eventData)
    {
        RefreshPanel();
    }

    // Called when resources change
    private void OnResourcesChanged(object eventData)
    {
        RefreshPanel();
    }

    // Refresh the whole panel state
    private void RefreshPanel()
    {
        bool hasSelection = HasSelectedStructure();

        SetContentVisible(hasSelection);

        if (!hasSelection)
        {
            ResetPanel();
            return;
        }

        if (GameStateManager.Instance == null || cooldownSystem == null)
        {
            ResetPanel();
            return;
        }

        GameState currentState = GameStateManager.Instance.CurrentState;

        ActionAvailabilityState moveState = GetMoveState(currentState);
        ActionAvailabilityState upgradeState = GetUpgradeState(currentState);
        ActionAvailabilityState infoState = GetInfoState(currentState);

        ApplyButtonState(moveButton, moveButtonImage, moveState);
        ApplyButtonState(upgradeButton, upgradeButtonImage, upgradeState);
        ApplyButtonState(infoButton, infoButtonImage, infoState);

        UpdateCooldownText(currentState, moveState, upgradeState);
    }

    // Check if there is a valid selected structure
    private bool HasSelectedStructure()
    {
        return selectionSystem != null && selectionSystem.HasSelection();
    }

    // Get move action state based on the current game state
    private ActionAvailabilityState GetMoveState(GameState currentState)
    {
        if (!HasSelectedStructure())
        {
            return ActionAvailabilityState.Blocked;
        }

        if (currentState == GameState.Combat)
        {
            return cooldownSystem.IsOnCooldown(CooldownType.MoveStructure)
                ? ActionAvailabilityState.Cooldown
                : ActionAvailabilityState.Free;
        }

        if (currentState == GameState.Preparation || currentState == GameState.StructureSelected)
        {
            return ActionAvailabilityState.Free;
        }

        return ActionAvailabilityState.Blocked;
    }

    // Get upgrade action state based on the current game state
    private ActionAvailabilityState GetUpgradeState(GameState currentState)
    {
        if (!HasSelectedStructure())
        {
            return ActionAvailabilityState.Blocked;
        }

        if (currentState == GameState.Combat)
        {
            return cooldownSystem.IsOnCooldown(CooldownType.UpgradeStructure)
                ? ActionAvailabilityState.Cooldown
                : ActionAvailabilityState.Free;
        }

        if (currentState == GameState.Preparation || currentState == GameState.StructureSelected)
        {
            return ActionAvailabilityState.Free;
        }

        return ActionAvailabilityState.Blocked;
    }

    // Get info action state based on the current game state
    private ActionAvailabilityState GetInfoState(GameState currentState)
    {
        if (buildCardSelectionMemory == null || !buildCardSelectionMemory.HasSelectedBuildCard)
        {
            return ActionAvailabilityState.Blocked;
        }

        if (currentState == GameState.Preparation ||
            currentState == GameState.StructureSelected ||
            currentState == GameState.Combat)
        {
            return ActionAvailabilityState.Free;
        }

        return ActionAvailabilityState.Blocked;
    }

    // Called by the Info button
    public void OpenBuildCardInfo()
    {
        if (buildCardInfoUI == null || buildCardSelectionMemory == null)
        {
            Debug.Log("ActionPanelUI: Missing build card info references.");
            return;
        }

        if (!buildCardSelectionMemory.HasSelectedBuildCard)
        {
            Debug.Log("ActionPanelUI: No build card selected for info.");
            return;
        }

        buildCardInfoUI.OpenForBuildType(buildCardSelectionMemory.CurrentBuildType);
    }

    // Bind UI button events
    private void BindButtons()
    {
        if (infoButton != null)
        {
            infoButton.onClick.RemoveListener(OpenBuildCardInfo);
            infoButton.onClick.AddListener(OpenBuildCardInfo);
        }
    }

    // Unbind UI button events
    private void UnbindButtons()
    {
        if (infoButton != null)
        {
            infoButton.onClick.RemoveListener(OpenBuildCardInfo);
        }
    }

    // Apply the visual/interactable state to a button
    private void ApplyButtonState(Button button, Image buttonImage, ActionAvailabilityState state)
    {
        if (button == null)
        {
            return;
        }

        switch (state)
        {
            case ActionAvailabilityState.Free:
                button.interactable = true;
                SetButtonColor(buttonImage, freeColor);
                break;

            case ActionAvailabilityState.Cooldown:
                button.interactable = false;
                SetButtonColor(buttonImage, cooldownColor);
                break;

            case ActionAvailabilityState.Blocked:
                button.interactable = false;
                SetButtonColor(buttonImage, blockedColor);
                break;
        }
    }

    // Set button image color
    private void SetButtonColor(Image buttonImage, Color color)
    {
        if (buttonImage != null)
        {
            buttonImage.color = color;
        }
    }

    // Update the panel helper text
    private void UpdateCooldownText(GameState currentState, ActionAvailabilityState moveState, ActionAvailabilityState upgradeState)
    {
        if (cooldownText == null)
        {
            return;
        }

        if (!HasSelectedStructure())
        {
            cooldownText.text = "";
            return;
        }

        if (moveState == ActionAvailabilityState.Cooldown)
        {
            cooldownText.text = "Move cooldown: " +
                                cooldownSystem.GetRemainingCooldown(CooldownType.MoveStructure).ToString("F1") + "s";
            return;
        }

        if (upgradeState == ActionAvailabilityState.Cooldown)
        {
            cooldownText.text = "Upgrade cooldown: " +
                                cooldownSystem.GetRemainingCooldown(CooldownType.UpgradeStructure).ToString("F1") + "s";
            return;
        }

        if (currentState == GameState.Preparation || currentState == GameState.StructureSelected)
        {
            cooldownText.text = "Preparation: actions ready";
            return;
        }

        if (currentState == GameState.Combat)
        {
            cooldownText.text = "Combat: actions available";
            return;
        }

        if (currentState == GameState.BuildingPlacement)
        {
            cooldownText.text = "Building placement active";
            return;
        }

        if (currentState == GameState.MoveStructure)
        {
            cooldownText.text = "Move mode active";
            return;
        }

        if (currentState == GameState.RadialUpgradeOpen)
        {
            cooldownText.text = "Upgrade menu active";
            return;
        }

        if (currentState == GameState.Paused)
        {
            cooldownText.text = "Paused";
            return;
        }

        if (currentState == GameState.BlessingSelection)
        {
            cooldownText.text = "Blessing selection active";
            return;
        }

        cooldownText.text = "Actions blocked";
    }

    // Reset the panel state
    private void ResetPanel()
    {
        if (upgradeButton != null) upgradeButton.interactable = false;
        if (moveButton != null) moveButton.interactable = false;
        if (infoButton != null) infoButton.interactable = false;

        SetButtonColor(upgradeButtonImage, blockedColor);
        SetButtonColor(moveButtonImage, blockedColor);
        SetButtonColor(infoButtonImage, blockedColor);

        if (cooldownText != null)
        {
            cooldownText.text = "";
        }
    }

    // Show or hide the panel content
    private void SetContentVisible(bool isVisible)
    {
        if (contentRoot != null)
        {
            contentRoot.SetActive(isVisible);
        }
    }
}