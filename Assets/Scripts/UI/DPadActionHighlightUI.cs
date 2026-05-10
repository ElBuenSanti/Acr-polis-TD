using UnityEngine;

public class DPadActionHighlightUI : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private float activeScale = 1.15f;
    [SerializeField] private float animationSpeed = 12f;

    private bool isActive;
    private Vector3 originalScale;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        originalScale = rectTransform.localScale;

        SetActive(false);
    }

    private void Update()
    {
        float targetAlpha = isActive ? 1f : 0f;
        Vector3 targetScale = isActive ? originalScale * activeScale : originalScale;

        if (canvasGroup != null)
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * animationSpeed);

        if (rectTransform != null)
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, Time.deltaTime * animationSpeed);
    }

    public void SetActive(bool active)
    {
        isActive = active;
    }
}