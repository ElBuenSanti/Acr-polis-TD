using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ActionPanelUI : MonoBehaviour
{
    [Header("Move Button")]
    [SerializeField] private CanvasGroup moveButtonGroup;
    [SerializeField] private TextMeshProUGUI moveText;
    [SerializeField] private Image xIcon;

    [Header("Visual")]
    [SerializeField] private float activeAlpha = 1f;
    [SerializeField] private float inactiveAlpha = 0.35f;

    // LÍNEA RARA / COMPLEJA: Actualización en bucle continuo de la UI dentro del método 'Update'.
    // Si bien refrescar la interfaz en cada frame consume pequeños ciclos de procesamiento, en sistemas de HUD dinámicos 
    // garantiza que cualquier cambio de estado externo (como que un enemigo muera y termine la oleada) se refleje en pantalla de inmediato.
    private void Update()
    {
        RefreshMoveButton();
    }

    // LÍNEA RARA / COMPLEJA: Evaluación de estados booleanos combinados mediante cortocircuitos lógicos ('&&').
    // Revisa múltiples sistemas de juego en paralelo para determinar las reglas de negocio del HUD:
    // 1. 'hasSelection': Comprueba que el gestor de construcción exista y que el jugador tenga una torre seleccionada en el mapa.
    // 2. 'waveRunning': Verifica si hay una oleada de enemigos activa en este instante a través del generador de oleadas ('WaveSpawner').
    // 3. 'isMoving': Checa si la máquina de estados global se encuentra específicamente en la fase de recolocación ('MovingTower').
    private void RefreshMoveButton()
    {
        bool hasSelection = BuildingManager.Instance != null &&
                            BuildingManager.Instance.selectedConstruction != null;

        bool waveRunning = WaveSpawner.Instance != null &&
                           WaveSpawner.Instance.IsWaveRunning();

        bool isMoving = GameStateController.Instance.currentState == GameState.MovingTower;

        if (moveButtonGroup == null)
            return;

        // Condición 1: Si el jugador ya está moviendo una estructura, el botón debe brillar al 100% (activeAlpha)
        if (isMoving)
        {
            moveButtonGroup.alpha = activeAlpha;

            if (moveText != null)
                moveText.text = "Mover";

            return;
        }

        // Condición 2: El botón se activa si hay una estructura seleccionada Y ADEMÁS no hay una oleada de enemigos atacando.
        // REGLA DE ORO DEL GAMEPLAY: Esto evita una ventaja injusta (exploit), impidiendo que el jugador teletransporte o salve 
        // sus torres del peligro en medio del combate activo cuando los enemigos están marchando.
        if (hasSelection && !waveRunning)
        {
            moveButtonGroup.alpha = activeAlpha;

            if (moveText != null)
                moveText.text = "Mover";
        }
        // Condición 3: Si no se cumplen los requisitos anteriores, el botón se "apaga" visualmente usando opacidad translúcida (inactiveAlpha)
        else
        {
            moveButtonGroup.alpha = inactiveAlpha;

            if (moveText != null)
                moveText.text = "Mover";
        }
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Controlador del Panel de Acciones en el HUD (ActionPanelUI). Su principal propósito es 
   actualizar de forma reactiva el estado visual (disponibilidad) del botón encargado de trasladar o mover las estructuras 
   defensivas en el mapa de juego.

   Características clave:
   1. Validación del Estado del Tablero: Sincroniza la interfaz de usuario de forma directa con las reglas lógicas y de 
      diseño del juego. En lugar de dejar que el jugador presione botones que no harán nada, altera la opacidad del 
      componente 'CanvasGroup' para comunicarle de forma clara e intuitiva si la acción de movimiento está permitida o bloqueada.
   2. Restricción Estratégica de Combate: Implementa un candado lógico vital al evaluar la variable '!waveRunning'. Esto 
      aporta equilibrio al bucle de juego del género Tower Defense, forzando al usuario a planificar la posición de sus 
      defensas durante la fase de preparación y congelando los traslados en las fases de asedio enemigo.
   3. Arquitectura Desacoplada de Lectura: El script lee pasivamente los datos de los Singletons controladores del juego 
      ('BuildingManager', 'WaveSpawner', 'GameStateController'). Esto permite que la interfaz se adapte por sí sola a lo 
      que ocurre en el mapa sin necesidad de que los scripts de mecánicas internas tengan que enviar alertas manuales al HUD.
   ========================================================================================================
*/