using UnityEngine;
using UnityEngine.EventSystems;

// Debug input used to test building placement flow
public class BuildPlacementDebugInput : MonoBehaviour
{
    [SerializeField] private BuildPlacementSystem buildPlacementSystem;
    [SerializeField] private float moveStep = 2f;

    private void Update()
    {
        if (buildPlacementSystem == null || !buildPlacementSystem.IsPlacing)
        {
            return;
        }

        if (IsUIFocused())
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            buildPlacementSystem.MovePreviewBy(new Vector3(0f, 0f, moveStep));
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            buildPlacementSystem.MovePreviewBy(new Vector3(0f, 0f, -moveStep));
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            buildPlacementSystem.MovePreviewBy(new Vector3(-moveStep, 0f, 0f));
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            buildPlacementSystem.MovePreviewBy(new Vector3(moveStep, 0f, 0f));
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            buildPlacementSystem.ConfirmPlacement();
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            buildPlacementSystem.CancelPlacement();
        }
    }

    private bool IsUIFocused()
    {
        return EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null;
    }
}