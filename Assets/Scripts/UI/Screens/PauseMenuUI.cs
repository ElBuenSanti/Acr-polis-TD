using UnityEngine;

// Handles the pause menu flow
public class PauseMenuUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject pauseMenuContent;
    [SerializeField] private GameObject settingsMenuContent;
    [SerializeField] private GameObject controlsMenuContent;
    [SerializeField] private UIScreenFlowController screenFlowController;

    private void Awake()
    {
        CloseAllMenus();
    }

    private void OnEnable()
    {
        GameEvents.StartListening(EventNames.StateChanged, OnStateChanged);
    }

    private void OnDisable()
    {
        GameEvents.StopListening(EventNames.StateChanged, OnStateChanged);
    }

    // Called when the game state changes
    private void OnStateChanged(object eventData)
    {
        if (!(eventData is GameState newState))
        {
            return;
        }

        if (newState == GameState.Paused)
        {
            OpenPauseMenu();
        }
        else
        {
            CloseAllMenus();
        }
    }

    // Open the base pause menu
    public void OpenPauseMenu()
    {
        if (screenFlowController != null)
        {
            screenFlowController.ShowPause();
        }

        SelectMenuDefault(pauseMenuContent);
    }

    // Open the settings menu
    public void OpenSettingsMenu()
    {
        if (screenFlowController != null)
        {
            screenFlowController.ShowSettingsFromPause();
        }

        SelectMenuDefault(settingsMenuContent);
    }

    // Open the controls menu
    public void OpenControlsMenu()
    {
        if (screenFlowController != null)
        {
            screenFlowController.ShowControlsFromPause();
        }

        SelectMenuDefault(controlsMenuContent);
    }

    // Return to the pause menu
    public void BackToPauseMenu()
    {
        if (screenFlowController != null)
        {
            screenFlowController.BackToPauseScreen();
        }

        SelectMenuDefault(pauseMenuContent);
    }

    // Resume the game
    public void ResumeGame()
    {
        if (screenFlowController != null)
        {
            screenFlowController.HidePause();
        }

        if (GameStateManager.Instance == null)
        {
            return;
        }

        GameStateManager.Instance.ChangeState(GameState.Combat);
    }

    // Return to main menu from pause
    public void ReturnToMainMenu()
    {
        if (screenFlowController != null)
        {
            screenFlowController.ShowMainMenu();
        }

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.Preparation);
        }

        Debug.Log("Pause menu -> Return to Main Menu");
    }

    // Close all pause-related menus
    private void CloseAllMenus()
    {
        if (screenFlowController != null)
        {
            screenFlowController.HidePause();
        }
    }

    // Force the default selection for a menu content root
    private void SelectMenuDefault(GameObject menuContent)
    {
        if (menuContent == null)
        {
            return;
        }

        MenuNavigationUI navigationUI = menuContent.GetComponent<MenuNavigationUI>();

        if (navigationUI != null)
        {
            navigationUI.SelectDefault();
        }
    }
}