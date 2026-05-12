using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuUI : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string gameSceneName = "GameScene";

    [Header("UI")]
    [SerializeField] private Button firstSelectedButton;

    private void Start()
    {
        Time.timeScale = 1f;

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayMainMenuMusic();

        if (firstSelectedButton != null && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
    }

    public void Play()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}