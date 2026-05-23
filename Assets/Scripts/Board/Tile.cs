//using System.Collections.Generic;
//using UnityEngine;

//public class Tile : MonoBehaviour
//{
//    private Renderer tileColor;
//    private bool isOccupied = false;
//    public bool IsOccupied => isOccupied;

//    [SerializeField] private Color originalTileColor;

//    public List<Tile> sameRowNeighbors = new List<Tile>();
//    public List<Tile> upperRowNeighbors = new List<Tile>();
//    public static List<Tile> AllTiles = new List<Tile>();

//    void Awake()
//    {
//        tileColor = GetComponent<Renderer>();
//        originalTileColor = tileColor.material.color;
//        AllTiles.Add(this);
//    }

//    //Visual Change
//    public void SetColor(Color color)
//    {
//        tileColor.material.color = color;
//        originalTileColor = color;
//    }

//    public void SetTempColor(Color color)
//    {
//        if (isOccupied)
//            return;

//        tileColor.material.color = color;
//    }

//    public void ResetSelf()
//    {
//        if (isOccupied)
//            return;

//        tileColor.material.color = originalTileColor;
//    }

//    //Hover
//    void OnMouseEnter() //HOOVER
//    {
//        if (!WaveSpawner.Instance.IsWaveRunning())
//        {
//            BuildingManager.Instance.SetHoveredTile(this);
//        }
//    }

//    void OnMouseExit() //salir del hoover
//    {
//        if (!WaveSpawner.Instance.IsWaveRunning())
//        {
//            BuildingManager.Instance.ClearHover(); 
//        }
//    }

//    void OnMouseDown()
//    {
//        if (WaveSpawner.Instance.IsWaveRunning())
//        {
//            return;
//        }

//        if (isOccupied)
//            return;

//        BuildingManager.Instance.PlaceBuilding(this);
//    }

//    //Tile Status
//    public void SetOccupied(bool value)
//    {
//        isOccupied = value;

//        if (isOccupied)
//        {
//            tileColor.material.color = Color.green;
//        }
//        else
//        {
//            ResetSelf();
//        }
//    }
//}



