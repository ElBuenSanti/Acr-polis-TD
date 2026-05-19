using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SettingsPanelUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private CanvasGroup settingsGroup;
    [SerializeField] private CanvasGroup pauseGroup;

    [Header("UI")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider uiSlider;
    [SerializeField] private Slider ambienceSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private GameObject firstSelectedObject;
    [SerializeField] private Button pauseFirstSelectedButton;

    [Header("Animation")]
    [SerializeField] private float fadeSpeed = 10f;

    [Header("Sound")]
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

    private void Update()
    {
        float targetAlpha = isOpen ? 1f : 0f;

        if (settingsGroup != null)
        {
            settingsGroup.alpha = Mathf.MoveTowards(
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

        if (firstSelectedObject != null && EventSystem.current != null)
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

        if (pauseFirstSelectedButton != null && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(pauseFirstSelectedButton.gameObject);
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
    }
}