using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Handles the basic building placement flow
public class BuildPlacementSystem : MonoBehaviour
{
    public BuildType CurrentBuildType { get; private set; }
    public bool IsPlacing { get; private set; }
    public Vector3 CurrentPreviewPosition { get; private set; }

    [Header("Preview Reference")]
    [SerializeField] private BuildPreview buildPreview;

    [Header("Preview Settings")]
    [SerializeField] private Vector3 defaultPreviewPosition = new Vector3(0f, 0f, 0f);

    [Header("Build Costs")]
    [SerializeField] private List<BuildCostData> buildCosts = new List<BuildCostData>();

    [Header("System References")]
    [SerializeField] private ResourceSystem resourceSystem;
    [SerializeField] private HUDNavigationUI buildCardsNavigationUI;

    [Header("Input Protection")]
    [SerializeField] private float inputBlockDuration = 0.15f;

    private GameState returnStateAfterPlacement = GameState.Preparation;
    private float inputUnlockTime;

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
        ResetPlacement();
    }

    // Start placing a selected build type
    public void StartPlacement(BuildType buildType)
    {
        if (GameStateManager.Instance == null)
        {
            Debug.LogWarning("BuildPlacementSystem: GameStateManager instance is missing.");
            return;
        }

        if (resourceSystem == null)
        {
            Debug.LogWarning("BuildPlacementSystem: ResourceSystem reference is missing.");
            return;
        }

        if (buildPreview == null)
        {
            Debug.LogWarning("BuildPlacementSystem: BuildPreview reference is missing.");
            return;
        }

        if (!CanStartPlacement())
        {
            Debug.Log("Build placement cannot start in the current state.");
            return;
        }

        List<ResourceAmount> costList = GetBuildCost(buildType);

        if (costList != null && costList.Count > 0)
        {
            if (!resourceSystem.CanAfford(costList))
            {
                Debug.Log("Not enough resources to start placement for: " + buildType);
                return;
            }
        }

        GameState currentState = GameStateManager.Instance.CurrentState;

        if (currentState == GameState.Combat)
        {
            returnStateAfterPlacement = GameState.Combat;
        }
        else
        {
            returnStateAfterPlacement = GameState.Preparation;
        }

        CurrentBuildType = buildType;
        IsPlacing = true;
        inputUnlockTime = Time.unscaledTime + inputBlockDuration;
        CurrentPreviewPosition = defaultPreviewPosition;

        buildPreview.gameObject.SetActive(true);
        buildPreview.SetBuildType(buildType);
        buildPreview.SetPosition(CurrentPreviewPosition);

        // Clear current UI focus so arrow keys move the preview instead of the menu
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        Debug.Log("Started placement for: " + buildType + " at " + CurrentPreviewPosition);
        GameStateManager.Instance.ChangeState(GameState.BuildingPlacement);
    }

    // Confirm the current placement
    public void ConfirmPlacement()
    {
        if (!IsPlacing)
        {
            return;
        }

        if (!CanReceivePlacementInput())
        {
            return;
        }

        if (resourceSystem == null)
        {
            Debug.LogWarning("BuildPlacementSystem: ResourceSystem reference is missing.");
            return;
        }

        List<ResourceAmount> costList = GetBuildCost(CurrentBuildType);

        if (costList != null && costList.Count > 0)
        {
            if (!resourceSystem.CanAfford(costList))
            {
                Debug.Log("Not enough resources to confirm placement for: " + CurrentBuildType);
                CancelPlacement();
                return;
            }

            bool resourcesSpent = resourceSystem.SpendResources(costList);

            if (!resourcesSpent)
            {
                Debug.Log("Placement failed because resources could not be spent.");
                CancelPlacement();
                return;
            }
        }

        Debug.Log("Placement confirmed for: " + CurrentBuildType + " at " + CurrentPreviewPosition);

        GameState nextState = returnStateAfterPlacement;
        ResetPlacement();

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(nextState);
        }

        RestoreBuildCardsFocus();
    }

    // Cancel the current placement
    public void CancelPlacement()
    {
        if (!IsPlacing)
        {
            return;
        }

        if (!CanReceivePlacementInput())
        {
            return;
        }

        Debug.Log("Placement cancelled for: " + CurrentBuildType);

        GameState nextState = returnStateAfterPlacement;
        ResetPlacement();

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(nextState);
        }

        RestoreBuildCardsFocus();
    }

    // Move the preview object to an absolute world position
    public void MovePreview(Vector3 newPosition)
    {
        if (!IsPlacing || buildPreview == null)
        {
            return;
        }

        if (!CanReceivePlacementInput())
        {
            return;
        }

        CurrentPreviewPosition = newPosition;
        buildPreview.SetPosition(CurrentPreviewPosition);
    }

    // Move the preview by an offset from the current position
    public void MovePreviewBy(Vector3 offset)
    {
        if (!IsPlacing || buildPreview == null)
        {
            return;
        }

        if (!CanReceivePlacementInput())
        {
            return;
        }

        CurrentPreviewPosition += offset;
        buildPreview.SetPosition(CurrentPreviewPosition);

        Debug.Log("Preview moved to: " + CurrentPreviewPosition);
    }

    // Return true when placement input can be used
    public bool CanReceivePlacementInput()
    {
        return IsPlacing && Time.unscaledTime >= inputUnlockTime;
    }

    // Return the configured cost for a build type
    public List<ResourceAmount> GetBuildCost(BuildType buildType)
    {
        foreach (BuildCostData buildCost in buildCosts)
        {
            if (buildCost.buildType == buildType)
            {
                return buildCost.costs;
            }
        }

        return null;
    }

    // Check if placement can begin in the current game state
    private bool CanStartPlacement()
    {
        GameState currentState = GameStateManager.Instance.CurrentState;

        return currentState == GameState.Preparation ||
               currentState == GameState.StructureSelected ||
               currentState == GameState.Combat;
    }

    // Called when the game state changes
    private void OnStateChanged(object eventData)
    {
        if (!(eventData is GameState newState))
        {
            return;
        }

        HandleStateChanged(newState);
    }

    // React to the current game state and cancel invalid placement flows
    private void HandleStateChanged(GameState newState)
    {
        if (!IsPlacing)
        {
            return;
        }

        bool shouldStopPlacement =
            newState == GameState.Paused ||
            newState == GameState.BlessingSelection ||
            newState == GameState.Victory ||
            newState == GameState.Defeat;

        if (shouldStopPlacement)
        {
            Debug.Log("Build placement stopped because of state change: " + newState);
            ResetPlacement();
        }
    }

    // Restore default focus to build cards after exiting placement
    private void RestoreBuildCardsFocus()
    {
        if (buildCardsNavigationUI != null)
        {
            buildCardsNavigationUI.SelectDefault();
        }
    }

    // Reset the current placement state
    private void ResetPlacement()
    {
        IsPlacing = false;
        CurrentBuildType = default;
        CurrentPreviewPosition = defaultPreviewPosition;
        returnStateAfterPlacement = GameState.Preparation;
        inputUnlockTime = 0f;

        if (buildPreview != null)
        {
            buildPreview.gameObject.SetActive(false);
        }
    }
}