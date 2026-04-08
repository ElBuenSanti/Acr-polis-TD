using System.Collections.Generic;
using UnityEngine;

// Handles cooldown logic for actions used during combat
public class CooldownSystem : MonoBehaviour
{
    [Header("Cooldown Settings")]
    [SerializeField] private List<CooldownData> cooldownSettings = new List<CooldownData>();

    private Dictionary<CooldownType, float> cooldownDurationDictionary;
    private Dictionary<CooldownType, float> cooldownEndTimeDictionary;

    private void Awake()
    {
        InitializeCooldowns();
    }

    // Create internal cooldown dictionaries
    private void InitializeCooldowns()
    {
        cooldownDurationDictionary = new Dictionary<CooldownType, float>();
        cooldownEndTimeDictionary = new Dictionary<CooldownType, float>();

        foreach (CooldownData cooldownData in cooldownSettings)
        {
            if (!cooldownDurationDictionary.ContainsKey(cooldownData.cooldownType))
            {
                cooldownDurationDictionary.Add(cooldownData.cooldownType, cooldownData.duration);
            }
        }
    }

    // Start cooldown for a specific action type
    public void StartCooldown(CooldownType cooldownType)
    {
        if (!cooldownDurationDictionary.ContainsKey(cooldownType))
        {
            Debug.LogWarning("CooldownSystem: No duration configured for " + cooldownType);
            return;
        }

        float duration = cooldownDurationDictionary[cooldownType];
        float endTime = Time.time + duration;

        cooldownEndTimeDictionary[cooldownType] = endTime;

        Debug.Log("Cooldown started: " + cooldownType + " for " + duration + " seconds.");
        GameEvents.TriggerEvent(EventNames.CooldownUpdated, cooldownType);
    }

    // Check if a cooldown is currently active
    public bool IsOnCooldown(CooldownType cooldownType)
    {
        if (!cooldownEndTimeDictionary.ContainsKey(cooldownType))
        {
            return false;
        }

        return Time.time < cooldownEndTimeDictionary[cooldownType];
    }

    // Get remaining cooldown time
    public float GetRemainingCooldown(CooldownType cooldownType)
    {
        if (!cooldownEndTimeDictionary.ContainsKey(cooldownType))
        {
            return 0f;
        }

        float remainingTime = cooldownEndTimeDictionary[cooldownType] - Time.time;
        return Mathf.Max(0f, remainingTime);
    }

    // Get normalized cooldown progress from 0 to 1
    public float GetCooldownProgress(CooldownType cooldownType)
    {
        if (!cooldownDurationDictionary.ContainsKey(cooldownType))
        {
            return 0f;
        }

        if (!cooldownEndTimeDictionary.ContainsKey(cooldownType))
        {
            return 0f;
        }

        float duration = cooldownDurationDictionary[cooldownType];
        float remainingTime = GetRemainingCooldown(cooldownType);

        if (duration <= 0f)
        {
            return 0f;
        }

        return remainingTime / duration;
    }

    // Return true if the action can currently be used
    public bool CanUse(CooldownType cooldownType)
    {
        if (GameStateManager.Instance == null)
        {
            return false;
        }

        GameState currentState = GameStateManager.Instance.CurrentState;

        // In preparation, everything is free
        if (currentState == GameState.Preparation || currentState == GameState.StructureSelected)
        {
            return true;
        }

        // In combat, actions depend on cooldown
        if (currentState == GameState.Combat)
        {
            return !IsOnCooldown(cooldownType);
        }

        return false;
    }
}