using UnityEngine;
using UnityEngine.UI;

// Handles a single build card button
public class BuildCardUI : MonoBehaviour
{
    [Header("Card Settings")]
    [SerializeField] private BuildType buildType;

    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private BuildPlacementSystem buildPlacementSystem;
    [SerializeField] private CooldownSystem cooldownSystem;

    private void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnCardPressed);
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnCardPressed);
        }
    }

    // Called when the build card is pressed
    private void OnCardPressed()
    {
        if (buildPlacementSystem == null)
        {
            Debug.LogWarning("BuildCardUI: BuildPlacementSystem reference is missing.");
            return;
        }

        if (GameStateManager.Instance == null)
        {
            Debug.LogWarning("BuildCardUI: GameStateManager instance is missing.");
            return;
        }

        if (cooldownSystem == null)
        {
            Debug.LogWarning("BuildCardUI: CooldownSystem reference is missing.");
            return;
        }

        CooldownType cooldownType = GetCooldownType();

        if (!cooldownSystem.CanUse(cooldownType))
        {
            Debug.Log("Build card is on cooldown: " + cooldownType);
            return;
        }

        GameState currentState = GameStateManager.Instance.CurrentState;

        buildPlacementSystem.StartPlacement(buildType);

        if (buildPlacementSystem.IsPlacing && currentState == GameState.Combat)
        {
            cooldownSystem.StartCooldown(cooldownType);
        }
    }

    // Convert build type into cooldown type
    private CooldownType GetCooldownType()
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