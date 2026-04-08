using UnityEngine;

public class ResourceSystemDebugInput : MonoBehaviour
{
    [SerializeField] private ResourceSystem resourceSystem;

    private void Update()
    {
        if (resourceSystem == null)
        {
            Debug.LogWarning("ResourceSystemDebugInput: ResourceSystem reference is missing.");
            return;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Pressed Q");
            resourceSystem.AddResource(ResourceType.Agape, 10);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("Pressed W");
            resourceSystem.AddResource(ResourceType.Ira, 5);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Pressed E");
            resourceSystem.AddResource(ResourceType.Meraki, 5);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("Pressed A");
            resourceSystem.SpendResource(ResourceType.Agape, 20);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Pressed S");
            resourceSystem.SpendResource(ResourceType.Ira, 10);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            Debug.Log("Pressed D");
            resourceSystem.SpendResource(ResourceType.Meraki, 10);
        }
    }
}