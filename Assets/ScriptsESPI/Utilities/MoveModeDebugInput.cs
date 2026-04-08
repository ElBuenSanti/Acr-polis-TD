using UnityEngine;
using UnityEngine.EventSystems;

// Debug input used to test move mode
public class MoveModeDebugInput : MonoBehaviour
{
    [SerializeField] private MoveModeSystem moveModeSystem;
    [SerializeField] private float moveStep = 2f;

    private void Update()
    {
        if (moveModeSystem == null || !moveModeSystem.IsMoving)
        {
            return;
        }

        if (IsUIFocused())
        {
            return;
        }

        if (!moveModeSystem.CanReceiveMoveInput())
        {
            return;
        }

        // T = up, G = down, F = left, H = right
        if (Input.GetKeyDown(KeyCode.T))
        {
            moveModeSystem.MovePreviewBy(new Vector3(0f, 0f, moveStep));
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            moveModeSystem.MovePreviewBy(new Vector3(0f, 0f, -moveStep));
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            moveModeSystem.MovePreviewBy(new Vector3(-moveStep, 0f, 0f));
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            moveModeSystem.MovePreviewBy(new Vector3(moveStep, 0f, 0f));
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            moveModeSystem.ConfirmMove();
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            moveModeSystem.CancelMove();
        }
    }

    private bool IsUIFocused()
    {
        return EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null;
    }
}