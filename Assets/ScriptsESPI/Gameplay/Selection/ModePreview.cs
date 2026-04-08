using UnityEngine;

// Represents the visual preview shown while moving a selected structure
public class MovePreview : MonoBehaviour
{
    // Set the preview world position
    public void SetPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }
}