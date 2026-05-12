using UnityEngine;

public class PulseGlowUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup glow;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float minAlpha = 0.25f;
    [SerializeField] private float maxAlpha = 0.8f;

    private void Update()
    {
        float pulse = Mathf.Lerp(
            minAlpha,
            maxAlpha,
            (Mathf.Sin(Time.unscaledTime * speed) + 1f) * 0.5f
        );

        glow.alpha = pulse;
    }
}