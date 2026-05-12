using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonJuice : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private RectTransform target;
    [SerializeField] private CanvasGroup glowGroup;
    [SerializeField] private float selectedScale = 1.08f;
    [SerializeField] private float speed = 12f;
    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private AudioClip confirmClip;

    private bool selected;
    private Vector3 normalScale;

    private void Awake()
    {
        if (target == null)
            target = GetComponent<RectTransform>();

        normalScale = target.localScale;

        if (glowGroup != null)
            glowGroup.alpha = 0f;
    }

    private void Update()
    {
        Vector3 targetScale = selected ? normalScale * selectedScale : normalScale;
        target.localScale = Vector3.Lerp(target.localScale, targetScale, Time.unscaledDeltaTime * speed);

        if (glowGroup != null)
            glowGroup.alpha = Mathf.Lerp(glowGroup.alpha, selected ? 1f : 0f, Time.unscaledDeltaTime * speed);
    }

    public void OnSelect(BaseEventData eventData)
    {
        selected = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        selected = false;
    }
}