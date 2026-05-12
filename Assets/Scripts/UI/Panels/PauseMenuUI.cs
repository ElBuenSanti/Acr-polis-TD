using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private Canvas pauseCanvas;
    [SerializeField] private Button firstSelectedButton;

    [SerializeField] private UISoundPlayer uiSoundPlayer;


    private void Start()
    {
        Close();
    }

    public void Open()
    {
        Time.timeScale = 0f;
        AudioListener.pause = true;

        if (pauseCanvas != null)
            pauseCanvas.enabled = true;

        if (uiSoundPlayer != null)
            uiSoundPlayer.PlayOpenPanel();

        GameStateController.Instance.SetState(GameState.Paused);

        if (firstSelectedButton != null)
            EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
    }

    public void Close()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (pauseCanvas != null)
            pauseCanvas.enabled = false;

        if (uiSoundPlayer != null)
            uiSoundPlayer.PlayConfirm();

        EventSystem.current.SetSelectedGameObject(null);
        GameStateController.Instance.SetState(GameState.MapIdle);
    }

    public void Toggle()
    {
        if (GameStateController.Instance.currentState == GameState.Paused)
            Close();
        else
            Open();
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}