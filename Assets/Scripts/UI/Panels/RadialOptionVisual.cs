using UnityEngine;
using UnityEngine.UI;

public class RadialOptionVisual : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform targetTransform;

    [Header("Settings")]
    [SerializeField] private float activeScale = 1.08f;
    [SerializeField] private float inactiveScale = 1f;
    [SerializeField] private float activeAlpha = 1f;
    [SerializeField] private float inactiveAlpha = 0f;
    [SerializeField] private float animationSpeed = 12f;

    private bool isActive;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (targetTransform == null)
            targetTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        float targetAlpha = isActive ? activeAlpha : inactiveAlpha;
        float targetScale = isActive ? activeScale : inactiveScale;

        if (canvasGroup != null)
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * animationSpeed);

        if (targetTransform != null)
            targetTransform.localScale = Vector3.Lerp(
                targetTransform.localScale,
                Vector3.one * targetScale,
                Time.deltaTime * animationSpeed
            );
    }

    public void SetActiveVisual(bool active)
    {
        isActive = active;
    }
}