using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Canvas pauseCanvas;
    [SerializeField] private Button firstSelectedButton;

    [Header("Sound")]
    [SerializeField] private UISoundPlayer uiSoundPlayer;

    // Asegura el reinicio y apagado del lienzo al inicializar el nivel
    private void Start()
    {
        CloseInstant();
    }

    // LÍNEA RARA / COMPLEJA: Pausa Lógica con Atenuación Acústica Dinámica ('Open').
    // Paraliza las mecánicas físicas usando 'Time.timeScale = 0f', pero mantiene 'AudioListener.pause = false'.
    // Esto evita el silencio digital absoluto y permite aplicar un filtro de volumen ('ApplyPausedAudio') 
    // para que la música de fondo y el ambiente sigan sonando de manera atenuada (efecto "low-pass" o sordera),
    // manteniendo la interfaz con un feedback sonoro vivo e interactivo.
    public void Open()
    {
        Time.timeScale = 0f;

        // NO pausar audio completamente
        AudioListener.pause = false;

        ApplyPausedAudio();

        if (pauseCanvas != null)
            pauseCanvas.enabled = true;

        if (uiSoundPlayer != null)
            uiSoundPlayer.PlayOpenPanel();

        GameStateController.Instance.SetState(GameState.Paused);

        if (firstSelectedButton != null && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
    }

    // LÍNEA RARA / COMPLEJA: Restauración de Foco y Contexto de Juego ('Close').
    // Reestablece la velocidad del mundo, desactiva el lienzo del menú y limpia por completo el foco del 'EventSystem' 
    // pasándole un parámetro 'null'. Esto evita que un botón invisible del menú de pausa retenga la selección del teclado o gamepad,
    // garantizando que los comandos vuelvan a dirigirse limpiamente a los sistemas tácticos del mapa.
    public void Close()
    {
        Time.timeScale = 1f;

        AudioListener.pause = false;

        RestoreAudio();

        if (pauseCanvas != null)
            pauseCanvas.enabled = false;

        if (uiSoundPlayer != null)
            uiSoundPlayer.PlayConfirm();

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        GameStateController.Instance.SetState(GameState.MapIdle);
    }

    // Alterna de forma inteligente el estado del menú evaluando el estado del gestor central
    public void Toggle()
    {
        if (GameStateController.Instance.currentState == GameState.Paused)
            Close();
        else
            Open();
    }

    // LÍNEA RARA / COMPLEJA: Reinicio por Índice Activo ('SceneManager.GetActiveScene().buildIndex').
    // En lugar de escribir el nombre del nivel como una cadena de texto manual (lo que provocaría fallos si se renombra el archivo),
    // interroga directamente al gestor de escenas para obtener el índice entero único del nivel en ejecución.
    // Reestablece las propiedades acústicas y de tiempo antes de invocar la recarga síncrona para prevenir bloqueos de inicialización.
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        RestoreAudio();

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }

    // Devuelve el flujo del juego a la escena principal limpiando las escalas de tiempo e hilos de audio
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        RestoreAudio();

        SceneManager.LoadScene("MainMenu");
    }

    // LÍNEA RARA / COMPLEJA: Modulador de Mezclador de Audio en Estado de Pausa ('ApplyPausedAudio').
    // Fuerza valores decimales específicos a los canales del mezclador musical para sumergir el audio en segundo plano.
    // Sin embargo, lee el canal "UIVolume" directamente desde las preferencias guardadas del jugador ('PlayerPrefs'), 
    // garantizando que los efectos de sonido del menú (clics, hovers) se escuchen con la potencia original elegida en los ajustes.
    private void ApplyPausedAudio()
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance.SetMixerVolume("MusicVolume", 0.20f);
        AudioManager.Instance.SetMixerVolume("AmbienceVolume", 0.15f);
        AudioManager.Instance.SetMixerVolume("SFXVolume", 0.35f);

        // UI normal para hover/click
        AudioManager.Instance.SetMixerVolume(
            "UIVolume",
            PlayerPrefs.GetFloat("UIVolume", 1f)
        );
    }

    // LÍNEA RARA / COMPLEJA: Restauración de Mezclas mediante Deserialización Local ('RestoreAudio').
    // Recupera de los registros del disco duro del usuario ('PlayerPrefs') los niveles de decibelios o factores de volumen
    // originales para cada canal de sonido ("MusicVolume", "SFXVolume", "AmbienceVolume"). Esto deshace la atenuación de la pausa 
    // de manera exacta, devolviendo el juego a su balance acústico personalizado de forma inmediata al reanudar la acción.
    private void RestoreAudio()
    {
        if (AudioManager.Instance == null)
            return;

        AudioManager.Instance.SetMixerVolume(
            "MusicVolume",
            PlayerPrefs.GetFloat("MusicVolume", 1f)
        );

        AudioManager.Instance.SetMixerVolume(
            "SFXVolume",
            PlayerPrefs.GetFloat("SFXVolume", 1f)
        );

        AudioManager.Instance.SetMixerVolume(
            "UIVolume",
            PlayerPrefs.GetFloat("UIVolume", 1f)
        );

        AudioManager.Instance.SetMixerVolume(
            "AmbienceVolume",
            PlayerPrefs.GetFloat("AmbienceVolume", 1f)
        );
    }

    // Apaga instantáneamente el lienzo de la interfaz y normaliza los parámetros de simulación
    private void CloseInstant()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (pauseCanvas != null)
            pauseCanvas.enabled = false;
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Administrador del Menú de Pausa (PauseMenuUI). Su función principal dentro de la 
   arquitectura del HUD es interceptar la ejecución del juego mediante la escala de tiempo y controlar la atmósfera 
   acústica de la escena, proporcionando transiciones suaves y seguras entre el gameplay activo y los menús de gestión.

   Características clave:
   1. Control de Audio No Destructivo: Evita el uso de la propiedad drástica 'AudioListener.pause = true', que cortaría 
      en seco todo el sonido del motor. En su lugar, manipula subcanales específicos a través de un intermediario 
      ('AudioManager'), permitiendo un diseño de sonido inmersivo y profesional donde el entorno sigue reaccionando a la pausa.
   2. Persistencia y Respeto a Configuración del Usuario: Utiliza de manera consistente el sistema 'PlayerPrefs' 
      para la lectura de volúmenes al cerrar el menú. Esto previene un fallo clásico en sistemas de sonido: que tras salir 
      de la pausa, los volúmenes del juego se restablezcan al valor máximo por defecto ignorando las preferencias del jugador.
   3. Recarga Dinámica y Segura de Escenas: Prepara al motor antes de realizar cargas de nivel destructivas. 
      Al normalizar 'Time.timeScale' y restaurar los canales de audio de forma secuencial en 'RestartLevel' y 'GoToMainMenu', 
      elimina errores donde hilos de ejecución previos se queden congelados en bucles infinitos en la nueva escena cargada.
   ========================================================================================================
*/