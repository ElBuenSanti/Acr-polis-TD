using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ControlsPanelUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private CanvasGroup controlsGroup;
    [SerializeField] private CanvasGroup pauseGroup;

    [Header("UI")]
    [SerializeField] private Button firstSelectedButton;
    [SerializeField] private Button pauseFirstSelectedButton;

    [Header("Animation")]
    [SerializeField] private float fadeSpeed = 10f;

    private bool isOpen;

    private void Awake()
    {
        if (controlsGroup == null)
            controlsGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        HideInstant();
    }

    private void Update()
    {
        float targetAlpha = isOpen ? 1f : 0f;

        if (controlsGroup != null)
        {
            controlsGroup.alpha = Mathf.Lerp(
                controlsGroup.alpha,
                targetAlpha,
                Time.unscaledDeltaTime * fadeSpeed
            );
        }
    }

    public void Open()
    {
        isOpen = true;

        SetGroup(pauseGroup, false);
        SetGroup(controlsGroup, true);

        GameStateController.Instance.SetState(GameState.Controls);

        if (firstSelectedButton != null)
            EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
    }

    public void CloseToPause()
    {
        isOpen = false;

        SetGroup(controlsGroup, false);
        SetGroup(pauseGroup, true);

        GameStateController.Instance.SetState(GameState.Paused);

        if (pauseFirstSelectedButton != null)
            EventSystem.current.SetSelectedGameObject(pauseFirstSelectedButton.gameObject);
    }

    private void HideInstant()
    {
        isOpen = false;

        if (controlsGroup != null)
        {
            controlsGroup.alpha = 0f;
            controlsGroup.interactable = false;
            controlsGroup.blocksRaycasts = false;
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