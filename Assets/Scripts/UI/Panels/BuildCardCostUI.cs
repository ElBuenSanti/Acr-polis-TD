using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Handles cost display and affordability feedback for a build card
public class BuildCardCostUI : MonoBehaviour
{
    [Header("Card Settings")]
    [SerializeField] private BuildType buildType;

    [Header("References")]
    [SerializeField] private TMP_Text costText;
    [SerializeField] private BuildPlacementSystem buildPlacementSystem;
    [SerializeField] private ResourceSystem resourceSystem;

    private void OnEnable()
    {
        GameEvents.StartListening(EventNames.ResourcesChanged, OnResourcesChanged);
        RefreshCostDisplay();
    }

    private void OnDisable()
    {
        GameEvents.StopListening(EventNames.ResourcesChanged, OnResourcesChanged);
    }

    // Refresh card visuals when resources change
    private void OnResourcesChanged(object eventData)
    {
        RefreshCostDisplay();
    }

    // Update the displayed cost text
    private void RefreshCostDisplay()
    {
        if (costText == null || buildPlacementSystem == null)
        {
            return;
        }

        List<ResourceAmount> costList = buildPlacementSystem.GetBuildCost(buildType);

        if (costList == null || costList.Count == 0)
        {
            costText.text = "Free";
            return;
        }

        string textValue = BuildCostText(costList);

        if (resourceSystem != null && !resourceSystem.CanAfford(costList))
        {
            textValue += " - Not enough";
        }

        costText.text = textValue;
    }

    // Build a readable cost string
    private string BuildCostText(List<ResourceAmount> costList)
    {
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
}