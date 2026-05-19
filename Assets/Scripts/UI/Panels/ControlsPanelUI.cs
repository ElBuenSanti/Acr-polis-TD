using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ControlsPanelUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private CanvasGroup controlsGroup;
    [SerializeField] private CanvasGroup pauseGroup;

    [Header("UI")]
    [SerializeField] private Button firstSelectedButton;
    [SerializeField] private Button pauseFirstSelectedButton;

    [Header("Animation")]
    [SerializeField] private float fadeSpeed = 10f;

    [SerializeField] private UISoundPlayer uiSoundPlayer;

    private bool isOpen;

    // Inicializa las dependencias críticas y busca componentes en el mismo objeto o en la escena si faltan
    private void Awake()
    {
        if (controlsGroup == null)
            controlsGroup = GetComponent<CanvasGroup>();
        if (uiSoundPlayer == null)
            uiSoundPlayer = FindAnyObjectByType<UISoundPlayer>();
    }

    // Fuerza el apagado instantáneo de la interfaz al arrancar el nivel para evitar parpadeos visuales
    private void Start()
    {
        HideInstant();
    }

    // Modula la opacidad del grupo de controles frame a frame mediante una transición suavizada interpolada
    private void Update()
    {
        float targetAlpha = isOpen ? 1f : 0f;

        if (controlsGroup != null)
        {
            controlsGroup.alpha = Mathf.Lerp(
                 controlsGroup.alpha,
                 targetAlpha,
                 Time.unscaledDeltaTime * fadeSpeed
            );
        }
    }

    // LÍNEA RARA / COMPLEJA: Transición Cruzada Orientada a Estados (Cross-Fading/State Switching).
    // Abre el subpanel de configuración de controles apagando de inmediato la interactividad del menú de pausa anterior.
    // Transiciona la máquina de estados a 'GameState.Controls' e inyecta directamente el foco de navegación en el botón objetivo.
    public void Open()
    {

        if (uiSoundPlayer != null)
            uiSoundPlayer.PlayOpenPanel();

        isOpen = true;

        SetGroup(pauseGroup, false);
        SetGroup(controlsGroup, true);

        GameStateController.Instance.SetState(GameState.Controls);

        // LÍNEA RARA / COMPLEJA: Forzado de Enfoque de Navegación UI ('EventSystem.current.SetSelectedGameObject').
        // Vital para la accesibilidad con mandos (Gamepads) o teclado. Si simplemente activamos el panel visualmente,
        // el selector del sistema se queda "en el aire" o apuntando al botón del menú anterior. Al forzar el 'gameObject'
        // del botón al EventSystem, el juego sabe exactamente dónde colocar el cursor virtual o el resalte visual.
        if (firstSelectedButton != null)
            EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
    }

    // LÍNEA RARA / COMPLEJA: Retorno Controlado de Contexto de Navegación ('CloseToPause').
    // Realiza el proceso inverso de 'Open'. No cierra la interfaz por completo hacia el juego activo, sino que devuelve
    // el flujo al menú de pausa contenedor. Restaura el estado a 'GameState.Paused' y reasigna el foco del control
    // sobre el botón de origen en el menú de pausa para evitar que el jugador pierda el rastro de la selección.
    public void CloseToPause()
    {

        if (uiSoundPlayer != null)
            uiSoundPlayer.PlayClosePanel();

        isOpen = false;

        SetGroup(controlsGroup, false);
        SetGroup(pauseGroup, true);

        GameStateController.Instance.SetState(GameState.Paused);

        if (pauseFirstSelectedButton != null)
            EventSystem.current.SetSelectedGameObject(pauseFirstSelectedButton.gameObject);
    }

    // Restablece el CanvasGroup de controles de forma súbita bloqueando cualquier interacción física
    private void HideInstant()
    {
        isOpen = false;

        if (controlsGroup != null)
        {
            controlsGroup.alpha = 0f;
            controlsGroup.interactable = false;
            controlsGroup.blocksRaycasts = false;
        }
    }

    // LÍNEA RARA / COMPLEJA: Encapsulador de Estados de Bloque de Interfaz ('SetGroup').
    // Centraliza la manipulación de estados de un 'CanvasGroup'. En lugar de escribir tres líneas independientes
    // para alternar '.interactable', '.blocksRaycasts' y '.alpha' cada vez que queremos apagar o encender un menú,
    // este método procesa de manera genérica el componente recibido optimizando la limpieza y legibilidad del código.
    private void SetGroup(CanvasGroup group, bool active)
    {
        if (group == null)
            return;

        group.interactable = active;
        group.blocksRaycasts = active;

        if (active)
            group.alpha = 1f;
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Controlador del Panel de Configuración de Controles (ControlsPanelUI). Es un submenú 
   dependiente del sistema de pausa principal cuya función es gestionar las transiciones visuales, auditivas y lógicas 
   cuando el usuario decide consultar el esquema de botones o remapear los controles a mitad de una partida.

   Características clave:
   1. Sincronización Estricta de Enfoque (Gamepad Friendly): Está programado pensando en la compatibilidad nativa con 
      mandos de consola y teclado. A través de la manipulación explícita del 'EventSystem', garantiza que el foco de la 
      UI salte de manera impecable entre el botón que abrió el menú y el primer elemento interactivo de la nueva pantalla.
   2. Arquitectura de Transición en Cascada: Coordina de manera robusta la coexistencia de dos lienzos independientes 
      ('controlsGroup' y 'pauseGroup'). Su lógica condicional impide que ocurra un error común en desarrollo de UI: 
      que el jugador presione por accidente botones fantasmas del menú de fondo que quedaron invisibles pero activos.
   3. Automatización de Bloques con CanvasGroup: Utiliza los parámetros de renderizado y física del 'CanvasGroup' para 
      aislar subsecciones enteras del HUD. Esto previene la necesidad de activar y desactivar físicamente los GameObjects 
      completos, lo que permite que las animaciones por código se ejecuten fluidamente de forma asíncrona mediante interpolaciones.
   ========================================================================================================
*/