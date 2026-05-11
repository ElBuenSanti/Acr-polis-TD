using UnityEngine;

public class BasePanelUI : MonoBehaviour
{
    [Header("Base Panel")]
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected float fadeSpeed = 10f;

    protected bool isOpen;

    protected virtual void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    protected virtual void Update()
    {
        float targetAlpha = isOpen ? 1f : 0f;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = Mathf.MoveTowards(
                canvasGroup.alpha,
                targetAlpha,
                Time.unscaledDeltaTime * fadeSpeed
            );
        }
    }

    public virtual void Open()
    {
        isOpen = true;
        SetInteractable(true);
    }

    public virtual void Close()
    {
        isOpen = false;
        SetInteractable(false);
    }

    public virtual void HideInstant()
    {
        isOpen = false;

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        SetInteractable(false);
    }

    protected void ShowInstant()
    {
        isOpen = true;

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;

        SetInteractable(true);
    }

    protected void SetInteractable(bool active)
    {
        if (canvasGroup == null)
            return;

        canvasGroup.interactable = active;
        canvasGroup.blocksRaycasts = active;
    }
}