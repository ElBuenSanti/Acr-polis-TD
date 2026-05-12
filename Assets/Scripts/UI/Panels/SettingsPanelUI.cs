using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Audio;

public class SettingsPanelUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private CanvasGroup settingsGroup;
    [SerializeField] private CanvasGroup pauseGroup;

    [Header("UI")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private GameObject firstSelectedObject;
    [SerializeField] private Button pauseFirstSelectedButton;

    [Header("Animation")]
    [SerializeField] private float fadeSpeed = 10f;

    [Header("Audio")]
    [SerializeField] private AudioMixer audioMixer;

    [SerializeField] private UISoundPlayer uiSoundPlayer;
    private bool isOpen;

    private void Awake()
    {
        if (settingsGroup == null)
            settingsGroup = GetComponent<CanvasGroup>();
        if (uiSoundPlayer == null)
            uiSoundPlayer = FindAnyObjectByType<UISoundPlayer>();
    }

    private void Start()
    {
        HideInstant();

        if (musicSlider != null)
            musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);

        if (sfxSlider != null)
            sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        if (fullscreenToggle != null)
            fullscreenToggle.isOn = Screen.fullScreen;

        SetMixerVolume("MusicVolume", musicSlider != null ? musicSlider.value : 1f);
        SetMixerVolume("SFXVolume", sfxSlider != null ? sfxSlider.value : 1f);
    }

    private void Update()
    {
        float targetAlpha = isOpen ? 1f : 0f;

        if (settingsGroup != null)
        {
            settingsGroup.alpha = Mathf.Lerp(
                 settingsGroup.alpha,
                 targetAlpha,
                 Time.unscaledDeltaTime * fadeSpeed
            );
        }
    }

    public void Open()
    {

        if (uiSoundPlayer != null)
            uiSoundPlayer.PlayOpenPanel();

        GameStateController.Instance.SetState(GameState.Settings);

        isOpen = true;

        SetGroup(pauseGroup, false);
        SetGroup(settingsGroup, true);

        if (firstSelectedObject != null)
            EventSystem.current.SetSelectedGameObject(firstSelectedObject);
    }

    public void CloseToPause()
    {

        if (uiSoundPlayer != null)
            uiSoundPlayer.PlayClosePanel();

        GameStateController.Instance.SetState(GameState.Paused);

        isOpen = false;

        SetGroup(settingsGroup, false);
        SetGroup(pauseGroup, true);

        if (pauseFirstSelectedButton != null)
            EventSystem.current.SetSelectedGameObject(pauseFirstSelectedButton.gameObject);
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

    private void SetMixerVolume(string parameterName, float value)
    {
        if (audioMixer == null)
            return;

        float volume = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(parameterName, volume);
    }

    public void SetFullscreen(bool active)
    {
        Screen.fullScreen = active;
        PlayerPrefs.SetInt("Fullscreen", active ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void HideInstant()
    {
        isOpen = false;

        if (settingsGroup != null)
        {
            settingsGroup.alpha = 0f;
            settingsGroup.interactable = false;
            settingsGroup.blocksRaycasts = false;
        }
    }

    private void SetGroup(CanvasGroup group, bool active)
    {
        if (group == null)
            return;

        group.interactable = active;
        group.blocksRaycasts = active;

        if (active)
            group.alpha = 1f;
    }
}