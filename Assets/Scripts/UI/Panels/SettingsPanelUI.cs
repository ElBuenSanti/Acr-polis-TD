using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SettingsPanelUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private CanvasGroup settingsGroup;
    [SerializeField] private CanvasGroup pauseGroup;

    [Header("UI")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider uiSlider;
    [SerializeField] private Slider ambienceSlider;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private GameObject firstSelectedObject;
    [SerializeField] private Button pauseFirstSelectedButton;

    [Header("Animation")]
    [SerializeField] private float fadeSpeed = 10f;

    [Header("Sound")]
    [SerializeField] private UISoundPlayer uiSoundPlayer;

    private bool isOpen;

    // Busca e inyecta las referencias nativas o globales necesarias al despertar el objeto
    private void Awake()
    {
        if (settingsGroup == null)
            settingsGroup = GetComponent<CanvasGroup>();

        if (uiSoundPlayer == null)
            uiSoundPlayer = FindAnyObjectByType<UISoundPlayer>();
    }

    // LÍNEA RARA / COMPLEJA: Inicialización Silenciosa Desacoplada de Eventos de Interfaz ('SetValueWithoutNotify').
    // Carga los valores de volumen y pantalla desde 'PlayerPrefs' y los inyecta en los sliders y toggles usando funciones 
    // "WithoutNotify". Esto es fundamental para evitar un bug de bucle infinito: si usáramos '.value = x', se dispararía el 
    // evento 'onValueChanged' de la UI de Unity, llamando prematuramente a los métodos 'SetMusicVolume' y sobreescribiendo 
    // la base de datos de audio mientras el juego apenas se está inicializando en su primer frame.
    private void Start()
    {
        HideInstant();

        float musicValue = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfxValue = PlayerPrefs.GetFloat("SFXVolume", 1f);
        float uiValue = PlayerPrefs.GetFloat("UIVolume", 1f);
        float ambienceValue = PlayerPrefs.GetFloat("AmbienceVolume", 1f);
        bool fullscreen = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;

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

        SetMusicVolume(musicValue);
        SetSFXVolume(sfxValue);
        SetUIVolume(uiValue);
        SetAmbienceVolume(ambienceValue);
        SetFullscreen(fullscreen);
    }

    // LÍNEA RARA / COMPLEJA: Interpolación por Aproximación No Lineal de Opacidad en Tiempo Desacoplado ('Mathf.MoveTowards').
    // A diferencia de 'Mathf.Lerp' (que reduce su velocidad de forma exponencial conforme se acerca al objetivo), 'Mathf.MoveTowards' 
    // desplaza el valor de forma lineal constante garantizando que alcance exactamente el 'targetAlpha' sin dejar decimales infinitos. 
    // Multiplica la velocidad por 'Time.unscaledDeltaTime' para que el fundido del panel de opciones mantenga su cadencia suave 
    // e idéntica incluso si el juego de fondo se encuentra totalmente congelado detrás de un menú de pausa.
    private void Update()
    {
        float targetAlpha = isOpen ? 1f : 0f;

        if (settingsGroup != null)
        {
            settingsGroup.alpha = Mathf.MoveTowards(
                settingsGroup.alpha,
                targetAlpha,
                Time.unscaledDeltaTime * fadeSpeed
            );
        }
    }

    // Muta el estado de juego hacia la configuración y transiciona la visualización e interactividad entre los lienzos
    public void Open()
    {
        if (uiSoundPlayer != null)
            uiSoundPlayer.PlayOpenPanel();

        GameStateController.Instance.SetState(GameState.Settings);

        isOpen = true;

        SetGroup(pauseGroup, false);
        SetGroup(settingsGroup, true);

        if (firstSelectedObject != null && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(firstSelectedObject);
    }

    // Devuelve el flujo lógico de la interfaz al menú de pausa previo restaurando el enfoque del periférico de entrada
    public void CloseToPause()
    {
        if (uiSoundPlayer != null)
            uiSoundPlayer.PlayClosePanel();

        GameStateController.Instance.SetState(GameState.Paused);

        isOpen = false;

        SetGroup(settingsGroup, false);
        SetGroup(pauseGroup, true);

        if (pauseFirstSelectedButton != null && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(pauseFirstSelectedButton.gameObject);
    }

    // Envía la solicitud de modificación de volumen de la música al subsistema de audio central
    public void SetMusicVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(value);
    }

    // Envía la solicitud de modificación de volumen de los efectos especiales al subsistema de audio central
    public void SetSFXVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(value);
    }

    // Envía la solicitud de modificación de volumen de los sonidos de interfaz al subsistema de audio central
    public void SetUIVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetUIVolume(value);
    }

    // Envía la solicitud de modificación de volumen del sonido ambiental al subsistema de audio central
    public void SetAmbienceVolume(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetAmbienceVolume(value);
    }

    // LÍNEA RARA / COMPLEJA: Modificación Mutativa de Despliegue de Ventana y Sincronización en Disco ('Screen.fullScreenMode').
    // Altera simultáneamente las propiedades de visualización del hardware ('fullScreenMode' y 'fullScreen') discriminando 
    // de manera estricta entre un modo de pantalla completa exclusiva de baja latencia o una ventana clásica de sistema operativo. 
    // Acto seguido, serializa la preferencia en formato binario de clave-valor usando 'PlayerPrefs.SetInt' y fuerza la escritura 
    // inmediata en el almacenamiento físico del dispositivo mediante '.Save()', evitando pérdidas de datos si el proceso colapsa.
    public void SetFullscreen(bool active)
    {
        Screen.fullScreenMode = active
            ? FullScreenMode.ExclusiveFullScreen
            : FullScreenMode.Windowed;

        Screen.fullScreen = active;

        PlayerPrefs.SetInt("Fullscreen", active ? 1 : 0);
        PlayerPrefs.Save();
    }

    // Apaga fulminantemente la transparencia, bloqueos e interacciones del CanvasGroup de configuraciones
    private void HideInstant()
    {
        isOpen = false;

        if (settingsGroup != null)
        {
            settingsGroup.alpha = 0f;
            settingsGroup.interactable = false;
            settingsGroup.blocksRaycasts = false;
        }
    }

    // Modifica de manera masiva los conmutadores lógicos de colisión y puntero de una capa de interfaz determinada
    private void SetGroup(CanvasGroup group, bool active)
    {
        if (group == null)
            return;

        group.interactable = active;
        group.blocksRaycasts = active;
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Panel de Gestión de Opciones y Ajustes de Usuario (SettingsPanelUI). Es un componente 
   estructural de la UI encargado de enlazar los elementos interactivos del menú de configuración (sliders de volumen, 
   toggles de pantalla) con las API del motor de Unity ('Screen') y los managers de persistencia locales, permitiendo 
   guardar las preferencias del jugador y alternar limpiamente con otros paneles como el menú de pausa.

   Características clave:
   1. Prevención de Bucles de Notificación: Utiliza técnicas de inyección limpia mediante 'SetValueWithoutNotify' 
      durante su arranque. Esto aísla la carga de datos del disco duro de las funciones callback, evitando que el panel 
      interprete el establecimiento inicial de los valores como si el usuario estuviera manipulando la UI manualmente.
   2. Transición Robusta en Estado de Pausa: Implementa lógica basada en 'Time.unscaledDeltaTime' para procesar su 
      desvanecimiento con independencia absoluta del reloj del juego. Esto garantiza que la interfaz responda con total 
      suavidad, velocidad y dinamismo cinemático sin importar si las mecánicas tridimensionales están detenidas en el fondo.
   3. Persistencia Segura y Control de Hardware: Centraliza la manipulación del hardware de visualización y el sistema 
      de almacenamiento 'PlayerPrefs'. Al invocar explícitamente el método 'Save()', mitiga el riesgo de corrupción o pérdida 
      de configuraciones del sistema si el usuario decide forzar el cierre de la aplicación de manera imprevista.
   ========================================================================================================
*/