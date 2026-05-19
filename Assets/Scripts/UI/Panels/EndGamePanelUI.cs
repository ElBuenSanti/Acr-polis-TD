using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EndGamePanelUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button firstSelectedButton;

    [Header("Settings")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";
    [SerializeField] private float fadeSpeed = 8f;

    private bool isOpen;

    // Inicializa la referencia del CanvasGroup al despertar el script si no se asignó previamente
    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    // Asegura el ocultamiento inmediato del panel en el primer frame del juego para evitar destellos visuales
    private void Start()
    {
        HideInstant();
    }

    // Ejecuta la interpolación lineal asíncrona para desvanecer o mostrar el lienzo de fin de partida
    private void Update()
    {
        float targetAlpha = isOpen ? 1f : 0f;

        canvasGroup.alpha = Mathf.Lerp(
            canvasGroup.alpha,
            targetAlpha,
            Time.unscaledDeltaTime * fadeSpeed
        );
    }

    // LÍNEA RARA / COMPLEJA: Congelamiento del Reloj del Motor de Simulación ('Time.timeScale = 0f').
    // Detiene por completo el transcurso del tiempo del mundo del juego afectando directamente a 'Time.deltaTime'.
    // Esto paraliza los sistemas de partículas, las rutinas de movimiento de los enemigos, las animaciones estándar
    // y las físicas del motor (FixedUpdate), permitiendo que la interfaz se despliegue de forma estática sobre el estado final.
    public void Open()
    {
        Time.timeScale = 0f;
        isOpen = true;

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // Modifica el flujo global de la partida para indicar que el bucle principal de juego ha concluido
        GameStateController.Instance.SetState(GameState.EndGame);

        // Fuerza el enfoque del sistema de eventos en el botón principal para dar soporte nativo a mandos de consola
        if (firstSelectedButton != null)
            EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
    }

    // LÍNEA RARA / COMPLEJA: Restauración de Escala Temporal y Carga Síncrona de Escenas ('SceneManager.LoadScene').
    // Es un error crítico común cargar una escena nueva manteniendo 'Time.timeScale = 0f', ya que la nueva escena
    // despertaría completamente congelada y rota. Al restablecer la escala a '1f', se garantiza el correcto funcionamiento
    // de los métodos 'Start' y 'Update' del menú principal. Posteriormente, destruye la escena actual de forma síncrona.
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Restablece de forma fulminante los estados lógicos, de renderizado y colisión del CanvasGroup
    private void HideInstant()
    {
        isOpen = false;
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como la Pantalla de Fin de Partida (EndGamePanelUI). Es el componente encargado de congelar 
   la lógica de juego y desplegar la ventana definitiva de resultados (ya sea por victoria o derrota del jugador), 
   ofreciendo una vía de escape segura hacia el menú de inicio y gestionando la accesibilidad táctica del HUD.

   Características clave:
   1. Control Temporal Absoluto: Utiliza de manera efectiva la manipulación de 'Time.timeScale' para suspender las 
      fuerzas mecánicas del escenario. Al complementarse con 'Time.unscaledDeltaTime' en el método 'Update', asegura 
      que la animación de desvanecimiento del propio panel de la UI se ejecute con total fluidez, incluso cuando el 
      resto del universo del juego está completamente paralizado en el fondo.
   2. Prevención de Bugs de Persistencia Temporal: Incorpora la regla de seguridad higiénica en el desarrollo de videojuegos 
      de restablecer la velocidad del tiempo antes de invocar la carga de una nueva escena. Esto evita fallos críticos de 
      congelamiento lógico que podrían bloquear indefinidamente el bucle de inicialización del Menú Principal.
   3. Preparación de Entrada para Dispositivos (EventSystem): Garantiza que tras el final de la simulación, el jugador 
      pueda interactuar de forma inmediata con las opciones del panel sin importar su periférico de entrada. Al forzar la 
      selección del primer botón en el 'EventSystem', se elimina la necesidad de contar con un puntero de ratón físico, 
      habilitando la navegación fluida a través de las teclas de dirección o joysticks.
   ========================================================================================================
*/