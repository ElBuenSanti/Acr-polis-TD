using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenuUI : MonoBehaviour
{
    [SerializeField] private Canvas pauseCanvas;
    [SerializeField] private Button firstSelectedButton;

    private void Start()
    {
        Close();
    }

    public void Open()
    {
        Time.timeScale = 0f;

        if (pauseCanvas != null)
            pauseCanvas.enabled = true;

        GameStateController.Instance.SetState(GameState.Paused);

        if (firstSelectedButton != null)
            EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
    }

    public void Close()
    {
        Time.timeScale = 1f;

        if (pauseCanvas != null)
            pauseCanvas.enabled = false;

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
}