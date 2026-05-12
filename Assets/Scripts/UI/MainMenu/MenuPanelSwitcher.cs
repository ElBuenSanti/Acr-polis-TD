using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuPanelSwitcher : MonoBehaviour
{
    [SerializeField] private CanvasGroup mainGroup;
    [SerializeField] private CanvasGroup settingsGroup;
    [SerializeField] private CanvasGroup controlsGroup;

    [SerializeField] private Button mainFirstButton;
    [SerializeField] private Button settingsFirstButton;
    [SerializeField] private Button controlsFirstButton;

    [SerializeField] private float fadeSpeed = 10f;

    private CanvasGroup activeGroup;

    private void Start()
    {
        ShowMainInstant();
    }

    private void Update()
    {
        FadeGroup(mainGroup);
        FadeGroup(settingsGroup);
        FadeGroup(controlsGroup);
    }

    public void ShowMain()
    {
        SetActiveGroup(mainGroup, mainFirstButton);
    }

    public void ShowSettings()
    {
        SetActiveGroup(settingsGroup, settingsFirstButton);
    }

    public void ShowControls()
    {
        SetActiveGroup(controlsGroup, controlsFirstButton);
    }

    private void ShowMainInstant()
    {
        activeGroup = mainGroup;

        SetGroupInstant(mainGroup, true);
        SetGroupInstant(settingsGroup, false);
        SetGroupInstant(controlsGroup, false);

        if (mainFirstButton != null)
            EventSystem.current.SetSelectedGameObject(mainFirstButton.gameObject);
    }

    private void SetActiveGroup(CanvasGroup group, Button firstButton)
    {
        activeGroup = group;

        SetInteractable(mainGroup, group == mainGroup);
        SetInteractable(settingsGroup, group == settingsGroup);
        SetInteractable(controlsGroup, group == controlsGroup);

        if (firstButton != null)
            EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
    }

    private void FadeGroup(CanvasGroup group)
    {
        if (group == null)
            return;

        float target = group == activeGroup ? 1f : 0f;
        group.alpha = Mathf.Lerp(group.alpha, target, Time.unscaledDeltaTime * fadeSpeed);
    }

    private void SetGroupInstant(CanvasGroup group, bool active)
    {
        if (group == null)
            return;

        group.alpha = active ? 1f : 0f;
        SetInteractable(group, active);
    }

    private void SetInteractable(CanvasGroup group, bool active)
    {
        if (group == null)
            return;

        group.interactable = active;
        group.blocksRaycasts = active;
    }
}