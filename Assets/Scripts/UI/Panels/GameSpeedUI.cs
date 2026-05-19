using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameSpeedUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI speedText;

    [SerializeField] private GameObject playIcon;
    [SerializeField] private GameObject x1Icon;
    [SerializeField] private GameObject x2Icon;

    private bool isSpeedTwo;

    // Actualiza de manera continua la coherencia gráfica de los iconos y textos del HUD
    private void Update()
    {
        RefreshVisual();
    }

    // LÍNEA RARA / COMPLEJA: Máquina de Estados de Doble Propósito (Context-Driven Toggle).
    // Este método actúa como un disparador híbrido. Si la oleada no ha comenzado, funciona como un botón de "Play" 
    // que arranca el combate e inicializa el tiempo. Si la oleada ya está en curso, muta su comportamiento para 
    // convertirse en un interruptor de velocidad que conmuta de forma cíclica la aceleración del motor entre x1 y x2.
    public void ToggleSpeed()
    {
        if (!WaveSpawner.Instance.IsWaveRunning())
        {
            WaveSpawner.Instance.StartWave();
            isSpeedTwo = false;
            Time.timeScale = 1f;
            return;
        }

        isSpeedTwo = !isSpeedTwo;
        Time.timeScale = isSpeedTwo ? 2f : 1f; // Aplica la aceleración del tiempo en el motor de físicas y simulación
    }

    // LÍNEA RARA / COMPLEJA: Matriz Matricial Condicional de Visibilidad de Capas de Interfaz (Layer-State Matrix).
    // Evalúa en cascada el estado del 'WaveSpawner' y el booleano 'isSpeedTwo' para encender o apagar de forma mutuamente 
    // excluyente los objetos del HUD. Esto previene un bug de renderizado común: que dos iconos se superpongan en la pantalla.
    private void RefreshVisual()
    {
        bool waveRunning = WaveSpawner.Instance.IsWaveRunning();

        if (playIcon != null)
            playIcon.SetActive(!waveRunning);

        if (x1Icon != null)
            x1Icon.SetActive(waveRunning && !isSpeedTwo);

        if (x2Icon != null)
            x2Icon.SetActive(waveRunning && isSpeedTwo);

        if (speedText != null)
        {
            if (!waveRunning)
                speedText.text = "";
            else
                speedText.text = isSpeedTwo ? "x2" : "x1";
        }
    }

    // LÍNEA RARA / COMPLEJA: Patrón Observador con Gestión de Memoria Segura (Event Subscription Lifecycle).
    // Vincula el método local 'ResetSpeed' al delegado/evento 'OnWaveEnded' del spawner global al activarse el objeto.
    // Esto desacopla las clases: el 'WaveSpawner' no necesita saber que existe este script de UI, simplemente grita 
    // al entorno "¡La oleada terminó!" y esta clase reacciona de forma automática reiniciando los valores del reloj.
    private void OnEnable()
    {
        if (WaveSpawner.Instance != null)
            WaveSpawner.Instance.OnWaveEnded += ResetSpeed;
    }

    // LÍNEA RARA / COMPLEJA: Desuscripción Estricta de Eventos (Memory Leak Prevention).
    // Regla de oro en Unity. Si destruyes o desactivas un objeto de interfaz pero olvidas desuscribirlo con el operador '-=',
    // la referencia en memoria se queda colgada (Garbage Collection Leak). El Spawner intentará invocar un objeto muerto, 
    // disparando una temida 'MissingReferenceException' que podría quebrar la lógica del flujo de la partida.
    private void OnDisable()
    {
        if (WaveSpawner.Instance != null)
            WaveSpawner.Instance.OnWaveEnded -= ResetSpeed;
    }

    // Restablece la velocidad del motor de Unity a su flujo normal (100% de velocidad estándar)
    private void ResetSpeed()
    {
        isSpeedTwo = false;
        Time.timeScale = 1f;
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Controlador de Velocidad de Juego y Disparador de Oleadas (GameSpeedUI). Es un 
   componente estratégico del HUD (común en juegos del género Tower Defense), diseñado para permitirle al jugador 
   iniciar el ataque enemigo o acelerar el flujo del tiempo del juego para evitar esperas monótonas en pantalla.

   Características clave:
   1. Control Dinámico de la Escala del Tiempo: Manipula de forma directa la variable global 'Time.timeScale'. 
      Esto acelera de manera nativa la cadencia de tiro de las torres, las animaciones y la velocidad de traslación 
      de las IA de los enemigos, permitiendo adelantar la acción sin romper las físicas de la escena.
   2. Gestión Limpia del Ciclo de Vida de Eventos: Al implementar de forma simétrica 'OnEnable' y 'OnDisable', 
      garantiza la estabilidad de la memoria RAM del juego, evitando fugas de objetos huérfanos que sigan escuchando 
      eventos del Administrador de Oleadas tras cambiar de nivel o cerrar la partida.
   3. Interfaz de Usuario Reactiva Automatizada: Centraliza el refresco visual en un método esclavo ('RefreshVisual') 
      que evalúa el estatus del gameplay en cada frame. Esto asegura que el HUD refleje de manera infalible la realidad 
      de la partida (ocultando multiplicadores de velocidad cuando el mapa está en calma y viceversa).
   ========================================================================================================
*/