using UnityEngine;

// Represents the visual preview shown during building placement
public class BuildPreview : MonoBehaviour
{
    [Header("Preview Settings")]
    [SerializeField] private BuildType buildType;

    public BuildType BuildType => buildType;

    // Set the preview world position
    public void SetPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    // Assign the build type to this preview
    public void SetBuildType(BuildType newBuildType)
    {
        buildType = newBuildType;
    }
}