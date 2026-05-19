using UnityEngine;

public class MusicManager : MonoBehaviour
{
    // Patrón Singleton: Permite que cualquier script del juego controle la música de fondo usando 'MusicManager.Instance'
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

    // Inicialización del Singleton para el control de la música
    private void Awake()
    {
        // Si ya existe un MusicManager activo, destruye este componente duplicado para no interferir
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    // Activa de manera simultánea la música y el sonido ambiental para el Menú Principal
    public void PlayMainMenuMusic()
    {
        PlayMusic(mainMenuMusic);
        PlayAmbience(cityAmbience);
    }

    // Activa la música tranquila y el ambiente urbano para la fase de construcción o preparación
    public void PlayConstructionMusic()
    {
        PlayMusic(constructionMusic);
        PlayAmbience(cityAmbience);
    }

    // Cambia el ambiente y la pista musical a un tono tenso cuando inicia el combate ordinario
    public void PlayCombatMusic()
    {
        PlayMusic(combatMusic);
        PlayAmbience(combatAmbience);
    }

    // Cambia la música a un tema especial cuando se alcanza una oleada importante (Hito/Milestone)
    public void PlayMilestoneMusic()
    {
        PlayMusic(milestoneMusic);
        PlayAmbience(combatAmbience);
    }

    // Activa la música y el sonido ambiental más épicos o peligrosos para la última oleada del juego
    public void PlayFinalWaveMusic()
    {
        PlayMusic(finalWaveMusic);
        PlayAmbience(finalWaveAmbience);
    }

    // Detiene por completo toda la reproducción de fondo aplicando un desvanecimiento de 1 segundo
    public void StopAllMusicAndAmbience()
    {
        // Validación preventiva para asegurar que el sistema base de audio responda correctamente
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance.StopMusic(1f);
        AudioManager.Instance.StopAmbience(1f);
    }

    // Método privado que sirve como puente seguro para enviar la música al AudioManager global
    private void PlayMusic(AudioClip clip)
    {
        // LÍNEA RARA / COMPLEJA: Se comunica con el AudioManager a través de su Singleton. 
        // El condicional '!= null' protege al juego de romperse con un error "NullReferenceException" 
        // en caso de que intentemos probar una escena de manera aislada en Unity sin el AudioManager cargado.
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayMusic(clip);
    }

    // Método privado que sirve como puente seguro para enviar el sonido de ambiente al AudioManager global
    private void PlayAmbience(AudioClip clip)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayAmbience(clip);
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Director Coreográfico de la Atmósfera (MusicManager) del videojuego. Su función 
   principal es definir e intercalar qué "estados de audio" o atmósferas completas deben sonar según la situación 
   actual del juego (Menú, Construcción, Combate, Oleada Crítica o Batalla Final).

   Características clave:
   1. Control de Atmósferas Duales: En lugar de manejar pistas sueltas, empareja de manera lógica una pista 
      musical (Music) con una pista de fondo ambiental (Ambience) para que cambien sincronizadas con un solo comando.
   2. Capa de Abstracción Semántica: Permite que los scripts que controlan el flujo de las oleadas o del juego 
      no tengan que saber qué archivo de sonido reproducir; solo llaman a métodos intuitivos como 'PlayCombatMusic()'.
   3. Interfaz Centralizada de Archivos: Reúne en un solo lugar del inspector de Unity todos los Assets de audio 
      largos (bucles/loops), facilitando el intercambio de bandas sonoras o audios ambientales sin tocar otros scripts.
   ========================================================================================================
*/