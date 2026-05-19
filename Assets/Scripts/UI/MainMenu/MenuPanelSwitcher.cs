using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuPanelSwitcher : MonoBehaviour
{
    [SerializeField] private CanvasGroup mainGroup;
    [SerializeField] private CanvasGroup settingsGroup;
    [SerializeField] private CanvasGroup controlsGroup;

    [SerializeField] private Button mainFirstButton;
    [SerializeField] private Button settingsFirstButton;
    [SerializeField] private Button controlsFirstButton;

    [SerializeField] private float fadeSpeed = 10f;

    private CanvasGroup activeGroup;

    // Configura el estado inicial instantáneo de los paneles en el primer frame de la escena
    private void Start()
    {
        ShowMainInstant();
    }

    // Procesa de forma continua el desvanecimiento cruzado de todos los menús de manera independiente
    private void Update()
    {
        FadeGroup(mainGroup);
        FadeGroup(settingsGroup);
        FadeGroup(controlsGroup);
    }

    // Métodos públicos diseñados para ser enlazados a eventos de botones de la UI (On Click)
    public void ShowMain()
    {
        SetActiveGroup(mainGroup, mainFirstButton);
    }

    public void ShowSettings()
    {
        SetActiveGroup(settingsGroup, settingsFirstButton);
    }

    public void ShowControls()
    {
        SetActiveGroup(controlsGroup, controlsFirstButton);
    }

    // LÍNEA RARA / COMPLEJA: Inicializador duro de interfaces en el arranque.
    // Configura las opacidades ('alpha') de golpe (1f para el principal, 0f para los submenús) sin animación.
    // Esto previene que durante el primer frame del juego el jugador perciba un parpadeo visual o vea los menús superpuestos.
    private void ShowMainInstant()
    {
        activeGroup = mainGroup;

        SetGroupInstant(mainGroup, true);
        SetGroupInstant(settingsGroup, false);
        SetGroupInstant(controlsGroup, false);

        if (mainFirstButton != null)
            EventSystem.current.SetSelectedGameObject(mainFirstButton.gameObject);
    }

    // LÍNEA RARA / COMPLEJA: Gestión selectiva de foco e interactividad por comparaciones booleanas directas.
    // Al evaluar expresiones lógicas como 'group == mainGroup', activa o desactiva de manera fulminante los bloques de colisión de la UI.
    // Esto asegura que, mientras un panel se está desvaneciendo visualmente (efecto alpha), sus botones internos queden bloqueados 
    // de inmediato para el control, impidiendo que el jugador presione accidentalmente opciones invisibles u ocultas de fondo.
    private void SetActiveGroup(CanvasGroup group, Button firstButton)
    {
        activeGroup = group;

        SetInteractable(mainGroup, group == mainGroup);
        SetInteractable(settingsGroup, group == settingsGroup);
        SetInteractable(controlsGroup, group == controlsGroup);

        if (firstButton != null)
            EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
    }

    // Aplica una interpolación matemática suave e independiente de la pausa física para cambiar la opacidad
    private void FadeGroup(CanvasGroup group)
    {
        if (group == null)
            return;

        float target = group == activeGroup ? 1f : 0f;
        group.alpha = Mathf.Lerp(group.alpha, target, Time.unscaledDeltaTime * fadeSpeed);
    }

    // Fuerza los valores visuales y lógicos de un panel de interfaz sin transiciones intermedias
    private void SetGroupInstant(CanvasGroup group, bool active)
    {
        if (group == null)
            return;

        group.alpha = active ? 1f : 0f;
        SetInteractable(group, active);
    }

    // LÍNEA RARA / COMPLEJA: Bloqueo de hilos de Raycasts y Entradas mediante propiedades de 'CanvasGroup'.
    // Modificar 'interactable' a false apaga el foco de navegación para mandos y teclados.
    // Modificar 'blocksRaycasts' a false le ordena al sistema gráfico ignorar los clics del mouse o toques en pantalla.
    // REGLA DE ORO: Es drásticamente más eficiente y óptimo apagar estas dos propiedades en un CanvasGroup que usar 
    // 'gameObject.SetActive(false)', ya que desactivar objetos UI de golpe fuerza al motor gráfico a recalcular y reconstruir 
    // toda la geometría de la interfaz (Canvas Rebuild), provocando caídas de frames o micro-tirones (stuttering).
    private void SetInteractable(CanvasGroup group, bool active)
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
   Este script actúa como el Conmutador Avanzado de Paneles de Interfaz (MenuPanelSwitcher). Su función principal es 
   orquestar el flujo de navegación entre las distintas pantallas del menú (Menú Principal, Opciones y Controles), 
   garantizando transiciones de desvanecimiento ("fade in/out") cruzadas fluidas y elegantes.

   Características clave:
   1. Navegación Profesional y Segura (Anti-Bugs): Resuelve el problema clásico donde el jugador puede clicar un botón 
      fantasma de un menú que se está ocultando. Al separar la lógica visual (manejada de forma suave en el 'Update') de 
      la lógica interactiva (bloqueada en el acto al cambiar de panel), se logra una navegación robusta y a prueba de errores.
   2. Rendimiento Ultra-Optimizado (Evita Canvas Rebuilds): Al mantener todos los paneles encendidos en la jerarquía 
      y limitarse a modular sus componentes 'CanvasGroup', el motor gráfico maneja todo a nivel de tarjeta de video (GPU), 
      ahorrando tiempo valioso en el procesador principal (CPU).
   3. Automatización de Selección para Controles: Al integrar la llamada a 'EventSystem.current.SetSelectedGameObject' 
      durante el intercambio de pantallas, redirige el cursor del Gamepad al botón de arranque nativo de cada panel. El usuario 
      puede navegar fluidamente con la cruceta entre las opciones y los controles sin perder jamás el foco del control.
   ========================================================================================================
*/