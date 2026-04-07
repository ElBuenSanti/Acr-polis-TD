using UnityEngine;

// Handles the controls menu placeholder behavior
public class ControlsMenuUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PauseMenuUI pauseMenuUI;
    [SerializeField] private MainMenuUI mainMenuUI;

    public void Back()
    {
        if (GameStateManager.Instance != null && GameStateManager.Instance.CurrentState == GameState.Paused)
        {
            if (pauseMenuUI != null)
            {
                pauseMenuUI.BackToPauseMenu();
                return;
            }
        }

        if (mainMenuUI != null)
        {
            mainMenuUI.BackToMainMenu();
        }
    }
}