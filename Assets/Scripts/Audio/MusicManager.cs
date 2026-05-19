using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [Header("Music Clips")]
    [SerializeField] private AudioClip mainMenuMusic;
    [SerializeField] private AudioClip constructionMusic;
    [SerializeField] private AudioClip combatMusic;
    [SerializeField] private AudioClip milestoneMusic;
    [SerializeField] private AudioClip finalWaveMusic;

    [Header("Ambience Clips")]
    [SerializeField] private AudioClip cityAmbience;
    [SerializeField] private AudioClip combatAmbience;
    [SerializeField] private AudioClip finalWaveAmbience;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public void PlayMainMenuMusic()
    {
        PlayMusic(mainMenuMusic);
        PlayAmbience(cityAmbience);
    }

    public void PlayConstructionMusic()
    {
        PlayMusic(constructionMusic);
        PlayAmbience(cityAmbience);
    }

    public void PlayCombatMusic()
    {
        PlayMusic(combatMusic);
        PlayAmbience(combatAmbience);
    }

    public void PlayMilestoneMusic()
    {
        PlayMusic(milestoneMusic);
        PlayAmbience(combatAmbience);
    }

    public void PlayFinalWaveMusic()
    {
        PlayMusic(finalWaveMusic);
        PlayAmbience(finalWaveAmbience);
    }

    public void StopAllMusicAndAmbience()
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance.StopMusic(1f);
        AudioManager.Instance.StopAmbience(1f);
    }

    private void PlayMusic(AudioClip clip)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMusic(clip);
    }

    private void PlayAmbience(AudioClip clip)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayAmbience(clip);
    }
}