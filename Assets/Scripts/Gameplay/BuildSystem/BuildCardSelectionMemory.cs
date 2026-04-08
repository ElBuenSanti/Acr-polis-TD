using UnityEngine;

// Stores the last selected build card so other UI systems can use it
public class BuildCardSelectionMemory : MonoBehaviour
{
    public bool HasSelectedBuildCard { get; private set; }
    public BuildType CurrentBuildType { get; private set; }

    // Save the current build card selection
    public void SetSelectedBuildType(BuildType buildType)
    {
        CurrentBuildType = buildType;
        HasSelectedBuildCard = true;

        Debug.Log("BuildCardSelectionMemory saved build type: " + buildType);
    }

    // Clear the current build card selection
    public void ClearSelection()
    {
        HasSelectedBuildCard = false;
        CurrentBuildType = default;

        Debug.Log("BuildCardSelectionMemory cleared.");
    }
}