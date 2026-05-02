using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHighlight : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public RectTransform rect;

    private Vector3 normalScale;
    private Vector3 selectedScale = Vector3.one * 1.1f;

    private void Awake()
    {
        normalScale = rect.localScale;
    }

    public void OnSelect(BaseEventData eventData)
    {
        rect.localScale = selectedScale;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        rect.localScale = normalScale;
    }
}