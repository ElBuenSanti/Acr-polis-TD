using UnityEngine;

public class BlessingCardHighlightUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private float selectedScale = 1.08f;
    [SerializeField] private float speed = 12f;

    private bool selected;
    private Vector3 normalScale;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (rectTransform == null)
            rectTransform = transform.parent.GetComponent<RectTransform>();

        normalScale = rectTransform.localScale;
    }

    private void Update()
    {
        float targetAlpha = selected ? 1f : 0f;
        Vector3 targetScale = selected ? normalScale * selectedScale : normalScale;

        if (canvasGroup != null)
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.unscaledDeltaTime * speed);

        if (rectTransform != null)
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, Time.unscaledDeltaTime * speed);
    }

    public void SetSelected(bool value)
    {
        selected = value;
    }
}