using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuSettingsUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle fullscreenToggle;

    [Header("Navigation")]
    [SerializeField] private GameObject firstSelectedObject;

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;

    private void Start()
    {
        if (musicSlider != null)
        {
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
            SetMusicVolume(musicSlider.value);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
            SetSFXVolume(sfxSlider.value);
        }

        if (fullscreenToggle != null)
            fullscreenToggle.isOn = Screen.fullScreen;
    }

    public void SelectFirstObject()
    {
        if (firstSelectedObject != null && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(firstSelectedObject);
    }

    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
        SetMixerVolume("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
        SetMixerVolume("SFXVolume", value);
    }

    public void SetFullscreen(bool active)
    {
        Screen.fullScreen = active;
        PlayerPrefs.SetInt("Fullscreen", active ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void SetMixerVolume(string parameterName, float value)
    {
        if (audioMixer == null)
            return;

        float volume = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(parameterName, volume);
    }
}