using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private Renderer tileColor;
    private bool isOccupied = false;
    public bool IsOccupied => isOccupied;

    [SerializeField] private Color originalTileColor;

    public List<Tile> sameRowNeighbors = new List<Tile>();
    public List<Tile> upperRowNeighbors = new List<Tile>();

    // Used only by UI/gamepad selector
    public List<Tile> allNeighbors = new List<Tile>();

    public static List<Tile> AllTiles = new List<Tile>();

    // LÍNEA RARA / COMPLEJA: Inicialización de Caché de Renderizado e Inserción Defensiva en el Índice Global de Casillas ('Awake').
    // Cachea el componente gráfico 'Renderer' para evitar la costosa consulta repetida en memoria de materiales. A diferencia de la versión 
    // anterior que inyectaba la instancia a ciegas, implementa una compuerta defensiva mediante 'if (!AllTiles.Contains(this))'. Esta verificación 
    // de contención evita la duplicación de punteros idénticos dentro de la lista estática global 'AllTiles' durante cargas de escena inestables o 
    // reinicios procedimentales del tablero, garantizando que cada celda física del mapa posea un único índice de indexación.
    void Awake()
    {
        tileColor = GetComponent<Renderer>();
        originalTileColor = tileColor.material.color;

        if (!AllTiles.Contains(this))
            AllTiles.Add(this);
    }

    // LÍNEA RARA / COMPLEJA: Remoción Asertiva del Grafo Estático en el Ciclo de Desmantelamiento de Memoria ('OnDestroy').
    // Mensaje nativo invocado de forma automática por el motor cuando el tablero se destruye, se cambia de nivel o la casilla se elimina. 
    // Evalúa si la lista global contiene la referencia actual mediante 'AllTiles.Contains(this)' y ejecuta de forma atómica su remoción 
    // ('AllTiles.Remove'). Esto mitiga las fugas de memoria y elimina por completo el riesgo de referencias fantasmas ("NullReferenceGhosting"), 
    // donde scripts externos de posicionamiento o interfaces intentan interactuar con coordenadas de celdas que ya no existen físicamente.
    void OnDestroy()
    {
        if (AllTiles.Contains(this))
            AllTiles.Remove(this);
    }

    // Sobreescribe permanentemente el color de la instancia y actualiza el búfer de respaldo original
    public void SetColor(Color color)
    {
        tileColor.material.color = color;
        originalTileColor = color;
    }

    // Aplica una tonalidad transitoria (ej. previsualización de construcción) bloqueando el cambio si la celda ya tiene una estructura armada
    public void SetTempColor(Color color)
    {
        if (isOccupied)
            return;

        tileColor.material.color = color;
    }

    // Devuelve el material del renderizador a su estado original si la casilla no está reteniendo un edificio activo
    public void ResetSelf()
    {
        if (isOccupied)
            return;

        tileColor.material.color = originalTileColor;
    }

    // Registra la casilla actual en el gestor de colocación al posicionar el cursor sobre su volumen, siempre que no haya combates activos
    void OnMouseEnter()
    {
        if (!WaveSpawner.Instance.IsWaveRunning())
        {
            BuildingManager.Instance.SetHoveredTile(this);
        }
    }

    // Notifica la retirada del cursor para limpiar las mallas de previsualización flotantes en el gestor de construcción
    void OnMouseExit()
    {
        if (!WaveSpawner.Instance.IsWaveRunning())
        {
            BuildingManager.Instance.ClearHover();
        }
    }

    // Coordina la colocación física de estructuras enviando esta coordenada al BuildingManager tras validar los estados de juego y ocupación
    void OnMouseDown()
    {
        if (WaveSpawner.Instance.IsWaveRunning())
            return;

        if (isOccupied)
            return;

        BuildingManager.Instance.PlaceBuilding(this);
    }

    // Mutador de estado de ocupación que modifica dinámicamente el canal visual del material para retroalimentar al jugador
    public void SetOccupied(bool value)
    {
        isOccupied = value;

        if (isOccupied)
        {
            tileColor.material.color = Color.green;
        }
        else
        {
            ResetSelf();
        }
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como la **Unidad Celular y Nodo del Grafo de Posicionamiento del Tablero** (`Tile`). 
   Su responsabilidad principal dentro de la arquitectura de este Tower Defense o juego de estrategia táctica es representar 
   individualmente cada cuadrante o parcela del mapa donde el usuario puede interactuar, recibir señales de apuntado del ratón o gamepad, 
   y albergar la cimentación de las construcciones del juego.

   Características arquitectónicas clave:
   1. Grafo de Adyacencia Multidireccional: La versión de producción expande significativamente el soporte de vecindad introduciendo 
      `allNeighbors`. Mientras que `sameRowNeighbors` y `upperRowNeighbors` resuelven la lógica algorítmica de soporte estructural 
      o distribución de energía, la nueva colección unificada permite que un selector de UI o mando de consola salte limpiamente de 
      una casilla a otra colindante en cualquier dirección física.
   2. Blindaje de Ciclo de Vida en Colecciones Estáticas: Al acoplar la validación de contención en `Awake` con la limpieza forzada 
      en `OnDestroy`, se transforma el sistema en un modelo robusto. Impide que se procesen llamadas a objetos liberados por el recolector 
      de basura de C# (Garbage Collector), manteniendo la lista estática `AllTiles` 100% íntegra en las transiciones de juego.
   3. Intercepciones de Estado por Fases (Phase-Gated Interactions): Los métodos nativos de colisión (`OnMouseEnter`, `OnMouseDown`, etc.) 
      están vinculados rígidamente al estado del `WaveSpawner`. Al validar `!IsWaveRunning()`, el script actúa como un cortafuegos físico, 
      impidiendo al usuario colocar estructuras, alterar colores o invocar al `BuildingManager` mientras la oleada de enemigos está activa.
   ========================================================================================================
*/