using UnityEngine;
using UnityEngine.AI;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private ConstructionData templeInitialConstruction;
    [SerializeField] private int rows = 2;
    [SerializeField] private int columns = 1;

    [SerializeField] private NavMeshSurface navSurface;

    private string navMeshObject = "NavMeshSurfaceBoard";
    private string layerMaskName = "Tiles";

    private float spaceBetweenTiles = 3f;
    private float spaceBetweenRows = 0.8f;
    private float rowOffset = 0f;

    private Tile[,] tiles;

    // LÍNEA RARA / COMPLEJA: Construcción Dinámica del Entorno de Navegación Artificial de la Malla ('Start').
    // Instancia en tiempo de ejecución un GameObject contenedor vacío dedicado a centralizar la geometría caminable. 
    // Le inyecta procedimentalmente el componente 'NavMeshSurface' y define su criterio de recolección en 'CollectObjects.All' 
    // para escanear toda la jerarquía activa. Por último, restringe el horneado físico mediante una máscara de capa 
    // ('LayerMask.GetMask') configurada con el nombre 'Tiles', garantizando que solo las casillas alteren las rutas de la I.A.
    void Start()
    {
        GameObject navObj = new GameObject(navMeshObject);
        navSurface = navObj.AddComponent<NavMeshSurface>();
        navSurface.collectObjects = CollectObjects.All;
        navSurface.layerMask = LayerMask.GetMask(layerMaskName);

        GenerateBoard();
        SpawnTempleInCenter();
        AssignRowNeighbors();
        AssignAllNeighbors();

        navSurface.BuildNavMesh();
    }

    // LÍNEA RARA / COMPLEJA: Factoría Matricial Bidimensional de Tablero Intercalado Desfasado ('GenerateBoard').
    // Inicializa la matriz de punteros 'tiles' con dimensiones rígidas basadas en filas y columnas. Implementa un bucle 
    // anidado donde las filas impares ('x % 2 != 0') sufren una reducción de volumen ('columns - 1') y un desplazamiento lateral 
    // ('rowOffset = spaceBetweenTiles / 2') para recrear un patrón de panal o tablero hexagonal/escalonado. Calcula la posición 
    // espacial en tres dimensiones acoplando el offset, instancia el prefab aplicando rotaciones en Euler fijas, extrae el componente 
    // 'Tile' para guardarlo en la matriz y alterna los colores del mapa mediante una evaluación aritmética posicional base.
    void GenerateBoard()
    {
        tiles = new Tile[rows, columns];

        for (short x = 0; x < rows; x++)
        {
            int currentColumns;

            if (x % 2 != 0)
            {
                currentColumns = columns - 1;
                rowOffset = spaceBetweenTiles / 2;
            }
            else
            {
                currentColumns = columns;
                rowOffset = 0f;
            }

            for (short z = 0; z < currentColumns; z++)
            {
                Vector3 positionOfTiles = new Vector3(
                    x * spaceBetweenRows,
                    0,
                    z * spaceBetweenTiles + rowOffset
                );

                GameObject tileInstance = Instantiate(
                    tilePrefab,
                    positionOfTiles,
                    Quaternion.Euler(90f, 90f, -90f)
                );

                Tile tile = tileInstance.GetComponent<Tile>();
                tiles[x, z] = tile;

                if ((x + z) % 2 == 0)
                {
                    tile.SetColor(Color.white);
                }
                else
                {
                    tile.SetColor(Color.black);
                }
            }
        }
    }

    // Mapea y enlaza las referencias de las casillas contenidas en la misma fila y calcula de forma predictiva los nodos de la fila superior
    void AssignRowNeighbors()
    {
        for (int x = 0; x < rows; x++)
        {
            for (int z = 0; z < columns; z++)
            {
                Tile tile = tiles[x, z];

                if (tile == null)
                    continue;

                tile.sameRowNeighbors.Clear();
                tile.upperRowNeighbors.Clear();

                for (int i = 0; i < columns; i++)
                {
                    if (i != z && tiles[x, i] != null)
                    {
                        tile.sameRowNeighbors.Add(tiles[x, i]);
                    }
                }

                int upperRow = x + 1;

                if (upperRow < rows)
                {
                    for (int i = 0; i < columns; i++)
                    {
                        if (tiles[upperRow, i] != null)
                        {
                            tile.upperRowNeighbors.Add(tiles[upperRow, i]);
                        }
                    }
                }
            }
        }
    }

    // LÍNEA RARA / COMPLEJA: Escaneo Geométrico Radial Colectivo de Proximidad Euclidiana ('AssignAllNeighbors').
    // Realiza un muestreo de fuerza bruta iterando cruzadamente sobre la lista estática globalizada 'Tile.AllTiles'. 
    // Para cada celda del mapa, vacía sus registros previos y calcula la distancia vectorial tridimensional exacta 
    // ('Vector3.Distance') frente a todas las demás baldosas del entorno. Si la magnitud espacial resultante es menor 
    // o igual al umbral crítico estricto de 'maxNeighborDistance' (2.8 unidades), convalida la baldosa como un vecino directo 
    // perimetral y la inyecta en la lista de adyacencia 'allNeighbors' para navegación o propagación de efectos.
    void AssignAllNeighbors()
    {
        float maxNeighborDistance = 2.8f;

        foreach (Tile tile in Tile.AllTiles)
        {
            if (tile == null)
                continue;

            tile.allNeighbors.Clear();

            foreach (Tile otherTile in Tile.AllTiles)
            {
                if (otherTile == null || otherTile == tile)
                    continue;

                float distance = Vector3.Distance(
                    tile.transform.position,
                    otherTile.transform.position
                );

                if (distance <= maxNeighborDistance)
                {
                    tile.allNeighbors.Add(otherTile);
                }
            }

            Debug.Log(tile.name + " neighbors: " + tile.allNeighbors.Count);
        }
    }

    // Determina matemáticamente el índice central de la primera fila horizontal para localizar el punto de origen del mapa
    public Tile GetMiddleTileFirstRow()
    {
        if (tiles == null || rows == 0 || columns == 0)
            return null;

        int middleIndex = columns / 2;

        return tiles[0, middleIndex];
    }

    // Fuerza la colocación del edificio principal de la base inyectando los datos del asset a través del BuildingManager
    void SpawnTempleInCenter()
    {
        Tile middleTile = GetMiddleTileFirstRow();

        Debug.Log("Middle tile: " + middleTile);

        if (middleTile == null)
            return;

        BuildingManager.Instance.currentBuilding = templeInitialConstruction;
        BuildingManager.Instance.PlaceBuilding(middleTile);
        BuildingManager.Instance.currentBuilding = null;
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el **Constructor de Terreno Avanzado y Generador de Mallas de Inteligencia Artificial** (`Board`). 
   Su función principal es orquestar todo el setup del tablero de juego al inicializarse la partida: calcula las coordenadas 
   matemáticas de las casillas, gestiona un diseño escalonado (con un offset en filas alternas para mecánicas de posicionamiento táctico), 
   establece las dependencias de vecindad de cada celda y hornea de manera dinámica el mapa de caminos navegable por el motor (`NavMesh`).

   Características arquitectónicas clave:
   1. Generación de Patrón Alterno / Escalonado: Al aplicar un desplazamiento condicional (`rowOffset`) y un ajuste dinámico 
      de columnas en las filas impares, el código permite construir tableros más complejos y fluidos que las grillas ortogonales 
      tradicionales, ideales para simular distribuciones compactas de defensa y canalización de tropas enemigas.
   2. Horneado Dinámico y Aislado del NavMesh: En lugar de depender de configuraciones estáticas pre-horneadas en el editor 
      de Unity, el script inyecta un objeto físico en tiempo de ejecución, asocia la máscara de capa de interacción (`Tiles`) 
      y compila la topología transitable mediante `navSurface.BuildNavMesh()`. Esto permite crear tableros procedurales o aleatorios 
      completamente compatibles con I.A. sobre la marcha.
   3. Registro Bidimensional y de Grafos de Vecindad: Almacena los componentes tanto en una estructura de matriz rígida 
      (`Tile[,]`) para accesos directos por coordenadas, como en listas de proximidad por distancia matemática (`allNeighbors`). 
      Esto dota al sistema de casillas de una gran flexibilidad para algoritmos de búsqueda de caminos, rango de ataque 
      de torretas o propagación de daño de área.
   ========================================================================================================
*/