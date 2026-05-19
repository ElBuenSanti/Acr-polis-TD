using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuSettingsUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider uiSlider;
    [SerializeField] private Slider ambienceSlider;
    [SerializeField] private Toggle fullscreenToggle;

    [Header("Navigation")]
    [SerializeField] private GameObject firstSelectedObject;

    private void Start()
    {
        float musicValue = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxValue = PlayerPrefs.GetFloat("SFXVolume", 1f);
        float uiValue = PlayerPrefs.GetFloat("UIVolume", 1f);
        float ambienceValue = PlayerPrefs.GetFloat("AmbienceVolume", 1f);
        bool fullscreen = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;


        if (musicSlider != null)
            musicSlider.SetValueWithoutNotify(musicValue);

        if (sfxSlider != null)
            sfxSlider.SetValueWithoutNotify(sfxValue);

        if (uiSlider != null)
            uiSlider.SetValueWithoutNotify(uiValue);

        if (ambienceSlider != null)
            ambienceSlider.SetValueWithoutNotify(ambienceValue);


        if (fullscreenToggle != null)
            fullscreenToggle.SetIsOnWithoutNotify(fullscreen);

        SetMusicVolume(musicValue);
        SetSFXVolume(sfxValue);
        SetUIVolume(uiValue);
        SetAmbienceVolume(ambienceValue);
        SetFullscreen(fullscreen);
    }

    public void SelectFirstObject()
    {
        if (firstSelectedObject != null && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(firstSelectedObject);
    }

    public void SetMusicVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(value);
    }

    public void SetSFXVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(value);
    }

    public void SetUIVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetUIVolume(value);
    }

    public void SetAmbienceVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetAmbienceVolume(value);
    }

    public void SetFullscreen(bool active)
    {
        Screen.fullScreenMode = active
            ? FullScreenMode.ExclusiveFullScreen
            : FullScreenMode.Windowed;

        Screen.fullScreen = active;

        PlayerPrefs.SetInt("Fullscreen", active ? 1 : 0);
        PlayerPrefs.Save();
    }
}