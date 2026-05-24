using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;

    public Pooling constructionPooling;

    [Header("Building Data")]
    [SerializeField] private List<ConstructionData> buildingDataList;
    [SerializeField] public ConstructionData currentBuilding;
    [SerializeField] public ConstructionController selectedConstruction;
    [SerializeField] private ConstructionController constructionToMove;

    [Header("Preview")]
    [SerializeField] private Material validPreviewMaterial;
    [SerializeField] private Material invalidPreviewMaterial;

    private GameObject previewObject;
    private Tile hoveredTile;

    public int tempVariable = 0;

    private bool moveMode;
    private float hightOffset = 1.5f;

    // Inicializa la instancia Singleton global y cachea el sistema de pooling para la reutilización de objetos tridimensionales
    private void Awake()
    {
        Instance = this;
        constructionPooling = FindAnyObjectByType<Pooling>();
    }

    // Asegura el estado inicial vacío de la edificación seleccionada al iniciar el ciclo de vida del juego
    private void Start()
    {
        currentBuilding = null;
    }

    // Configura e inyecta la referencia de la edificación actual usando un índice de la lista base de datos
    public void SetConstructionIndex(int index)
    {
        if (index < 0 || index >= buildingDataList.Count)
            return;

        tempVariable = index;
        currentBuilding = buildingDataList[tempVariable];

        moveMode = false;
        constructionToMove = null;

        ShowStatus("Seleccionaste " + GetConstructionName(currentBuilding.type) + ". Elige una casilla para construir.");

        CreatePreviewObject();
        UpdatePreview();
    }

    // Resetea el puntero de la edificación activa y elimina los objetos flotantes de previsualización
    public void ClearCurrentBuildingSelection()
    {
        currentBuilding = null;
        ClearPreviewObject();
        UpdatePreview();
    }

    // Actualiza dinámicamente la baldosa sobre la que se encuentra el puntero del ratón
    public void SetHoveredTile(Tile tile)
    {
        hoveredTile = tile;
        UpdatePreview();
    }

    // Limpia el rastro de la baldosa enfocada y redibuja los contenedores gráficos del terreno
    public void ClearHover()
    {
        hoveredTile = null;
        UpdatePreview();
    }

    // LÍNEA RARA / COMPLEJA: Restauración Masiva de Estado de Malla Táctica y Actualización Crítica del Holograma Preview ('UpdatePreview').
    // Itera recursivamente sobre la colección estática de todas las casillas del escenario táctico ('Tile.AllTiles') para forzar su 
    // restauración de color e iluminación original, evitando que los tintes de previsualizaciones anteriores queden congelados en el mapa. 
    // Acto seguido, valida la presencia de selecciones activas y delega el cálculo de coloración contextual y traslación del objeto fantasma 
    // a las subfunciones específicas de renderizado y posicionamiento.
    private void UpdatePreview()
    {
        foreach (Tile t in Tile.AllTiles)
            t.ResetSelf();

        if (currentBuilding == null || hoveredTile == null)
        {
            UpdatePreviewObject(null);
            return;
        }

        ApplyPreview(hoveredTile);
        UpdatePreviewObject(hoveredTile);
    }

    // LÍNEA RARA / COMPLEJA: Inyección Procedural de Matrices de Color por Vecindad de Fila Estructurada ('ApplyPreview').
    // Determina la viabilidad espacial llamando a 'CanPlaceOnTile'. Aplica un operador ternario básico para teñir la baldosa raíz (Amarillo/Rojo). 
    // Si la estructura corresponde a un muro ('ConstructionType.Wall'), el script expande su análisis lógico e itera sobre los arreglos 
    // de vecinos horizontales ('sameRowNeighbors') y superiores ('upperRowNeighbors') para inyectarles colores diferenciados (Cian y Verde), 
    // previsualizando en tiempo real el área de cobertura total que abarcará la barrera defensiva multilocalizada antes de ser consolidada.
    private void ApplyPreview(Tile tile)
    {
        bool valid = CanPlaceOnTile(tile);

        tile.SetTempColor(valid ? Color.yellow : Color.red);

        if (currentBuilding.type == ConstructionType.Wall)
        {
            foreach (Tile t in tile.sameRowNeighbors)
            {
                if (t != null)
                    t.SetTempColor(valid ? Color.cyan : Color.red);
            }

            foreach (Tile t in tile.upperRowNeighbors)
            {
                if (t != null)
                    t.SetTempColor(valid ? Color.green : Color.red);
            }
        }
    }

    // Almacena la referencia técnica de la construcción seleccionada para su posterior manipulación o consulta de atributos
    public void Select(ConstructionController construction)
    {
        selectedConstruction = construction;
        Debug.Log("Seleccionado: " + construction.name);
    }

    // LÍNEA RARA / COMPLEJA: Algoritmo Mutativo con Penalización por Repetición y Despliegue de Componentes de Pooling ('PlaceBuilding').
    // Valida la escala de tiempo. Evalúa si ya existe una estructura del mismo tipo en el mapa mediante 'ExistsConstructionOfType'. 
    // Si es positivo, aplica la lista de costos incrementales por inflación de recursos ('bonusWillToPay'); si es la primera unidad, usa la tasa base 
    // ('willToPay'). Tras debitar el capital mediante 'WillManager', procesa las casillas objetivo e invoca al sistema de optimización de memoria 
    // 'constructionPooling.CreateObject', evitando la sobrecarga del recolector de basura al reciclar las mallas tridimensionales desde el pool.
    public void PlaceBuilding(Tile tile)
    {
        if (WaveSpawner.Instance.IsWaveRunning())
        {
            PlayInvalidSound();
            ShowStatus("No puedes construir durante la oleada");
            return;
        }

        if (moveMode)
        {
            MoveConstruction(tile);
            return;
        }

        if (currentBuilding == null)
        {
            PlayInvalidSound();
            ShowStatus("Primero selecciona una construcción");
            return;
        }

        if (tile == null)
        {
            PlayInvalidSound();
            ShowStatus("Selecciona una casilla válida");
            return;
        }

        List<Tile> tilesToBuild = GetTilesToBuild(tile);

        foreach (Tile t in tilesToBuild)
        {
            if (t == null)
            {
                PlayInvalidSound();
                ShowStatus("Espacio inválido");
                return;
            }

            if (t.IsOccupied)
            {
                PlayInvalidSound();
                ShowStatus("Espacio ocupado");
                return;
            }
        }

        int buildingCountIndex = ExistsConstructionOfType(currentBuilding.type);
        List<WillProduction> bonusWillsToPay =  currentBuilding.bonusWillToPay;
        List<WillProduction> actualWillsToPay = currentBuilding.willToPay;

        foreach (WillProduction w in actualWillsToPay)
        {
            foreach(WillProduction b in bonusWillsToPay)
            {
                if(w.type == b.type)
                {
                    if (!WillManager.Instance.SpendMoney(w.type, w.amount + (b.amount)*buildingCountIndex))
                    {
                        PlayInvalidSound();
                        ShowStatus("Te falta " + w.amount + " de " + w.type + " para construir.");
                        return;
                    }

                }
            }
        }

        /*
        bool hasSameType = ExistsConstructionOfType(currentBuilding.type);
        List<WillProduction> costList = hasSameType ? currentBuilding.bonusWillToPay : currentBuilding.willToPay;

        foreach (WillProduction w in costList)
        {
            if (!WillManager.Instance.SpendMoney(w.type, w.amount))
            {
                PlayInvalidSound();
                ShowStatus("Te falta " + w.amount + " de " + w.type + " para construir.");
                return;
            }
        }
        */

        ConstructionGroup group = null;

        if (tilesToBuild.Count > 1)
        {
            GameObject groupObj = new GameObject("ConstructionGroup");
            group = groupObj.AddComponent<ConstructionGroup>();
            group.tiles = tilesToBuild;
        }

        foreach (Tile t in tilesToBuild)
        {
            if (t == null)
                continue;

            t.SetOccupied(true);

            GameObject building = constructionPooling.CreateObject(currentBuilding.prefab, t.transform);

            building.transform.position = t.transform.position + Vector3.up * hightOffset;
            building.transform.rotation = Quaternion.identity;

            BaseConstruction baseConstruction = building.GetComponent<BaseConstruction>();
            ConstructionController constructionController = building.GetComponent<ConstructionController>();

            if (group != null)
                constructionController.group = group;

            baseConstruction.SetTile(t);
            baseConstruction.Initialize(currentBuilding);
            constructionController.Initialize(currentBuilding);
        }

        if (GameplaySoundPlayer.Instance != null)
        {
            if (currentBuilding.type == ConstructionType.Wall)
                GameplaySoundPlayer.Instance.PlayBuildWall();
            else
                GameplaySoundPlayer.Instance.PlayBuildStructure();
        }

        ShowStatus(GetConstructionName(currentBuilding.type) + " colocado correctamente.");


        ClearCurrentBuildingSelection();
    }

    // Configura e inicializa el estado lógico global para trasladar estructuras existentes a lo largo de la cuadrícula
    public void TryEnterMoveMode()
    {
        currentBuilding = null;
        ClearPreviewObject();
        UpdatePreview();

        if (WaveSpawner.Instance.IsWaveRunning())
        {
            PlayInvalidSound();
            ShowStatus("No puedes mover estructuras durante la oleada");
            return;
        }

        if (selectedConstruction == null)
        {
            PlayInvalidSound();
            ShowStatus("Selecciona una estructura para mover");
            return;
        }

        BaseConstruction baseConstruction = selectedConstruction.GetComponent<BaseConstruction>();

        if (baseConstruction == null || baseConstruction.Data == null)
        {
            PlayInvalidSound();
            ShowStatus("Estructura inválida");
            return;
        }

        if (baseConstruction.Data.type == ConstructionType.Temple)
        {
            PlayInvalidSound();
            ShowStatus("El templo no se puede mover");
            return;
        }

        moveMode = true;
        constructionToMove = selectedConstruction;

        ShowStatus("Modo mover activado");
    }

    // LÍNEA RARA / COMPLEJA: Reubicación Coordenada Colectiva de Miembros de Grupo Estructural ('MoveConstruction').
    // Desglosa las baldosas de origen y destino discriminando si el objeto posee un componente de enlace colectivo ('group'). 
    // Libera de forma masiva los conmutadores lógicos de ocupación de las celdas antiguas setting '.SetOccupied(false)' 
    // y desplaza espacialmente mediante matrices de transformación lineal cada pieza del conjunto hacia las nuevas coordenadas 
    // calculadas de forma indexada, resincronizando sus punteros nativos de baldosas sin necesidad de destruir o recrear las entidades.
    private void MoveConstruction(Tile newTile)
    {
        if (constructionToMove == null)
        {
            PlayInvalidSound();
            ShowStatus("No hay estructura para mover");
            return;
        }

        if (newTile == null)
        {
            PlayInvalidSound();
            ShowStatus("Espacio inválido");
            return;
        }

        List<ConstructionController> constructionsToMove = new List<ConstructionController>();
        List<Tile> oldTiles = new List<Tile>();
        List<Tile> newTiles = new List<Tile>();

        if (constructionToMove.group == null)
        {
            constructionsToMove.Add(constructionToMove);

            BaseConstruction baseConstruction = constructionToMove.GetComponent<BaseConstruction>();

            oldTiles.Add(baseConstruction.GetTile());
            newTiles.Add(newTile);
        }
        else
        {
            constructionsToMove.AddRange(constructionToMove.group.members);

            oldTiles.AddRange(constructionToMove.group.tiles);

            newTiles.Add(newTile);
            newTiles.AddRange(newTile.sameRowNeighbors);
            newTiles.AddRange(newTile.upperRowNeighbors);
        }

        foreach (Tile tile in newTiles)
        {
            if (tile == null)
            {
                PlayInvalidSound();
                ShowStatus("Espacio inválido");
                return;
            }

            if (tile.IsOccupied && !oldTiles.Contains(tile))
            {
                PlayInvalidSound();
                ShowStatus("Espacio ocupado");
                return;
            }
        }

        foreach (Tile tile in oldTiles)
        {
            if (tile != null)
                tile.SetOccupied(false);
        }

        for (int i = 0; i < constructionsToMove.Count; i++)
        {
            ConstructionController construction = constructionsToMove[i];
            BaseConstruction baseConstruction = construction.GetComponent<BaseConstruction>();
            Tile targetTile = newTiles[i];

            targetTile.SetOccupied(true);

            construction.transform.position = targetTile.transform.position + Vector3.up * hightOffset;
            construction.transform.rotation = Quaternion.identity;

            baseConstruction.SetTile(targetTile);
        }

        if (constructionToMove.group != null)
            constructionToMove.group.tiles = newTiles;

        moveMode = false;
        constructionToMove = null;

        ShowStatus("Estructura movida correctamente.");

        ClearCurrentBuildingSelection();
    }


    /*
    // Busca de forma exhaustiva en la escena la presencia de cualquier entidad activa que coincida con el tipo estructural consultado
    private bool ExistsConstructionOfType(ConstructionType type)
    {
        BaseConstruction[] all = FindObjectsByType<BaseConstruction>(FindObjectsSortMode.None);

        foreach (BaseConstruction b in all)
        {
            if (b != null && b.Data != null && b.Data.type == type)
                return true;
        }

        return false;
    }
    */

    //HUBO CAMBIOS AQUÍ
    private int ExistsConstructionOfType(ConstructionType type)
    {
        int buildingCountIndex = 0;
        BaseConstruction[] all = FindObjectsByType<BaseConstruction>(FindObjectsSortMode.None);
        foreach (BaseConstruction b in all)
        {
            if (b != null && b.Data != null && b.Data.type == type)
            {
                buildingCountIndex++;
            }
        }
        return buildingCountIndex;
    }

    // Retorna la colección de baldosas requeridas para una edificación, anexando vecinos si se trata de un muro compuesto
    private List<Tile> GetTilesToBuild(Tile tile)
    {
        List<Tile> tiles = new List<Tile>();

        if (tile == null)
            return tiles;

        tiles.Add(tile);

        if (currentBuilding != null && currentBuilding.type == ConstructionType.Wall)
        {
            tiles.AddRange(tile.sameRowNeighbors);
            tiles.AddRange(tile.upperRowNeighbors);
        }

        return tiles;
    }

    // Evalúa de forma integral las condiciones de ocupación física del suelo y la solvencia económica del usuario
    private bool CanPlaceOnTile(Tile tile)
    {
        if (tile == null || currentBuilding == null)
            return false;

        List<Tile> tilesToBuild = GetTilesToBuild(tile);

        foreach (Tile t in tilesToBuild)
        {
            if (t == null || t.IsOccupied)
                return false;
        }

        return HasEnoughResources(currentBuilding);
    }

    //HUBO CAMBIOS AQUÍ
    // Compara el inventario monetario global contra el costo de producción indexado de la estructura seleccionada
    /*
    private bool HasEnoughResources(ConstructionData data)
    {
        if (data == null)
            return false;

        bool hasSameType = ExistsConstructionOfType(data.type);
        List<WillProduction> costList = hasSameType ? data.bonusWillToPay : data.willToPay;

        foreach (WillProduction w in costList)
        {
            if (!WillManager.Instance.HasEnoughMoney(w.type, w.amount))
                return false;
        }

        return true;
    }
    */
    private bool HasEnoughResources(ConstructionData data)
    {
        int buildingCountIndex = ExistsConstructionOfType(currentBuilding.type);
        List<WillProduction> bonusWillsToPay = currentBuilding.bonusWillToPay;
        List<WillProduction> actualWillsToPay = currentBuilding.willToPay;

        foreach (WillProduction w in actualWillsToPay)
        {
            foreach (WillProduction b in bonusWillsToPay)
            {
                if (w.type == b.type)
                {
                    if (!WillManager.Instance.HasEnoughMoney(w.type, w.amount))
                        return false;
                }
            }
        }
        return true;
    }

    // LÍNEA RARA / COMPLEJA: Clonación Estática Secuencial y Desactivación por Software de Componentes en Holograma ('CreatePreviewObject').
    // Instancia una copia huérfana del prefab original para que actúe como fantasma de posicionamiento espacial. Para evitar colisiones 
    // físicas anómalas o interferencias con el sistema de trazado de rayos ('Raycasting'), recorre matricialmente todos los 'Collider' 
    // desactivándolos mediante '.enabled = false'. Ejecuta el mismo procedimiento de apagado con todos los componentes lógicos que hereden 
    // de 'MonoBehaviour' para congelar la lógica interna e inteligencia artificial del objeto durante su fase de previsualización estética.
    private void CreatePreviewObject()
    {
        ClearPreviewObject();

        if (currentBuilding == null || currentBuilding.prefab == null)
            return;

        previewObject = Instantiate(currentBuilding.prefab);
        previewObject.name = "Preview_" + currentBuilding.type;

        foreach (Collider col in previewObject.GetComponentsInChildren<Collider>())
            col.enabled = false;

        foreach (MonoBehaviour behaviour in previewObject.GetComponentsInChildren<MonoBehaviour>())
            behaviour.enabled = false;

        ApplyPreviewMaterial(validPreviewMaterial);
        previewObject.SetActive(false);
    }

    // Destruye de la memoria de Unity el objeto translúcido de previsualización activa y purga la variable contenedora
    private void ClearPreviewObject()
    {
        if (previewObject != null)
            Destroy(previewObject);

        previewObject = null;
    }

    // Traslada el objeto fantasma hacia las coordenadas espaciales de la baldosa enfocada alternando su material según validez técnica
    private void UpdatePreviewObject(Tile tile)
    {
        if (previewObject == null || currentBuilding == null || tile == null)
        {
            if (previewObject != null)
                previewObject.SetActive(false);

            return;
        }

        previewObject.SetActive(true);
        previewObject.transform.position = tile.transform.position + Vector3.up * hightOffset;
        previewObject.transform.rotation = Quaternion.identity;

        bool valid = CanPlaceOnTile(tile);
        ApplyPreviewMaterial(valid ? validPreviewMaterial : invalidPreviewMaterial);
    }

    // Extrae y sobrescribe de manera recursiva la propiedad material de todos los componentes de renderizado de la estructura flotante
    private void ApplyPreviewMaterial(Material material)
    {
        if (previewObject == null || material == null)
            return;

        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
            renderer.material = material;
    }

    // Reproduce la pista de audio asignada para denotar un error en el posicionamiento o falta de fondos económicos
    private void PlayInvalidSound()
    {
        if (GameplaySoundPlayer.Instance != null)
            GameplaySoundPlayer.Instance.PlayInvalidPlacement();
    }

    // Registra logs internos en la consola de depuración y reenvía las alertas textuales al sistema de notificaciones del HUD
    private void ShowStatus(string message)
    {
        if (StatusMessageUI.Instance != null)
            StatusMessageUI.Instance.ShowMessage(message);

        Debug.Log(message);
    }

    private string GetConstructionName(ConstructionType type)
    {
        switch (type)
        {
            case ConstructionType.Wall:
                return "Muro";
            case ConstructionType.Defense:
                return "Defensa";
            case ConstructionType.Barracks:
                return "Barracas";
            case ConstructionType.Plaza:
                return "Plaza";
            case ConstructionType.Temple:
                return "Templo";
            default:
                return type.ToString();
        }
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Gestor y Núcleo de Construcción Táctica del Escenario (BuildingManager). Su cometido principal 
   dentro del flujo lógico del videojuego es coordinar la colocación, validación, previsualización material y traslado 
   espacial de todas las estructuras defensivas y muros sobre la cuadrícula del mapa, interactuando de forma síncrona con los 
   sistemas económicos globales (`WillManager`) y la planificación del flujo de oleadas enemigas (`WaveSpawner`).

   Características clave:
   1. Control de Costos por Inflación de Estructuras: Incorpora una mecánica avanzada de economía donde el precio de 
      las edificaciones varía dinámicamente. Evalúa la presencia previa de unidades del mismo tipo en la escena para decidir 
      si cobra la tarifa base o aplica penalizaciones de encarecimiento progresivo definidas en las variables de bonus.
   2. Sistema de Previsualización Holográfica Limpia: Genera copias flotantes eficientes a través de la desactivación recursiva 
      por software de colisionadores y scripts nativos. Esto evita que los objetos fantasma interfieran con las físicas globales, 
      mientras que la alteración dinámica de materiales (`validPreviewMaterial` / `invalidPreviewMaterial`) otorga un feedback inmediato.
   3. Gestión de Estructuras Compuestas (Muros por Grupo): Cuenta con soporte lógico nativo para gestionar la colocación y el 
      traslado unificado de elementos que ocupan múltiples celdas espaciales (como las secciones de muros de la Acrópolis). Al enlazar 
      las baldosas adyacentes a través de clases contenedoras (`ConstructionGroup`), permite mover o bloquear bloques enteros sin perder la integridad.
   ========================================================================================================
*/