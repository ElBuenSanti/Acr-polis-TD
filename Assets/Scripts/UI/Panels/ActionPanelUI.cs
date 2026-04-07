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

    [Header("Visual Colors")]
    [SerializeField] private Color freeColor = Color.white;
    [SerializeField] private Color cooldownColor = new Color(0.7f, 0.7f, 0.7f, 1f);
    [SerializeField] private Color blockedColor = new Color(0.45f, 0.45f, 0.45f, 1f);

    private void OnEnable()
    {
        GameEvents.StartListening(EventNames.StructureSelected, OnStructureSelected);
        GameEvents.StartListening(EventNames.StructureDeselected, OnStructureDeselected);
        GameEvents.StartListening(EventNames.StateChanged, OnStateChanged);
    }

    private void OnDisable()
    {
        GameEvents.StopListening(EventNames.StructureSelected, OnStructureSelected);
        GameEvents.StopListening(EventNames.StructureDeselected, OnStructureDeselected);
        GameEvents.StopListening(EventNames.StateChanged, OnStateChanged);
    }

    private void Update()
    {
        RefreshActionStates();
    }

    private void Start()
    {
        ResetPanel();
        SetContentVisible(false);
    }

    // Called when a structure is selected
    private void OnStructureSelected(object eventData)
    {
        SetContentVisible(true);
        RefreshActionStates();
    }

    // Called when a structure is deselected
    private void OnStructureDeselected(object eventData)
    {
        ResetPanel();
        SetContentVisible(false);
    }

    // Called when the game state changes
    private void OnStateChanged(object eventData)
    {
        RefreshActionStates();
    }

    // Refresh button states based on the current game state
    private void RefreshActionStates()
    {
        if (contentRoot == null || !contentRoot.activeSelf)
        {
            return;
        }

        if (GameStateManager.Instance == null || cooldownSystem == null)
        {
            return;
        }

        GameState currentState = GameStateManager.Instance.CurrentState;

        ActionAvailabilityState moveState = GetMoveState(currentState);
        ActionAvailabilityState upgradeState = GetUpgradeState(currentState);

        ApplyButtonState(moveButton, moveButtonImage, moveState);
        ApplyButtonState(upgradeButton, upgradeButtonImage, upgradeState);
        ApplyButtonState(infoButton, infoButtonImage, ActionAvailabilityState.Free);

        UpdateCooldownText(moveState, upgradeState);
    }

    // Get move action state based on the current game state
    private ActionAvailabilityState GetMoveState(GameState currentState)
    {
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

    // Update the cooldown text
    private void UpdateCooldownText(ActionAvailabilityState moveState, ActionAvailabilityState upgradeState)
    {
        if (cooldownText == null || cooldownSystem == null)
        {
            return;
        }

        if (moveState == ActionAvailabilityState.Cooldown)
        {
            cooldownText.text = "Move cooldown: " + cooldownSystem.GetRemainingCooldown(CooldownType.MoveStructure).ToString("F1") + "s";
            return;
        }

        if (upgradeState == ActionAvailabilityState.Cooldown)
        {
            cooldownText.text = "Upgrade cooldown: " + cooldownSystem.GetRemainingCooldown(CooldownType.UpgradeStructure).ToString("F1") + "s";
            return;
        }

        if (GameStateManager.Instance != null && GameStateManager.Instance.CurrentState == GameState.Combat)
        {
            cooldownText.text = "Combat: actions available";
        }
        else
        {
            cooldownText.text = "Preparation: no cooldown";
        }
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