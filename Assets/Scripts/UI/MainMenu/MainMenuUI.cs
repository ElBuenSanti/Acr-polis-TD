using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuUI : MonoBehaviour
{
    [Header("Scenes")]
    [SerializeField] private string gameSceneName = "GameScene";

    [Header("UI")]
    [SerializeField] private Button firstSelectedButton;

    // Configura e inicializa el entorno global y la interfaz del menú principal al cargar la escena
    private void Start()
    {
        // Fuerza el reinicio del tiempo por si venimos de una escena previa que se quedó pausada
        Time.timeScale = 1f;

        // LÍNEA RARA / COMPLEJA: 'AudioListener.pause = false'.
        // El 'AudioListener' actúa como los "oídos virtuales" de la cámara en Unity. 
        // Si en la escena de juego pausaste la partida y mutgaste todo el sonido usando 'AudioListener.pause = true', 
        // ese estado de silenciamiento total se queda guardado en la memoria global del motor de audio.
        // Forzarlo a 'false' aquí asegura que la música y los efectos del menú principal vuelvan a escucharse inmediatamente.
        AudioListener.pause = false;

        // Reproduce el tema musical principal del juego si el gestor de música está activo en la memoria
        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayMainMenuMusic();

        // Posiciona por código el foco del mando o teclado en el botón de inicio para habilitar la navegación inmediata
        if (firstSelectedButton != null && EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
    }

    // LÍNEA RARA / COMPLEJA: Carga de escenas mediante 'SceneManager.LoadScene'.
    // Corta por completo el ciclo de ejecución actual, destruye todos los objetos que no estén marcados como persistentes 
    // y carga en la memoria RAM el mapa o nivel que coincida exactamente con el nombre de texto indicado ('gameSceneName').
    // REGLA DE ORO: Asegura restablecer 'Time.timeScale = 1f' justo antes de la carga para que las corrutinas, físicas e hilos 
    // de la pantalla de carga o del nuevo nivel arranquen sin congelamientos temporales.
    public void Play()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    // Gestiona el cierre absoluto del programa de forma controlada
    public void Quit()
    {
        // LÍNEA RARA / COMPLEJA: 'Application.Quit()'.
        // Envía una instrucción directa al sistema operativo para cerrar el proceso y la ventana de la aplicación.
        // REGLA DE ORO: Este método solo funciona en las compilaciones finales ejecutables (.exe, .app, compilaciones de consola o móviles). 
        // Si lo presionas mientras estás probando el juego dentro del editor de Unity, el comando será completamente ignorado; 
        // no detendrá el modo Play del editor, por lo que es un comportamiento normal que no cierre el programa en desarrollo.
        Application.Quit();
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Controlador General del Menú Principal (MainMenuUI). Su propósito principal es gestionar 
   el punto de entrada del juego, coordinando la carga de niveles, la inicialización del motor de audio y la salida de la aplicación.

   Características clave:
   1. Restablecimiento del Estado Global (Limpieza de Persistencia): Funciona como un escudo contra bugs de transición. 
      Al limpiar y forzar las variables globales de tiempo ('timeScale') y de mutear audio ('AudioListener.pause'), evita 
      que los errores lógicos provocados al pausar o salir de una partida previa afecten la carga fresca del menú.
   2. Transición y Enrutamiento de Niveles: Expone métodos públicos limpios ('Play' y 'Quit') diseñados para ser vinculados 
      directamente a los eventos del componente 'Button' de Unity, lo que facilita la conexión desde el Inspector sin 
      escribir código intermedio.
   3. Inicialización Automática de Foco para Gamepads: En lugar de obligar al usuario a hacer clic con el mouse, delega al 
      'EventSystem' la activación del primer botón. Esto garantiza que la experiencia con mandos de consola o controles sea 
      completamente fluida desde el primer segundo en que el juego arranca.
   ========================================================================================================
*/