using UnityEngine;

// Handles the settings menu placeholder behavior
public class SettingsMenuUI : MonoBehaviour
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