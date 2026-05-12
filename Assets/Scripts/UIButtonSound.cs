using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonSound : MonoBehaviour, ISelectHandler, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private UISoundPlayer uiSoundPlayer;

    private void Awake()
    {
        if (uiSoundPlayer == null)
            uiSoundPlayer = FindAnyObjectByType<UISoundPlayer>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (uiSoundPlayer != null)
            uiSoundPlayer.PlayHover();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (uiSoundPlayer != null)
            uiSoundPlayer.PlayHover();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (uiSoundPlayer != null)
            uiSoundPlayer.PlayConfirm();
    }
}