using UnityEngine;

public class ResourcesDebugListener : MonoBehaviour
{
    [SerializeField] private ResourceSystem resourceSystem;

    private void OnEnable()
    {
        GameEvents.StartListening(EventNames.ResourcesChanged, OnResourcesChanged);
    }

    private void OnDisable()
    {
        GameEvents.StopListening(EventNames.ResourcesChanged, OnResourcesChanged);
    }

    // This method is called when the resources changed event is triggered
    private void OnResourcesChanged(object eventData)
    {
        if (resourceSystem == null)
        {
            return;
        }

        Debug.Log("Resources updated -> Agape: " +
                  resourceSystem.GetResourceAmount(ResourceType.Agape) +
                  " | Ira: " +
                  resourceSystem.GetResourceAmount(ResourceType.Ira) +
                  " | Meraki: " +
                  resourceSystem.GetResourceAmount(ResourceType.Meraki));
    }
}