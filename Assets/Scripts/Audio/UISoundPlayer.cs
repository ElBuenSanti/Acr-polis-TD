using UnityEngine;

public class UISoundPlayer : MonoBehaviour
{
    [Header("UI Clips")]
    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private AudioClip confirmClip;
    [SerializeField] private AudioClip cancelClip;
    [SerializeField] private AudioClip openPanelClip;
    [SerializeField] private AudioClip closePanelClip;
    [SerializeField] private AudioClip errorClip;

    public void PlayHover()
    {
        Play(hoverClip);
    }

    public void PlayConfirm()
    {
        Play(confirmClip);
    }

    public void PlayCancel()
    {
        Play(cancelClip);
    }

    public void PlayOpenPanel()
    {
        Play(openPanelClip);
    }

    public void PlayClosePanel()
    {
        Play(closePanelClip);
    }

    public void PlayError()
    {
        Play(errorClip);
    }

    private void Play(AudioClip clip)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayUI(clip);
    }
}