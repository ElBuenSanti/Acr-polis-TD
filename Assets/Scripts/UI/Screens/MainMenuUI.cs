using UnityEngine;

// Handles the main menu visibility and flow
public class MainMenuUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject mainMenuContent;
    [SerializeField] private GameObject gameplayHUDRoot;
    [SerializeField] private GameObject settingsMenuContent;
    [SerializeField] private GameObject controlsMenuContent;
    [SerializeField] private GameObject mainMenuPanel;

    private void Awake()
    {
        OpenMainMenu();
    }

    // Show the main menu and hide all other main-menu-related screens
    public void OpenMainMenu()
    {
        SetMainMenuVisible(true);
        SetGameplayHUDVisible(false);
        SetSettingsMenuVisible(false);
        SetControlsMenuVisible(false);
    }

    // Start the game and show gameplay HUD
    public void StartGame()
    {
        SetMainMenuVisible(false);
        SetSettingsMenuVisible(false);
        SetControlsMenuVisible(false);
        SetMainMenuPanelVisible(false);
        SetGameplayHUDVisible(true);

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
        SetMainMenuVisible(false);
        SetSettingsMenuVisible(true);
        SetControlsMenuVisible(false);
    }

    // Open controls from the main menu
    public void OpenControlsFromMainMenu()
    {
        Debug.Log("Main menu -> Open Controls");
        SetMainMenuVisible(false);
        SetSettingsMenuVisible(false);
        SetControlsMenuVisible(true);
    }

    // Return from settings or controls back to main menu
    public void BackToMainMenu()
    {
        Debug.Log("Return to Main Menu");
        OpenMainMenu();
    }

    private void SetMainMenuVisible(bool isVisible)
    {
        if (mainMenuContent != null)
        {
            mainMenuContent.SetActive(isVisible);
        }
    }

    private void SetGameplayHUDVisible(bool isVisible)
    {
        if (gameplayHUDRoot != null)
        {
            gameplayHUDRoot.SetActive(isVisible);
        }
    }

    private void SetSettingsMenuVisible(bool isVisible)
    {
        if (settingsMenuContent != null)
        {
            settingsMenuContent.SetActive(isVisible);
        }
    }

    private void SetControlsMenuVisible(bool isVisible)
    {
        if (controlsMenuContent != null)
        {
            controlsMenuContent.SetActive(isVisible);
        }
    }

    private void SetMainMenuPanelVisible(bool isVisible)
    {
        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(isVisible);
        }
    }
}