using UnityEngine;

// Debug input used to test move mode
public class MoveModeDebugInput : MonoBehaviour
{
    [SerializeField] private MoveModeSystem moveModeSystem;

    private void Update()
    {
        if (moveModeSystem == null || !moveModeSystem.IsMoving)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            moveModeSystem.MovePreviewTo(new Vector3(0f, 0f, 2f));
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            moveModeSystem.MovePreviewTo(new Vector3(0f, 0f, -2f));
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            moveModeSystem.MovePreviewTo(new Vector3(-2f, 0f, 0f));
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            moveModeSystem.MovePreviewTo(new Vector3(2f, 0f, 0f));
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            moveModeSystem.ConfirmMove();
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            moveModeSystem.CancelMove();
        }
    }
}