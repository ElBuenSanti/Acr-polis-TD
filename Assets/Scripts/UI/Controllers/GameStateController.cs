using UnityEngine;

// LÍNEA RARA / COMPLEJA (Para principiantes): Un 'enum' (Enumerador) no es una clase, sino una estructura de datos especial.
// Funciona como una lista de "palabras clave" o etiquetas legibles para los humanos que representan estados lógicos.
// Internamente, Unity los procesa como números enteros (0, 1, 2...), pero usarlos evita tener que recordar números mágicos o strings.
public enum GameState
{
    MapIdle,
    ShopOpen,
    PlacingTower,
    RadialOpen,
    MovingTower,
    TownHallMenu,
    Paused,
    BlessingSelection,
    Settings,
    Controls,
    EndGame,
    MainMenu
}

public class GameStateController : MonoBehaviour
{
    // Patrón Singleton: Permite que cualquier clase consulte o altere el estado del juego mediante 'GameStateController.Instance'
    public static GameStateController Instance;

    // Almacena de manera pública el estado actual en el que se encuentra corriendo el bucle del juego
    public GameState currentState;

    private GridSelector selector;

    // Inicializa el Singleton y localiza los componentes necesarios en la escena
    void Awake()
    {
        Instance = this;
        selector = FindAnyObjectByType<GridSelector>();
    }

    // Arranca el flujo inicial del juego al cargar la escena
    void Start()
    {
        // Fuerza a que el juego inicie en modo de espera/exploración del mapa
        SetState(GameState.MapIdle);
    }

    // Método centralizado y crítico encargado de realizar la transición de un estado a otro
    public void SetState(GameState newState)
    {
        currentState = newState;
        // Imprime en la consola de Unity cada cambio de estado, lo cual es vital para tareas de depuración (debugging)
        Debug.Log("Estado: " + newState);
    }

    // Función de consulta rápida que devuelve un valor verdadero o falso (Booleano)
    public bool IsBusy()
    {
        // LÍNEA RARA / COMPLEJA: Devuelve el resultado de una evaluación lógica directa.
        // Si el estado actual es DIFERENTE a 'MapIdle' (es decir, el jugador está en la tienda, pausado, moviendo torres, etc.),
        // el método responderá con un 'true' (el sistema está ocupado). Si está libre en el mapa, responderá 'false'.
        return currentState != GameState.MapIdle;
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Cerebro Organizacional u Orquestador Central (GameStateController) del videojuego, 
   implementando una arquitectura básica de Máquina de Estados (State Pattern / FSM).

   Características clave:
   1. Control de Flujo unificado: Centraliza en un solo lugar todas las fases posibles por las que puede pasar 
      la experiencia de juego (estar libre en el mapa, comprando, ubicando una torre, pausado o en pantallas de fin de juego).
   2. Semáforo del Sistema (IsBusy): Proporciona un método de consulta global muy limpio. Otros componentes complejos 
      (como el sistema de IA de los enemigos o los controladores de inputs que vimos antes) pueden invocar a 'IsBusy()' 
      para saber si deben congelar sus acciones o si el jugador está interactuando con un menú prioritario.
   3. Arquitectura Robusta y Escalable: Al obligar a que todos los cambios de fase pasen estrictamente por el método 
      'SetState', se genera un cuello de botella controlado que facilita el rastreo de bugs en la consola y permite 
      añadir fácilmente código extra en el futuro cuando un estado específico inicie o termine.
   ========================================================================================================
*/