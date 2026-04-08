using UnityEngine;

// Debug input used to test blessing selection placeholder
public class BlessingSelectionDebugInput : MonoBehaviour
{
    private void Update()
    {
        if (GameStateManager.Instance == null)
        {
            Debug.LogWarning("BlessingSelectionDebugInput: GameStateManager is missing.");
            return;
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            Debug.Log("Debug: Trying to trigger BlessingSelection.");
            bool changed = GameStateManager.Instance.ChangeState(GameState.BlessingSelection);
            Debug.Log("Debug: ChangeState returned = " + changed);
        }
    }
}