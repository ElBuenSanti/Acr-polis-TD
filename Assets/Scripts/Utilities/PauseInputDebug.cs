using UnityEngine;

// Debug input used to open or close pause state
public class PauseInputDebug : MonoBehaviour
{
    private void Update()
    {
        if (GameStateManager.Instance == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameState currentState = GameStateManager.Instance.CurrentState;

            if (currentState == GameState.Paused)
            {
                GameState mainState = GameStateManager.Instance.LastMainState;

                if (mainState == GameState.Paused)
                {
                    GameStateManager.Instance.ChangeState(GameState.Combat);
                }
                else
                {
                    GameStateManager.Instance.ChangeState(GameState.Preparation);
                }
            }
            else
            {
                GameStateManager.Instance.ChangeState(GameState.Paused);
            }
        }
    }
}