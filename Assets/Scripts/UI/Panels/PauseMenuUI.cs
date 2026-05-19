using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Canvas pauseCanvas;
    [SerializeField] private Button firstSelectedButton;

    [Header("Sound")]
    [SerializeField] private UISoundPlayer uiSoundPlayer;

    private void Start()
    {
        CloseInstant();
    }

    public void Open()
    {
        Time.timeScale = 0f;

        // NO pausar audio completamente
        AudioListener.pause = false;

        ApplyPausedAudio();

        if (pauseCanvas != null)
            pauseCanvas.enabled = true;

        if (uiSoundPlayer != null)
            uiSoundPlayer.PlayOpenPanel();

        GameStateController.Instance.SetState(GameState.Paused);

        if (firstSelectedButton != null && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
    }

    public void Close()
    {
        Time.timeScale = 1f;

        AudioListener.pause = false;

        RestoreAudio();

        if (pauseCanvas != null)
            pauseCanvas.enabled = false;

        if (uiSoundPlayer != null)
            uiSoundPlayer.PlayConfirm();

        if (EventSystem.current != null)
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
        AudioListener.pause = false;

        RestoreAudio();

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        RestoreAudio();

        SceneManager.LoadScene("MainMenu");
    }

    private void ApplyPausedAudio()
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance.SetMixerVolume("MusicVolume", 0.20f);
        AudioManager.Instance.SetMixerVolume("AmbienceVolume", 0.15f);
        AudioManager.Instance.SetMixerVolume("SFXVolume", 0.35f);

        // UI normal para hover/click
        AudioManager.Instance.SetMixerVolume(
            "UIVolume",
            PlayerPrefs.GetFloat("UIVolume", 1f)
        );
    }

    private void RestoreAudio()
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance.SetMixerVolume(
            "MusicVolume",
            PlayerPrefs.GetFloat("MusicVolume", 1f)
        );

        AudioManager.Instance.SetMixerVolume(
            "SFXVolume",
            PlayerPrefs.GetFloat("SFXVolume", 1f)
        );

        AudioManager.Instance.SetMixerVolume(
            "UIVolume",
            PlayerPrefs.GetFloat("UIVolume", 1f)
        );

        AudioManager.Instance.SetMixerVolume(
            "AmbienceVolume",
            PlayerPrefs.GetFloat("AmbienceVolume", 1f)
        );
    }

    private void CloseInstant()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (pauseCanvas != null)
            pauseCanvas.enabled = false;
    }
}