using UnityEngine;

// Debug input used to test victory and defeat placeholder screens
public class VictoryDefeatDebugInput : MonoBehaviour
{
    private void Update()
    {
        if (GameStateManager.Instance == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.F7))
        {
            GameStateManager.Instance.ChangeState(GameState.Victory);
            Debug.Log("Debug: Victory state triggered.");
        }

        if (Input.GetKeyDown(KeyCode.F8))
        {
            GameStateManager.Instance.ChangeState(GameState.Defeat);
            Debug.Log("Debug: Defeat state triggered.");
        }
    }
}