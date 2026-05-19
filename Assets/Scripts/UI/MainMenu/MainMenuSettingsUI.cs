using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuSettingsUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider uiSlider;
    [SerializeField] private Slider ambienceSlider;
    [SerializeField] private Toggle fullscreenToggle;

    [Header("Navigation")]
    [SerializeField] private GameObject firstSelectedObject;

    // Recupera y aplica las preferencias guardadas por el usuario al cargar la pantalla de configuración
    private void Start()
    {
        // LÍNEA RARA / COMPLEJA: 'PlayerPrefs' (Persistencia de Datos básica en disco).
        // Accede al almacenamiento local del dispositivo para extraer configuraciones previas del jugador usando una "llave" de texto.
        // El segundo parámetro (ej. 1f) es un valor por defecto: si el juego se ejecuta por primera vez y la llave no existe, devolverá un volumen del 100%.
        // REGLA DE ORO con Booleanos: Como PlayerPrefs no soporta variables de tipo bool de forma nativa, se simula guardando un entero: 1 es Verdadero y 0 es Falso.
        float musicValue = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxValue = PlayerPrefs.GetFloat("SFXVolume", 1f);
        float uiValue = PlayerPrefs.GetFloat("UIVolume", 1f);
        float ambienceValue = PlayerPrefs.GetFloat("AmbienceVolume", 1f);
        bool fullscreen = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;


        // LÍNEA RARA / COMPLEJA: Inicialización silenciosa usando 'SetValueWithoutNotify' y 'SetIsOnWithoutNotify'.
        // Si usaras la propiedad estándar 'slider.value = musicValue', Unity interpretaría que el jugador movió físicamente el Slider.
        // Eso dispararía automáticamente el evento 'OnValueChanged', ejecutando lógicas sonoras innecesarias repetidas veces durante el primer frame de carga.
        // Estos métodos aplican el valor de forma visual inmediata "en silencio", evitando bucles lógicos o detonaciones accidentales de audio en el arranque.
        if (musicSlider != null)
            musicSlider.SetValueWithoutNotify(musicValue);

        if (sfxSlider != null)
            sfxSlider.SetValueWithoutNotify(sfxValue);

        if (uiSlider != null)
            uiSlider.SetValueWithoutNotify(uiValue);

        if (ambienceSlider != null)
            ambienceSlider.SetValueWithoutNotify(ambienceValue);


        if (fullscreenToggle != null)
            fullscreenToggle.SetIsOnWithoutNotify(fullscreen);

        // Envía los valores cargados hacia los mezcladores correspondientes a través del AudioManager
        SetMusicVolume(musicValue);
        SetSFXVolume(sfxValue);
        SetUIVolume(uiValue);
        SetAmbienceVolume(ambienceValue);
        SetFullscreen(fullscreen);
    }

    // Fuerza la selección del primer elemento de este panel para garantizar la compatibilidad inmediata con mandos (Gamepad)
    public void SelectFirstObject()
    {
        // Le indica al módulo de interacción global de Unity ('EventSystem.current') qué componente UI debe recibir el foco de entrada actual
        if (firstSelectedObject != null && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(firstSelectedObject);
    }

    public void SetMusicVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(value);
    }

    public void SetSFXVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(value);
    }

    public void SetUIVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetUIVolume(value);
    }

    public void SetAmbienceVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetAmbienceVolume(value);
    }

    // Gestiona el cambio de visualización de pantalla completa o modo ventana de la aplicación
    public void SetFullscreen(bool active)
    {
        // Operador Ternario: Define el modo exacto de despliegue según el estado del interruptor (Toggle)
        Screen.fullScreenMode = active
            ? FullScreenMode.ExclusiveFullScreen
            : FullScreenMode.Windowed;

        Screen.fullScreen = active;

        // Guarda el estado modificado de la pantalla en las preferencias locales convirtiendo el booleano en entero (1 o 0)
        PlayerPrefs.SetInt("Fullscreen", active ? 1 : 0);
        PlayerPrefs.Save(); // Escribe físicamente los cambios acumulados en el disco duro para evitar pérdidas si el juego se cierra abruptamente
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Controlador de la Interfaz de Ajustes del Menú Principal (MainMenuSettingsUI). Su principal 
   propósito es gestionar la persistencia, carga y aplicación de los parámetros de volumen (música, efectos, interfaz y ambiente) 
   y las opciones de pantalla del videojuego de manera centralizada.

   Características clave:
   1. Persistencia de Opciones de Usuario (PlayerPrefs): Asegura una experiencia de usuario correcta al recordar las configuraciones 
      del jugador. Almacena las preferencias en el sistema del dispositivo, de modo que cada vez que el usuario inicie una sesión, 
      el juego automáticamente adaptará los decibelios y el modo de ventana a su gusto personal.
   2. Inicialización Segura y Silenciosa: Utiliza funciones de UI avanzadas ('SetValueWithoutNotify') destinadas a mitigar 
      el comportamiento reactivo de Unity, asegurando que los gráficos del menú reflejen la realidad guardada sin detonar 
      falsos positivos o distorsiones sonoras simultáneas en la carga del menú.
   3. Soporte Nativo para Navegación Accesible: Expone el método público 'SelectFirstObject' que trabaja con el 'EventSystem', 
      imprescindible para que al abrir la pantalla de opciones mediante un Gamepad, el control se posicione de inmediato en 
      el primer Slider sin que el usuario experimente un bloqueo o tenga que recurrir forzosamente al puntero del mouse.
   ========================================================================================================
*/