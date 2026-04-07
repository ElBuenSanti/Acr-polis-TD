using UnityEngine;

// Handles the pause menu flow
public class PauseMenuUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject pauseMenuContent;
    [SerializeField] private GameObject settingsMenuContent;
    [SerializeField] private GameObject controlsMenuContent;

    private void Awake()
    {
        SetPauseMenuVisible(false);
        SetSettingsMenuVisible(false);
        SetControlsMenuVisible(false);
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
        SetPauseMenuVisible(true);
        SetSettingsMenuVisible(false);
        SetControlsMenuVisible(false);
    }

    // Open the settings menu
    public void OpenSettingsMenu()
    {
        SetPauseMenuVisible(false);
        SetSettingsMenuVisible(true);
        SetControlsMenuVisible(false);
    }

    // Open the controls menu
    public void OpenControlsMenu()
    {
        SetPauseMenuVisible(false);
        SetSettingsMenuVisible(false);
        SetControlsMenuVisible(true);
    }

    // Return to the pause menu
    public void BackToPauseMenu()
    {
        OpenPauseMenu();
    }

    // Resume the game
    public void ResumeGame()
    {
        if (GameStateManager.Instance == null)
        {
            return;
        }

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

    // Close all pause-related menus
    private void CloseAllMenus()
    {
        SetPauseMenuVisible(false);
        SetSettingsMenuVisible(false);
        SetControlsMenuVisible(false);
    }

    private void SetPauseMenuVisible(bool isVisible)
    {
        if (pauseMenuContent != null)
        {
            pauseMenuContent.SetActive(isVisible);
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
}