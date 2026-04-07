using UnityEngine;

// Handles button actions from the main menu
public class MainMenuButtonActions : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private MainMenuUI mainMenuUI;

    public void StartGame()
    {
        if (mainMenuUI != null)
        {
            mainMenuUI.StartGame();
        }
    }

    public void OpenSettings()
    {
        if (mainMenuUI != null)
        {
            mainMenuUI.OpenSettingsFromMainMenu();
        }
    }

    public void OpenControls()
    {
        if (mainMenuUI != null)
        {
            mainMenuUI.OpenControlsFromMainMenu();
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quit button pressed.");
        Application.Quit();
    }
}