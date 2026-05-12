using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource ambienceSource;
    [SerializeField] private AudioSource uiSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Settings")]
    [SerializeField] private float musicFadeTime = 0.6f;
    [SerializeField] private float ambienceFadeTime = 0.6f;

    private Coroutine musicRoutine;
    private Coroutine ambienceRoutine;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayUI(AudioClip clip)
    {
        PlayOneShot(uiSource, clip);
    }

    public void PlaySFX(AudioClip clip)
    {
        PlayOneShot(sfxSource, clip);
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource == null || clip == null)
            return;

        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        if (musicRoutine != null)
            StopCoroutine(musicRoutine);

        musicRoutine = StartCoroutine(ChangeLoopingAudioRoutine(
            musicSource,
            clip,
            loop,
            musicFadeTime
        ));
    }

    public void PlayAmbience(AudioClip clip, bool loop = true)
    {
        if (ambienceSource == null || clip == null)
            return;

        if (ambienceSource.clip == clip && ambienceSource.isPlaying)
            return;

        if (ambienceRoutine != null)
            StopCoroutine(ambienceRoutine);

        ambienceRoutine = StartCoroutine(ChangeLoopingAudioRoutine(
            ambienceSource,
            clip,
            loop,
            ambienceFadeTime
        ));
    }

    public void StopMusic(float duration = 1f)
    {
        if (musicSource == null)
            return;

        if (musicRoutine != null)
            StopCoroutine(musicRoutine);

        musicRoutine = StartCoroutine(FadeOutAndStopRoutine(musicSource, duration));
    }

    public void StopAmbience(float duration = 1f)
    {
        if (ambienceSource == null)
            return;

        if (ambienceRoutine != null)
            StopCoroutine(ambienceRoutine);

        ambienceRoutine = StartCoroutine(FadeOutAndStopRoutine(ambienceSource, duration));
    }

    public void FadeOutMusic(float duration = 1f)
    {
        StopMusic(duration);
    }

    public void SetMixerVolume(string parameterName, float value)
    {
        if (audioMixer == null)
            return;

        float volume = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(parameterName, volume);
    }

    private void PlayOneShot(AudioSource source, AudioClip clip)
    {
        if (source == null || clip == null)
            return;

        source.PlayOneShot(clip);
    }

    private IEnumerator ChangeLoopingAudioRoutine(AudioSource source, AudioClip newClip, bool loop, float fadeTime)
    {
        float originalVolume = source.volume;
        float timer = 0f;

        if (source.isPlaying)
        {
            while (timer < fadeTime)
            {
                timer += Time.unscaledDeltaTime;
                source.volume = Mathf.Lerp(originalVolume, 0f, timer / fadeTime);
                yield return null;
            }
        }

        source.clip = newClip;
        source.loop = loop;
        source.Play();

        timer = 0f;

        while (timer < fadeTime)
        {
            timer += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(0f, originalVolume, timer / fadeTime);
            yield return null;
        }

        source.volume = originalVolume;
    }

    private IEnumerator FadeOutAndStopRoutine(AudioSource source, float duration)
    {
        float originalVolume = source.volume;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(originalVolume, 0f, timer / duration);
            yield return null;
        }

        source.Stop();
        source.volume = originalVolume;
    }
}