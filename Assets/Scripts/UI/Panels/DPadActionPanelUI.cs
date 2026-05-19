using UnityEngine;

public class DPadActionPanelUI : MonoBehaviour
{
    [Header("Highlights")]
    [SerializeField] private DPadActionHighlightUI upHighlight;
    [SerializeField] private DPadActionHighlightUI downHighlight;
    [SerializeField] private DPadActionHighlightUI leftHighlight;
    [SerializeField] private DPadActionHighlightUI rightHighlight;

    [Header("Action Groups")]
    [SerializeField] private CanvasGroup upGroup;
    [SerializeField] private CanvasGroup downGroup;
    [SerializeField] private CanvasGroup leftGroup;
    [SerializeField] private CanvasGroup rightGroup;

    [Header("Settings")]
    [SerializeField] private float highlightTime = 0.18f;
    [SerializeField] private float activeAlpha = 1f;
    [SerializeField] private float disabledAlpha = 0.35f;

    private float upTimer;
    private float downTimer;
    private float leftTimer;
    private float rightTimer;

    // Ejecuta el ciclo continuo de decremento de temporizadores y refresco de opacidades en el HUD
    private void Update()
    {
        UpdateHighlight(ref upTimer, upHighlight);
        UpdateHighlight(ref downTimer, downHighlight);
        UpdateHighlight(ref leftTimer, leftHighlight);
        UpdateHighlight(ref rightTimer, rightHighlight);

        RefreshAvailability();
    }

    // Activa el destello visual superior y restablece su temporizador de apagado automático
    public void FlashUp()
    {
        upTimer = highlightTime;
        SetActive(upHighlight, true);
    }

    // Activa el destello visual inferior y restablece su temporizador de apagado automático
    public void FlashDown()
    {
        downTimer = highlightTime;
        SetActive(downHighlight, true);
    }

    // Activa el destello visual izquierdo y restablece su temporizador de apagado automático
    public void FlashLeft()
    {
        leftTimer = highlightTime;
        SetActive(leftHighlight, true);
    }

    // Activa el destello visual derecho y restablece su temporizador de apagado automático
    public void FlashRight()
    {
        rightTimer = highlightTime;
        SetActive(rightHighlight, true);
    }

    // LÍNEA RARA / COMPLEJA: Evaluación condicional cruzada y encadenamiento de 'FindObjectOfType' en Update.
    // Aunque 'FindObjectOfType' es una operación costosa para el procesador (ya que examina todos los objetos de la escena),
    // aquí se utiliza para determinar en tiempo real si la casilla actual apuntada por el cursor táctico ('currentTile') 
    // contiene una estructura activa ('IsOccupied'). Esto condiciona de forma dinámica la disponibilidad de la acción izquierda.
    private void RefreshAvailability()
    {
        bool waveRunning = WaveSpawner.Instance != null && WaveSpawner.Instance.IsWaveRunning();
        bool mapIdle = GameStateController.Instance.currentState == GameState.MapIdle;

        bool canUseGlobal = mapIdle && !waveRunning;

        SetGroup(upGroup, canUseGlobal);
        SetGroup(downGroup, canUseGlobal);
        SetGroup(rightGroup, canUseGlobal);

        //bool hasSelectedStructure =
        //    BuildingManager.Instance != null &&
        //    BuildingManager.Instance.selectedConstruction != null;

        bool hasSelectedStructure =
            FindObjectOfType<GridSelector>() != null &&
            FindObjectOfType<GridSelector>().currentTile != null &&
            FindObjectOfType<GridSelector>().currentTile.IsOccupied;

        SetGroup(leftGroup, canUseGlobal && hasSelectedStructure);
    }

    // LÍNEA RARA / COMPLEJA: Paso de parámetros por referencia mediante la palabra clave 'ref'.
    // Por defecto, en C#, los tipos de datos primitivos como 'float' se pasan "por valor" (se crea una copia local dentro del método).
    // Al anteponer 'ref', le pasamos al método la dirección de memoria exacta de la variable original ('upTimer', 'downTimer', etc.).
    // Esto permite que una única función genérica modifique directamente el valor de los cuatro temporizadores individuales de la clase,
    // reduciendo drásticamente la duplicación de código redundante.
    private void UpdateHighlight(ref float timer, DPadActionHighlightUI highlight)
    {
        if (timer <= 0f)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
            SetActive(highlight, false);
    }

    // Invoca de forma segura el estado de activación del componente de resplandor
    private void SetActive(DPadActionHighlightUI highlight, bool active)
    {
        if (highlight != null)
            highlight.SetActive(active);
    }

    // Altera la opacidad del CanvasGroup basándose en el estado de validez de las reglas de juego
    private void SetGroup(CanvasGroup group, bool active)
    {
        if (group == null)
            return;

        group.alpha = active ? activeAlpha : disabledAlpha;
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Panel de Acciones Vinculadas a la Cruceta (DPadActionPanelUI). Su propósito principal 
   dentro del HUD es interactuar con el D-Pad (Cruceta de un mando o teclado), ofreciendo retroalimentación visual inmediata 
   en forma de destellos de corta duración ('Flash') y controlando qué comandos están disponibles según la fase del juego.

   Características clave:
   1. Temporizadores por Referencia: Centraliza el algoritmo de cuenta regresiva para el desvanecimiento de las luces 
      de la interfaz. Al utilizar la palabra clave 'ref', optimiza el mantenimiento del script al delegar el comportamiento 
      de actualización en un único bloque reutilizable en lugar de replicar cuatro lógicas idénticas independientes.
   2. Interfaz Reactiva al Entorno Lógico: Bloquea o desbloquea los comandos de la cruceta según el contexto actual 
      de la partida ('RefreshAvailability'). Por ejemplo, si los enemigos están atacando u otra ventana está abierta, 
      el panel atenúa los iconos disminuyendo su opacidad ('disabledAlpha') indicando al jugador que sus comandos no surtirán efecto.
   3. Validación de Casillas de Rejilla (Grid-Based UI): Muestra una dependencia inteligente con el sistema de casillas. 
      La acción de la dirección izquierda se condiciona directamente a que el selector del mapa ('GridSelector') esté 
      posicionado sobre un cuadrante ocupado, lo que resulta sumamente útil para activar menús contextuales de demolición, 
      reparación o inspección de torres específicas de forma automatizada.
   ========================================================================================================
*/