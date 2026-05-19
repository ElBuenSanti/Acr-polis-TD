using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    // Patrón Singleton: Permite que cualquier otro script acceda al AudioManager usando 'AudioManager.Instance'
    public static AudioManager Instance;

    // Claves constantes para guardar y cargar los ajustes de volumen en el almacenamiento local (PlayerPrefs)
    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";
    private const string UI_KEY = "UIVolume";
    private const string AMBIENCE_KEY = "AmbienceVolume";

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

    // Contenedores para controlar las corrutinas activas y evitar que se encimen efectos de transición
    private Coroutine musicRoutine;
    private Coroutine ambienceRoutine;

    // Inicialización del Singleton y persistencia entre escenas
    private void Awake()
    {
        // Si ya existe otra instancia de AudioManager en el juego, destruye esta nueva para evitar duplicados
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // Hace que este objeto no se destruya al cambiar de escena (mantiene la música sonando de fondo)
        DontDestroyOnLoad(gameObject);
        LoadSavedVolumes();
    }

    // Reproduce un sonido de interfaz de usuario de manera inmediata
    public void PlayUI(AudioClip clip)
    {
        PlayOneShot(uiSource, clip);
    }

    // Reproduce un efecto de sonido del juego (SFX) de manera inmediata
    public void PlaySFX(AudioClip clip)
    {
        PlayOneShot(sfxSource, clip);
    }

    // Cambia la música actual aplicando un efecto de transición (Fade Out y Fade In)
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource == null || clip == null)
            return;

        // Si la canción solicitada ya se está reproduciendo, no hace nada para no reiniciar el tema
        if (musicSource.clip == clip && musicSource.isPlaying)
            return;

        // Si ya se estaba ejecutando una transición de música, la detiene para iniciar la nueva
        if (musicRoutine != null)
            StopCoroutine(musicRoutine);

        // Inicia la corrutina que baja el volumen de la música vieja y sube el de la nueva
        musicRoutine = StartCoroutine(ChangeLoopingAudioRoutine(
            musicSource,
            clip,
            loop,
            musicFadeTime
        ));
    }

    // Cambia el sonido ambiental actual aplicando un efecto de transición suave
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

    // Detiene la música bajando el volumen gradualmente en un tiempo determinado
    public void StopMusic(float duration = 1f)
    {
        if (musicSource == null)
            return;

        if (musicRoutine != null)
            StopCoroutine(musicRoutine);

        musicRoutine = StartCoroutine(FadeOutAndStopRoutine(musicSource, duration));
    }

    // Detiene el sonido ambiental bajando el volumen gradualmente
    public void StopAmbience(float duration = 1f)
    {
        if (ambienceSource == null)
            return;

        if (ambienceRoutine != null)
            StopCoroutine(ambienceRoutine);

        ambienceRoutine = StartCoroutine(FadeOutAndStopRoutine(ambienceSource, duration));
    }

    // Acceso directo público para apagar la música de forma gradual (llama internamente a StopMusic)
    public void FadeOutMusic(float duration = 1f)
    {
        StopMusic(duration);
    }

    // Carga los niveles de volumen guardados previamente en el dispositivo al iniciar el juego
    public void LoadSavedVolumes()
    {
        // El valor '1f' al final es el valor por defecto si el jugador abre el juego por primera vez
        SetMixerVolume("MusicVolume", PlayerPrefs.GetFloat(MUSIC_KEY, 1f));
        SetMixerVolume("SFXVolume", PlayerPrefs.GetFloat(SFX_KEY, 1f));
        SetMixerVolume("UIVolume", PlayerPrefs.GetFloat(UI_KEY, 1f));
        SetMixerVolume("AmbienceVolume", PlayerPrefs.GetFloat(AMBIENCE_KEY, 1f));
    }

    // Modifica y guarda permanentemente el volumen de la música
    public void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat(MUSIC_KEY, value);
        PlayerPrefs.Save();

        SetMixerVolume("MusicVolume", value);
    }

    // Modifica y guarda permanentemente el volumen de los efectos de sonido
    public void SetSFXVolume(float value)
    {
        PlayerPrefs.SetFloat(SFX_KEY, value);
        PlayerPrefs.Save();

        SetMixerVolume("SFXVolume", value);
    }

    // Modifica y guarda permanentemente el volumen de la interfaz de usuario
    public void SetUIVolume(float value)
    {
        PlayerPrefs.SetFloat(UI_KEY, value);
        PlayerPrefs.Save();

        SetMixerVolume("UIVolume", value);
    }

    // Modifica y guarda permanentemente el volumen del sonido ambiental
    public void SetAmbienceVolume(float value)
    {
        PlayerPrefs.SetFloat(AMBIENCE_KEY, value);
        PlayerPrefs.Save();

        SetMixerVolume("AmbienceVolume", value);
    }

    // Traduce los valores de sliders de UI (0 a 1) a la escala logarítmica de decibelios del AudioMixer (-80dB a 20dB)
    public void SetMixerVolume(string parameterName, float value)
    {
        if (audioMixer == null)
            return;

        // EXPLICACIÓN COMPLEJA: Los AudioMixer de Unity no se controlan de 0 a 1, sino por Decibelios (donde 0dB es normal y -80dB es silencio).
        // 'Mathf.Clamp' asegura que el valor nunca sea exactamente 0 (ya que el logaritmo de 0 no existe matemáticamente y daría un error).
        // 'Mathf.Log10' convierte la escala lineal en logarítmica (como escucha el oído humano), y se multiplica por 20 para dar los decibelios correctos.
        float volume = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(parameterName, volume);
    }

    // Método optimizado para reproducir clips cortos sin interrumpir otros sonidos del mismo canal
    private void PlayOneShot(AudioSource source, AudioClip clip)
    {
        if (source == null || clip == null)
            return;

        // 'PlayOneShot' permite encimar sonidos en el mismo AudioSource (útil para ráfagas de disparos o clics rápidos de menú)
        source.PlayOneShot(clip);
    }

    // Corrutina encargada de hacer un "Crossfade" (apagar un sonido gradualmente, cambiar el clip y encender el nuevo de forma suave)
    private IEnumerator ChangeLoopingAudioRoutine(AudioSource source, AudioClip newClip, bool loop, float fadeTime)
    {
        float originalVolume = source.volume;
        float timer = 0f;

        // FADE OUT: Si ya está sonando algo, baja el volumen progresivamente hasta 0
        if (source.isPlaying)
        {
            while (timer < fadeTime)
            {
                // 'Time.unscaledDeltaTime' asegura que el volumen cambie fluidamente incluso si el juego está pausado (Time.timeScale = 0)
                timer += Time.unscaledDeltaTime;
                // 'Mathf.Lerp' calcula el punto intermedio entre el volumen original y cero basándose en el tiempo transcurrido
                source.volume = Mathf.Lerp(originalVolume, 0f, timer / fadeTime);
                yield return null; // Pausa la ejecución hasta el siguiente frame del juego
            }
        }

        // CAMBIO DE TRACK: Se asigna la nueva canción, se configura el bucle y se le da Play
        source.clip = newClip;
        source.loop = loop;
        source.Play();

        timer = 0f;

        // FADE IN: Sube el volumen gradualmente desde 0 hasta recuperar su potencia original
        while (timer < fadeTime)
        {
            timer += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(0f, originalVolume, timer / fadeTime);
            yield return null;
        }

        // Asegura que el volumen quede exactamente en su nivel original al terminar el ciclo numérico del Lerp
        source.volume = originalVolume;
    }

    // Corrutina para desvanecer un canal de audio por completo hasta detener su reproducción
    private IEnumerator FadeOutAndStopRoutine(AudioSource source, float duration)
    {
        float originalVolume = source.volume;
        float timer = 0f;

        // Reduce el volumen de forma continua durante la duración especificada
        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(originalVolume, 0f, timer / duration);
            yield return null;
        }

        source.Stop(); // Detiene por completo el reproductor de audio
        source.volume = originalVolume; // Restablece el valor del volumen del componente para cuando vuelva a usarse
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script es un Administrador de Audio global (AudioManager) diseñado bajo el patrón Singleton. 
   Su función principal es centralizar, controlar y organizar la reproducción de todo el apartado sonoro del juego, 
   dividiéndolo en 4 canales específicos: Música, Ambiente, Efectos de Sonido (SFX) e Interfaz de Usuario (UI).

   Características clave:
   1. Transiciones Suaves (Crossfades): Automatiza el proceso de "Fade Out" (bajar volumen) y "Fade In" (subir volumen) 
      a través de Corrutinas independientes para que los cambios de música o ambiente no sean abruptos para el jugador.
   2. Persistencia entre Niveles: Utiliza 'DontDestroyOnLoad', garantizando que la música no se corte al cambiar de pantallas o mapas.
   3. Sistema de Guardado (Save/Load): Gestiona de forma automática la persistencia de los volúmenes en el dispositivo 
      usando 'PlayerPrefs', permitiendo que los ajustes que configure el jugador en el menú de opciones se guarden de forma permanente.
   4. Conversión Logarítmica: Incluye una conversión matemática de lineal a logarítmica para modificar correctamente los 
      parámetros del AudioMixer de Unity, imitando la percepción auditiva humana.
   ========================================================================================================
*/