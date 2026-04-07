using UnityEngine;

// Controls basic HUD visibility and state-based UI behavior
public class HUDStateController : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private GameObject buildCardsPanel;
    [SerializeField] private GameObject radialMenuPanel;
    [SerializeField] private GameObject blessingSelectionPanel;

    private void OnEnable()
    {
        GameEvents.StartListening(EventNames.StateChanged, OnStateChanged);
    }

    private void OnDisable()
    {
        GameEvents.StopListening(EventNames.StateChanged, OnStateChanged);
    }

    private void Start()
    {
        ApplyState(GameStateManager.Instance.CurrentState);
    }

    // Called when the game state changes
    private void OnStateChanged(object eventData)
    {
        if (eventData is GameState newState)
        {
            ApplyState(newState);
        }
    }

    // Apply UI behavior based on the current game state
    private void ApplyState(GameState currentState)
    {
        HideAllStatePanels();

        switch (currentState)
        {
            case GameState.Preparation:
                SetBuildCardsVisible(true);
                break;

            case GameState.BuildingPlacement:
                SetBuildCardsVisible(true);
                break;

            case GameState.StructureSelected:
                SetBuildCardsVisible(true);
                break;

            case GameState.RadialUpgradeOpen:
                SetBuildCardsVisible(true);
                SetRadialMenuVisible(true);
                break;

            case GameState.Combat:
                SetBuildCardsVisible(true);
                break;

            case GameState.Paused:
                SetBuildCardsVisible(false);
                break;

            case GameState.BlessingSelection:
                SetBlessingSelectionVisible(true);
                break;

            case GameState.Victory:
                break;

            case GameState.Defeat:
                break;
        }
    }

    // Hide panels that depend on specific game states
    private void HideAllStatePanels()
    {
        SetRadialMenuVisible(false);
        SetBlessingSelectionVisible(false);
    }

    // Show or hide the build cards panel
    private void SetBuildCardsVisible(bool isVisible)
    {
        if (buildCardsPanel != null)
        {
            buildCardsPanel.SetActive(isVisible);
        }
    }

    // Show or hide the radial menu panel
    private void SetRadialMenuVisible(bool isVisible)
    {
        if (radialMenuPanel != null)
        {
            radialMenuPanel.SetActive(isVisible);
        }
    }

    // Show or hide the blessing selection panel
    private void SetBlessingSelectionVisible(bool isVisible)
    {
        if (blessingSelectionPanel != null)
        {
            blessingSelectionPanel.SetActive(isVisible);
        }
    }
}