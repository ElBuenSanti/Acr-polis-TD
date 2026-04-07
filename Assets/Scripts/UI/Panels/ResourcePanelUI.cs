using TMPro;
using UnityEngine;

// Handles the resources panel UI and updates resource text values
public class ResourcesPanelUI : MonoBehaviour
{
    [Header("Text References")]
    [SerializeField] private TMP_Text agapeText;
    [SerializeField] private TMP_Text iraText;
    [SerializeField] private TMP_Text merakiText;

    [Header("System Reference")]
    [SerializeField] private ResourceSystem resourceSystem;

    private void OnEnable()
    {
        GameEvents.StartListening(EventNames.ResourcesChanged, OnResourcesChanged);
        UpdateResourceTexts();
    }

    private void OnDisable()
    {
        GameEvents.StopListening(EventNames.ResourcesChanged, OnResourcesChanged);
    }

    // Called when the resource values change
    private void OnResourcesChanged(object eventData)
    {
        UpdateResourceTexts();
    }

    // Update all resource text fields with current values
    private void UpdateResourceTexts()
    {
        if (resourceSystem == null)
        {
            Debug.LogWarning("ResourcesPanelUI: ResourceSystem reference is missing.");
            return;
        }

        if (agapeText == null || iraText == null || merakiText == null)
        {
            Debug.LogWarning("ResourcesPanelUI: One or more text references are missing.");
            return;
        }

        agapeText.text = "Agape: " + resourceSystem.GetResourceAmount(ResourceType.Agape);
        iraText.text = "Ira: " + resourceSystem.GetResourceAmount(ResourceType.Ira);
        merakiText.text = "Meraki: " + resourceSystem.GetResourceAmount(ResourceType.Meraki);
    }
}