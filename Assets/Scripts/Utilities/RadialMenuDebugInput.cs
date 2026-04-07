using UnityEngine;

// Debug input used to test radial menu flow
public class RadialMenuDebugInput : MonoBehaviour
{
    [SerializeField] private RadialMenuUI radialMenuUI;

    private void Update()
    {
        if (radialMenuUI == null)
        {
            return;
        }

        // Cancel the current radial selection flow
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            radialMenuUI.CancelSelection();
        }
    }
}