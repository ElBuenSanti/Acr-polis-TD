using UnityEngine;

// Handles the main menu visibility and flow
public class MainMenuUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject mainMenuContent;
    [SerializeField] private UIScreenFlowController screenFlowController;

    private void Awake()
    {
        if (screenFlowController != null)
        {
            screenFlowController.ShowMainMenu();
        }
    }

    // Show the main menu and hide all other screens
    public void OpenMainMenu()
    {
        if (screenFlowController != null)
        {
            screenFlowController.ShowMainMenu();
        }

        SelectMenuDefault(mainMenuContent);
    }

    // Start the game and show gameplay HUD
    public void StartGame()
    {
        if (screenFlowController != null)
        {
            screenFlowController.ShowGameplay();
        }

        if (GameStateManager.Instance != null)
        {
            GameStateManager.Instance.ChangeState(GameState.Preparation);
        }

        Debug.Log("Main menu -> Start Game");
    }

    // Open settings from the main menu
    public void OpenSettingsFromMainMenu()
    {
        Debug.Log("Main menu -> Open Settings");

        if (screenFlowController != null)
        {
            screenFlowController.ShowSettingsFromMainMenu();
        }
    }

    // Open controls from the main menu
    public void OpenControlsFromMainMenu()
    {
        Debug.Log("Main menu -> Open Controls");

        if (screenFlowController != null)
        {
            screenFlowController.ShowControlsFromMainMenu();
        }
    }

    // Return from settings or controls back to main menu
    public void BackToMainMenu()
    {
        Debug.Log("Return to Main Menu");

        if (screenFlowController != null)
        {
            screenFlowController.BackToMainMenuScreen();
        }

        SelectMenuDefault(mainMenuContent);
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