using System.Collections.Generic;
using UnityEngine;

public class ResourceSystem : MonoBehaviour
{
    [Header("Starting Resources")]
    [SerializeField] private int startingAgape = 100;
    [SerializeField] private int startingIra = 0;
    [SerializeField] private int startingMeraki = 0;

    private Dictionary<ResourceType, int> resourceDictionary;

    private void Awake()
    {
        InitializeResources();
    }

    private void Start()
    {
        Debug.Log("ResourceSystem initialized.");
        Debug.Log("Agape: " + GetResourceAmount(ResourceType.Agape));
        Debug.Log("Ira: " + GetResourceAmount(ResourceType.Ira));
        Debug.Log("Meraki: " + GetResourceAmount(ResourceType.Meraki));

        NotifyResourcesChanged();
    }

    // Create the internal resource dictionary and set starting values
    private void InitializeResources()
    {
        resourceDictionary = new Dictionary<ResourceType, int>();

        resourceDictionary[ResourceType.Agape] = startingAgape;
        resourceDictionary[ResourceType.Ira] = startingIra;
        resourceDictionary[ResourceType.Meraki] = startingMeraki;
    }

    // Return the current amount of a specific resource
    public int GetResourceAmount(ResourceType resourceType)
    {
        if (resourceDictionary.TryGetValue(resourceType, out int amount))
        {
            return amount;
        }

        return 0;
    }

    // Add an amount to a specific resource
    public void AddResource(ResourceType resourceType, int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        resourceDictionary[resourceType] += amount;

        Debug.Log("Added " + amount + " " + resourceType);
        NotifyResourcesChanged();
    }

    // Check if the player has enough of a specific resource
    public bool HasEnoughResource(ResourceType resourceType, int amount)
    {
        return GetResourceAmount(resourceType) >= amount;
    }

    // Spend an amount of a specific resource if possible
    public bool SpendResource(ResourceType resourceType, int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        if (!HasEnoughResource(resourceType, amount))
        {
            Debug.LogWarning("Not enough " + resourceType);
            return false;
        }

        resourceDictionary[resourceType] -= amount;

        Debug.Log("Spent " + amount + " " + resourceType);
        NotifyResourcesChanged();

        return true;
    }

    // Check if the player can afford a list of resource costs
    public bool CanAfford(List<ResourceAmount> costs)
    {
        if (costs == null || costs.Count == 0)
        {
            return true;
        }

        foreach (ResourceAmount cost in costs)
        {
            if (!HasEnoughResource(cost.resourceType, cost.amount))
            {
                return false;
            }
        }

        return true;
    }

    // Spend a list of resource costs if all requirements are met
    public bool SpendResources(List<ResourceAmount> costs)
    {
        if (!CanAfford(costs))
        {
            Debug.LogWarning("Cannot afford this cost.");
            return false;
        }

        foreach (ResourceAmount cost in costs)
        {
            resourceDictionary[cost.resourceType] -= cost.amount;
        }

        Debug.Log("Multiple resources spent successfully.");
        NotifyResourcesChanged();

        return true;
    }

    // Send an event when resource values change
    private void NotifyResourcesChanged()
    {
        GameEvents.TriggerEvent(EventNames.ResourcesChanged, this);
    }
}