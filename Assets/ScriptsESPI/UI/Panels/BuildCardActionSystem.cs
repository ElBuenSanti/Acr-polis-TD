using UnityEngine;

// Handles gameplay logic when a build card is used
public class BuildCardActionSystem : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BuildPlacementSystem buildPlacementSystem;
    [SerializeField] private CooldownSystem cooldownSystem;
    [SerializeField] private BuildCardSelectionMemory buildCardSelectionMemory;

    // Try to use a build card from UI or keyboard flow
    public bool TryUseBuildCard(BuildType buildType)
    {
        if (buildPlacementSystem == null)
        {
            Debug.LogWarning("BuildCardActionSystem: BuildPlacementSystem reference is missing.");
            return false;
        }

        if (cooldownSystem == null)
        {
            Debug.LogWarning("BuildCardActionSystem: CooldownSystem reference is missing.");
            return false;
        }

        if (buildCardSelectionMemory == null)
        {
            Debug.LogWarning("BuildCardActionSystem: BuildCardSelectionMemory reference is missing.");
            return false;
        }

        if (GameStateManager.Instance == null)
        {
            Debug.LogWarning("BuildCardActionSystem: GameStateManager instance is missing.");
            return false;
        }

        CooldownType cooldownType = GetCooldownType(buildType);

        if (!cooldownSystem.CanUse(cooldownType))
        {
            Debug.Log("BuildCardActionSystem: Build card is on cooldown: " + cooldownType);
            return false;
        }

        buildCardSelectionMemory.SetSelectedBuildType(buildType);

        GameState currentState = GameStateManager.Instance.CurrentState;

        buildPlacementSystem.StartPlacement(buildType);

        if (buildPlacementSystem.IsPlacing && currentState == GameState.Combat)
        {
            cooldownSystem.StartCooldown(cooldownType);
        }

        Debug.Log("BuildCardActionSystem: Build card used -> " + buildType);
        return buildPlacementSystem.IsPlacing;
    }

    private CooldownType GetCooldownType(BuildType buildType)
    {
        switch (buildType)
        {
            case BuildType.Temple:
                return CooldownType.BuildTemple;

            case BuildType.Barracks:
                return CooldownType.BuildBarracks;

            case BuildType.Defense:
                return CooldownType.BuildDefense;
        }

        return CooldownType.BuildTemple;
    }
}