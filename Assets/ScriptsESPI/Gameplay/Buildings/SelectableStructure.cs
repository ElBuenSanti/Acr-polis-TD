using UnityEngine;

// Basic selectable structure used for testing the selection system
public class SelectableStructure : MonoBehaviour, ISelectable
{
    [Header("Structure Info")]
    [SerializeField] private string displayName = "Structure";
    [SerializeField] private string structureType = "Utility";
    [SerializeField] private string description = "Basic structure description.";
    [SerializeField] private string stats = "Production: 0\nCadence: None\nRange: None";

    // Called when this structure becomes selected
    public void OnSelected()
    {
        Debug.Log(displayName + " selected.");
    }

    // Called when this structure is no longer selected
    public void OnDeselected()
    {
        Debug.Log(displayName + " deselected.");
    }

    // Return the display name for UI panels
    public string GetDisplayName()
    {
        return displayName;
    }

    // Return the structure type for UI panels
    public string GetStructureType()
    {
        return structureType;
    }

    // Return the structure description for UI panels
    public string GetDescription()
    {
        return description;
    }

    // Return the structure stats for UI panels
    public string GetStats()
    {
        return stats;
    }
}