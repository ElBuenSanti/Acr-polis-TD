using UnityEngine;

// Debug input used to test building placement flow
public class BuildPlacementDebugInput : MonoBehaviour
{
    [SerializeField] private BuildPlacementSystem buildPlacementSystem;

    private void Update()
    {
        if (buildPlacementSystem == null || !buildPlacementSystem.IsPlacing)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            buildPlacementSystem.MovePreview(new Vector3(0f, 0f, 2f));
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            buildPlacementSystem.MovePreview(new Vector3(0f, 0f, -2f));
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            buildPlacementSystem.MovePreview(new Vector3(-2f, 0f, 0f));
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            buildPlacementSystem.MovePreview(new Vector3(2f, 0f, 0f));
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            buildPlacementSystem.ConfirmPlacement();
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            buildPlacementSystem.CancelPlacement();
        }
    }
}