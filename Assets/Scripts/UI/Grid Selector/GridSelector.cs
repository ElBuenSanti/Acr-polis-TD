using System.Collections;
using UnityEngine;

public class GridSelector : MonoBehaviour
{
    [Header("Selection")]
    public Tile currentTile;

    [Header("Movement")]
    [SerializeField] private float moveCooldown = 0.28f;
    [SerializeField] private float inputDeadZone = 0.75f;

    [Header("Camera Reference")]
    [SerializeField] private Camera mainCamera;

    private float lastMoveTime;

    // LÍNEA RARA / COMPLEJA: Inicializador asíncrono usando una Corrutina ('IEnumerator').
    // A diferencia de un Start tradicional, permite usar 'yield return' para pausar la ejecución del script.
    // Detiene el flujo de este componente hasta que el listado global 'Tile.AllTiles' deje de ser nulo y contenga datos.
    // Esto previene errores de orden de carga si los mosaicos del mapa tardan en instanciarse o generarse.
    private IEnumerator Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        yield return new WaitUntil(() => Tile.AllTiles != null && Tile.AllTiles.Count > 0);

        currentTile = FindFirstTile();
        UpdateHover();

        Debug.Log("GridSelector started on tile: " + currentTile.name);
    }

    // Gestiona el desplazamiento del cursor entre las celdas del tablero físico
    public void Move(Vector2 input)
    {
        if (currentTile == null)
            return;

        if (Time.time < lastMoveTime + moveCooldown)
            return;

        if (input.magnitude < inputDeadZone)
            return;

        Tile nextTile = GetBestNeighbor(input);

        if (nextTile == null)
            return;

        currentTile = nextTile;
        lastMoveTime = Time.time;

        UpdateHover();

        Debug.Log("Moved to tile: " + currentTile.name);
    }

    private Tile FindFirstTile()
    {
        return Tile.AllTiles[0];
    }

    // LÍNEA RARA / COMPLEJA: Algoritmo de selección direccional por Producto Punto (Vector3.Dot).
    // Evalúa matemáticamente cuál de los vecinos ('allNeighbors') se alinea mejor con la dirección del joystick.
    // Procesa la dirección física del vecino, descarta el eje de altura (y = 0f) y calcula el coseno del ángulo.
    // Si el puntaje supera el umbral mínimo (0.45f) y es el más cercano a 1f (alineación perfecta), lo selecciona como el mejor candidato.
    private Tile GetBestNeighbor(Vector2 input)
    {
        Vector3 desiredDirection = GetCameraRelativeDirection(input);

        Tile bestTile = null;
        float bestScore = 0.45f;

        foreach (Tile tile in currentTile.allNeighbors)
        {
            if (tile == null)
                continue;

            Vector3 tileDirection = tile.transform.position - currentTile.transform.position;
            tileDirection.y = 0f;
            tileDirection.Normalize();

            float score = Vector3.Dot(desiredDirection, tileDirection);

            if (score > bestScore)
            {
                bestScore = score;
                bestTile = tile;
            }
        }

        return bestTile;
    }

    // LÍNEA RARA / COMPLEJA: Proyección espacial del Input basada en la orientación de la Cámara.
    // Si la cámara del juego está rotada (por ejemplo, en perspectiva isométrica), presionar "Arriba" en el stick 
    // debe mover el selector hacia adelante según lo que ve el ojo de la cámara, no según los ejes globales del mundo.
    // Multiplica los vectores frontales y laterales de la cámara, anula su inclinación vertical en 'y' y normaliza el resultado.
    private Vector3 GetCameraRelativeDirection(Vector2 input)
    {
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 direction = cameraRight * input.x + cameraForward * input.y;
        direction.Normalize();

        return direction;
    }

    // Actualiza los efectos visuales de selección o previsualización en el 'BuildingManager'
    private void UpdateHover()
    {
        if (currentTile == null)
            return;

        // Solo proyecta el recuadro de selección si el jugador se encuentra activamente edificando o trasladando defensas
        if (GameStateController.Instance.currentState == GameState.PlacingTower ||
            GameStateController.Instance.currentState == GameState.MovingTower)
        {
            BuildingManager.Instance.SetHoveredTile(currentTile);
        }
        else
        {
            BuildingManager.Instance.ClearHover(); // Limpia los efectos visuales si el juego está en reposo o menús
        }
    }

    // Permite forzar o reubicar externamente la posición del selector sobre una casilla específica
    public void SetCurrentTile(Tile tile)
    {
        if (tile == null)
            return;

        currentTile = tile;
        UpdateHover();
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Navegador y Selector del Tablero de Juego (GridSelector). Su función principal es 
   gestionar de forma lógica la posición de un cursor o retícula virtual que salta entre casillas interconectadas (Tiles) 
   en respuesta a los mandos del jugador.

   Características clave:
   1. Navegación Espacial Relativa a la Cámara: Resuelve de forma avanzada el problema de los controles en mundos 
      en 3D con perspectivas anguladas o isométricas. Al proyectar matemáticamente el eje del control sobre los 
      vectores de la cámara ('cameraForward' / 'cameraRight'), el movimiento del joystick se siente completamente 
      natural e intuitivo para el usuario desde su punto de vista en la pantalla.
   2. Motor de Decisiones por Producto Punto (Dot Product): No asume que el mapa es una cuadrícula perfecta de 90°. 
      Al utilizar trigonometría vectorial para calcular la proximidad angular de los vecinos de una celda, es capaz 
      de operar de forma fluida sobre mapas con casillas dispuestas de forma hexagonal, irregular o con caminos curvos.
   3. Filtro de Input Profesional: Incorpora controles de tiempo de espera ('moveCooldown') y zonas muertas 
      ('inputDeadZone') sumamente estrictos para evitar que el selector se desplace a velocidades incontrolables o 
      reaccione a micro-vibraciones involuntarias en las palancas físicas del mando.
   ========================================================================================================
*/