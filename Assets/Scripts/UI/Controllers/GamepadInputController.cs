using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadInputController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridSelector selector;
    [SerializeField] private RadialMenuUI radialMenu;
    [SerializeField] private ShopUI shopUI;
    [SerializeField] private PauseMenuUI pauseMenuUI;
    [SerializeField] private GameSpeedUI gameSpeedUI;

    [Header("D-Pad")]
    [SerializeField] private int wallConstructionIndex = 3;
    private bool dpadInUse;
    [SerializeField] private DPadActionPanelUI dpadPanelUI;

    [Header("Blessing Selection")]
    [SerializeField] private BlessingChoiceUI blessingChoiceUI;

    [Header("Settings")]
    [SerializeField] private SettingsPanelUI settingsPanelUI;

    [Header("Controls")]
    [SerializeField] private ControlsPanelUI controlsPanelUI;

    [SerializeField] private UISoundPlayer uiSoundPlayer;

    // Busca de forma automática en la escena las referencias que no hayan sido asignadas manualmente en el Inspector
    private void Awake()
    {
        // LÍNEA RARA / COMPLEJA: 'FindAnyObjectByType<T>()' es un método moderno de Unity (remplaza al antiguo FindObjectOfType).
        // Se encarga de buscar en toda la escena activa cualquier objeto que tenga el script especificado entre los signos '< >'.
        // Es muy útil para autoconectar componentes, pero debe usarse únicamente en Awake o Start porque es costoso en rendimiento.
        if (selector == null)
            selector = FindAnyObjectByType<GridSelector>();

        if (radialMenu == null)
            radialMenu = FindAnyObjectByType<RadialMenuUI>();

        if (shopUI == null)
            shopUI = FindAnyObjectByType<ShopUI>();

        if (gameSpeedUI == null)
            gameSpeedUI = FindAnyObjectByType<GameSpeedUI>();

        if (pauseMenuUI == null)
            pauseMenuUI = FindAnyObjectByType<PauseMenuUI>();

        if (dpadPanelUI == null)
            dpadPanelUI = FindAnyObjectByType<DPadActionPanelUI>();

        if (blessingChoiceUI == null)
            blessingChoiceUI = FindAnyObjectByType<BlessingChoiceUI>();

        if (settingsPanelUI == null)
            settingsPanelUI = FindAnyObjectByType<SettingsPanelUI>();

        if (controlsPanelUI == null)
            controlsPanelUI = FindAnyObjectByType<ControlsPanelUI>();

        if (uiSoundPlayer == null)
            uiSoundPlayer = FindAnyObjectByType<UISoundPlayer>();
    }

    private bool IsTutorialOpen()
    {
        return GameStateController.Instance != null &&
               GameStateController.Instance.currentState == GameState.Tutorial;
    }

    // ---------------------------
    // MOVEMENT
    // ---------------------------
    // Mensaje automático enviado por el componente 'Player Input' de Unity al mover el Joystick Izquierdo o las Flechas
    public void OnMove(InputValue value)
    {
        if (IsTutorialOpen())
            return;

        // EXPLICACIÓN: 'value.Get<Vector2>()' extrae las coordenadas físicas X (horizontal) y Y (vertical) del stick.
        // Los valores devueltos oscilan entre -1f y 1f, mapeando la dirección exacta del joystick del control.
        Vector2 input = value.Get<Vector2>();

        // LÓGICA DE ESTADOS: Redirecciona el mismo Joystick para controlar diferentes sistemas según el momento del juego.
        if (GameStateController.Instance.currentState == GameState.BlessingSelection)
        {
            if (blessingChoiceUI != null)
                blessingChoiceUI.Move(input); // Mueve el cursor de selección de bendiciones de dioses

            return;
        }

        if (GameStateController.Instance.currentState == GameState.RadialOpen)
        {
            radialMenu.ReadStick(input); // Direcciona la aguja del menú radial selector
            return;
        }

        if (GameStateController.Instance.currentState == GameState.MapIdle ||
            GameStateController.Instance.currentState == GameState.PlacingTower ||
            GameStateController.Instance.currentState == GameState.MovingTower)
        {
            selector.Move(input); // Mueve el selector de la cuadrícula o rejilla sobre el mapa del juego
        }
    }

    // ---------------------------
    // CONFIRM (A)
    // ---------------------------
    // Registra la pulsación del botón de confirmación (típicamente el botón 'A' en Xbox o 'X' en PlayStation)
    public void OnConfirm(InputValue value)
    {
        // Verifica que la acción corresponda al momento en que se presiona el botón, no cuando se suelta
        if (!value.isPressed)
            return;

        GameState state = GameStateController.Instance.currentState;

        if (state == GameState.Tutorial)
        {
            if (TutorialPanelUI.Instance != null)
                TutorialPanelUI.Instance.TryCloseFromInput();

            return;
        }

        if (uiSoundPlayer != null)
            uiSoundPlayer.PlayConfirm();

        if (state == GameState.BlessingSelection)
        {
            if (blessingChoiceUI != null)
                blessingChoiceUI.Confirm();

            return;
        }

        // RADIAL
        if (state == GameState.RadialOpen)
        {
            radialMenu.Confirm();
            return;
        }

        // SHOP
        if (state == GameState.ShopOpen)
        {
            shopUI.ConfirmSelection();
            return;
        }

        // MAP IDLE
        if (state == GameState.MapIdle)
        {
            TrySelectTower();
            return;
        }

        // PLACING
        if (state == GameState.PlacingTower)
        {
            BuildingManager.Instance.PlaceBuilding(selector.currentTile);

            if (shopUI != null)
                shopUI.Close();

            GameStateController.Instance.SetState(GameState.MapIdle);
            return;
        }

        // MOVING
        if (state == GameState.MovingTower)
        {
            BuildingManager.Instance.PlaceBuilding(selector.currentTile);

            if (shopUI != null)
                shopUI.Close();

            GameStateController.Instance.SetState(GameState.MapIdle);
            return;
        }

        if (state == GameState.PlacingTower || state == GameState.MovingTower || state == GameState.RadialOpen)
        {
            if (IsWaveBlockingAction())
                return;
        }
    }

    // ---------------------------
    // CANCEL (B)
    // ---------------------------
    // Registra la pulsación del botón de retroceso (típicamente el botón 'B' en Xbox o 'Círculo' en PlayStation)
    public void OnCancel(InputValue value)
    {
        if (!value.isPressed)
            return;

        GameState state = GameStateController.Instance.currentState;

        if (state == GameState.Tutorial)
            return;

        if (uiSoundPlayer != null)
            uiSoundPlayer.PlayCancel();

        if (state == GameState.RadialOpen)
        {
            radialMenu.Close();
            GameStateController.Instance.SetState(GameState.MapIdle);
            return;
        }

        if (state == GameState.ShopOpen)
        {
            shopUI.Close();
            return;
        }

        if (state == GameState.PlacingTower || state == GameState.MovingTower)
        {
            GameStateController.Instance.SetState(GameState.MapIdle);
            return;
        }

        if (state == GameState.TownHallMenu)
        {
            GameStateController.Instance.SetState(GameState.MapIdle);
            return;
        }

        if (state == GameState.Settings)
        {
            if (settingsPanelUI != null)
                settingsPanelUI.CloseToPause();

            return;
        }

        if (state == GameState.Controls)
        {
            if (controlsPanelUI != null)
                controlsPanelUI.CloseToPause();

            return;
        }

        // PAUSE (B para cerrar pausa)
        if (state == GameState.Paused)
        {
            if (pauseMenuUI != null)
                pauseMenuUI.Close();

            return;
        }

        if (state == GameState.BlessingSelection)
        {
            if (blessingChoiceUI != null)
                blessingChoiceUI.Close();

            return;
        }
    }

    // ---------------------------
    // MOVE TOWER (X)
    // ---------------------------
    // Activa la reubicación de una torre previamente seleccionada (Botón 'X' de Xbox / 'Cuadrado' de PlayStation)
    public void OnMoveTower(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (IsTutorialOpen())
            return;

        // Comprueba que no haya una horda activa atacando que impida reconstruir
        if (IsWaveBlockingAction())
            return;

        if (GameStateController.Instance.currentState != GameState.MapIdle)
            return;

        if (BuildingManager.Instance.selectedConstruction == null)
            return;

        BuildingManager.Instance.TryEnterMoveMode();
        GameStateController.Instance.SetState(GameState.MovingTower);
    }

    // ---------------------------
    // START WAVE (Y)
    // ---------------------------
    // Inicia u optimiza el control del tiempo/oleada (Botón 'Y' de Xbox / 'Triángulo' de PlayStation)
    public void OnStartWave(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (IsTutorialOpen())
            return;

        if (GameStateController.Instance.currentState != GameState.MapIdle)
            return;

        if (TutorialPanelUI.Instance != null &&
            TutorialPanelUI.Instance.ShouldShowTutorial())
        {
            TutorialPanelUI.Instance.Open();
            return;
        }

        if (gameSpeedUI != null)
        {
            gameSpeedUI.ToggleSpeed(); // Cambia o acelera la velocidad temporal del combate
        }
    }

    // ---------------------------
    // PAUSE (START)
    // ---------------------------
    // Alterna la activación del menú de pausa principal mediante el botón de opciones
    public void OnPause(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (IsTutorialOpen())
            return;

        if (pauseMenuUI != null)
        {
            pauseMenuUI.Toggle();
        }
    }

    // ---------------------------
    // SHOP (LT / RT)
    // ---------------------------
    // Los gatillos traseros abren o cierran el catálogo de compras del juego
    public void OnOpenShopLeft(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (IsTutorialOpen())
            return;

        if (IsWaveBlockingAction())
            return;

        shopUI.Toggle();
    }

    public void OnOpenShopRight(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (IsTutorialOpen())
            return;

        if (IsWaveBlockingAction())
            return;

        shopUI.Toggle();
    }

    // ---------------------------
    // SHOP NAVIGATION (LB / RB)
    // ---------------------------
    // Los botones superiores (Bumpers) cambian de pestañas o artículos en el menú de la tienda
    public void OnShopLeft(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (IsTutorialOpen())
            return;

        shopUI.MoveLeft();
    }

    public void OnShopRight(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (IsTutorialOpen())
            return;

        shopUI.MoveRight();
    }

    // ---------------------------
    // SELECT TOWER
    // ---------------------------
    // Intenta identificar y seleccionar la estructura ubicada debajo del cursor del jugador
    private void TrySelectTower()
    {
        if (WaveSpawner.Instance.IsWaveRunning())
            return;

        Tile tile = selector.currentTile;

        if (tile == null)
            return;

        if (!tile.IsOccupied)
        {
            Debug.Log("Tile is empty");
            return;
        }

        ConstructionController construction = FindConstructionOnTile(tile);

        if (construction == null)
        {
            Debug.Log("No construction found on this tile");
            return;
        }

        BuildingManager.Instance.Select(construction);

        // LÍNEA RARA / COMPLEJA: Intenta extraer componentes específicos usando Polimorfismo.
        // Obtiene el script base y el componente del Templo de forma separada para validar si la estructura 
        // seleccionada corresponde al Templo principal del mapa o es una edificación defensiva estándar.
        BaseConstruction baseConstruction = construction.GetComponent<BaseConstruction>();
        Temple temple = construction.GetComponent<Temple>();

        if (baseConstruction != null &&
            baseConstruction.Data != null &&
            baseConstruction.Data.type == ConstructionType.Temple)
        {
            BuildingManager.Instance.Select(construction);

            // Si el templo no tiene una deidad vinculada, interrumpe el flujo normal para obligar a elegir un Dios
            if (!construction.HasBlessingChosen())
            {
                if (blessingChoiceUI != null)
                    blessingChoiceUI.Open(construction);

                return;
            }

            if (radialMenu != null)
                radialMenu.Open(construction);

            GameStateController.Instance.SetState(GameState.RadialOpen);
            return;
        }

        if (radialMenu != null)
            radialMenu.Open(construction);

        GameStateController.Instance.SetState(GameState.RadialOpen);
    }

    // Busca de manera iterativa dentro del listado estático global qué construcción coincide exactamente con la celda consultada
    private ConstructionController FindConstructionOnTile(Tile tile)
    {
        // Bucle Foreach: Recorre uno a uno todos los elementos contenidos en la lista 'AllConstructions'
        foreach (BaseConstruction construction in BaseConstruction.AllConstructions)
        {
            if (construction == null)
                continue; // Salta al siguiente elemento de la lista si este registro está destruido o corrupto

            if (construction.GetTile() == tile)
            {
                return construction.GetComponent<ConstructionController>(); // Retorna el script controlador si coinciden las coordenadas
            }
        }

        return null;
    }

    // Captura los clics realizados en la cruceta direccional (D-Pad / Flechas de un Gamepad)
    public void OnDpad(InputValue value)
    {
        if (IsTutorialOpen())
            return;

        Vector2 input = value.Get<Vector2>();

        // EXPLICACIÓN COMPLEJA: Evita lecturas continuas o falsos positivos (anti-spam).
        // Las crucetas de los gamepads a veces detectan micro-movimientos imprecisos. Al medir la magnitud del vector,
        // si el usuario no presiona firmemente la flecha (magnitud menor a 0.4f), se apaga la bandera y se ignora el input.
        if (input.magnitude < 0.4f)
        {
            dpadInUse = false;
            return;
        }

        // Si la cruceta se mantiene hundida de manera continua, bloquea la repetición del comando hasta que el usuario la suelte
        if (dpadInUse)
            return;

        dpadInUse = true;

        if (GameStateController.Instance.currentState != GameState.MapIdle)
            return;

        if (IsWaveBlockingAction())
            return;

        // EVALUACIÓN DE DIRECCIÓN: Determina qué flecha física de la cruceta fue presionada en base a los ejes cartesianos
        if (input.y > 0.5f) // D-Pad Arriba
        {
            if (dpadPanelUI != null)
                dpadPanelUI.FlashUp(); // Destello visual en la interfaz

            RepairWallShortcut();
        }
        else if (input.y < -0.5f) // D-Pad Abajo
        {
            if (dpadPanelUI != null)
                dpadPanelUI.FlashDown();

            BuildWallShortcut();
        }
        else if (input.x < -0.5f) // D-Pad Izquierda
        {
            if (dpadPanelUI != null)
                dpadPanelUI.FlashLeft();

            MoveSelectedStructureShortcut();
        }
        else if (input.x > 0.5f) // D-Pad Derecha
        {
            if (dpadPanelUI != null)
                dpadPanelUI.FlashRight();

            FocusTempleShortcut();
        }
    }

    // Función de control que bloquea mecánicas constructivas durante la ejecución del combate
    private bool IsWaveBlockingAction()
    {
        if (!WaveSpawner.Instance.IsWaveRunning())
            return false; // Permite la acción si la oleada no está en curso

        if (StatusMessageUI.Instance != null)
            StatusMessageUI.Instance.ShowMessage("No puedes hacer esto durante la oleada");

        return true; // Bloquea la acción
    }

    // Atajo rápido para equipar directamente la construcción de muros
    private void BuildWallShortcut()
    {
        BuildingManager.Instance.SetConstructionIndex(wallConstructionIndex);
        GameStateController.Instance.SetState(GameState.PlacingTower);

        if (StatusMessageUI.Instance != null)
            StatusMessageUI.Instance.ShowMessage("Construir muro");
    }

    // Atajo rápido para reparar los daños generales del muro
    private void RepairWallShortcut()
    {
        Wall wall = FindAnyObjectByType<Wall>();

        if (wall == null)
        {
            StatusMessageUI.Instance.ShowMessage("No hay muro para reparar");
            return;
        }

        wall.RepairGroup(); // Activa la reparación colectiva del sistema defensivo
        StatusMessageUI.Instance.ShowMessage("Reparando muro");
    }

    // Atajo rápido que extrae la estructura del mosaico apuntado y arranca su modo de desplazamiento
    private void MoveSelectedStructureShortcut()
    {
        Tile tile = selector.currentTile;

        if (tile == null || !tile.IsOccupied)
        {
            StatusMessageUI.Instance.ShowMessage("Colócate sobre una estructura para moverla");
            return;
        }

        BuildingManager.Instance.ClearCurrentBuildingSelection();

        ConstructionController construction = FindConstructionOnTile(tile);

        if (construction == null)
        {
            StatusMessageUI.Instance.ShowMessage("No hay estructura para mover");
            return;
        }

        BuildingManager.Instance.Select(construction);

        if (radialMenu != null)
            radialMenu.Close();

        BuildingManager.Instance.TryEnterMoveMode();
        GameStateController.Instance.SetState(GameState.MovingTower);

        StatusMessageUI.Instance.ShowMessage("Mover estructura");
    }

    // Atajo rápido para encuadrar y centrar inmediatamente el enfoque en el Templo Sagrado
    private void FocusTempleShortcut()
    {
        ConstructionController temple = FindTempleController();

        if (temple == null)
        {
            StatusMessageUI.Instance.ShowMessage("No se encontró el templo");
            Debug.LogWarning("No temple found.");
            return;
        }

        BaseConstruction baseConstruction = temple.GetComponent<BaseConstruction>();

        if (baseConstruction == null || baseConstruction.Data == null)
        {
            StatusMessageUI.Instance.ShowMessage("El templo no tiene datos");
            return;
        }

        BuildingManager.Instance.Select(temple);

        // Si el templo requiere asignar una bendición, despliega el panel divino correspondiente
        if (!temple.HasBlessingChosen())
        {
            if (blessingChoiceUI == null)
            {
                StatusMessageUI.Instance.ShowMessage("Blessing UI no está conectada");
                Debug.LogWarning("BlessingChoiceUI reference is missing.");
                return;
            }

            blessingChoiceUI.Open(temple);
            return;
        }

        // Si ya posee una bendición divina, despliega el menú radial estándar para interactuar con él
        if (radialMenu != null)
            radialMenu.Open(temple);

        GameStateController.Instance.SetState(GameState.RadialOpen);
    }

    // Localiza de manera exhaustiva el Templo dentro de los registros activos del mapa de juego
    private ConstructionController FindTempleController()
    {
        foreach (BaseConstruction construction in BaseConstruction.AllConstructions)
        {
            if (construction == null)
                continue;

            Temple templeComponent = construction.GetComponent<Temple>();

            if (templeComponent != null)
                return construction.GetComponent<ConstructionController>();

            if (construction.Data != null && construction.Data.type == ConstructionType.Temple)
                return construction.GetComponent<ConstructionController>();
        }

        return null;
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Traductor e Intérprete del Control de Mandos (GamepadInputController). Su principal 
   objetivo es recibir las entradas físicas de un mando clásico de consola (usando el moderno sistema 'New Input System' 
   de Unity) y transformarlas en instrucciones lógicas dentro del videojuego (un juego estilo Tower Defense o Estrategia).

   Características clave:
   1. Control Dinámico Basado en Contexto: Redirecciona de manera sumamente eficiente las palancas y botones del mando. 
      Por ejemplo, la palanca izquierda sirve para mover la retícula sobre el mapa en estado normal, pero si el usuario abre 
      un menú de deidades o una interfaz de selección radial, la palanca pasa automáticamente a navegar las opciones internas 
      de dichos menús sin encimarse con el mundo.
   2. Sistema de Atajos mediante Cruceta (D-Pad): Mapea las direcciones de las flechas del control para desencadenar 
      acciones instantáneas o macroinstrucciones del juego, tales como la compra rápida de murallas, reparaciones grupales automatizadas, 
      focalización inmediata en el Templo central o la activación del traslado de torres.
   3. Protección de Estado y Flujo del Juego (Seguridad de Estado): Monitorea de manera constante a través del 'GameStateController' 
      y del 'WaveSpawner' si es legal ejecutar ciertas acciones; bloquea de manera inteligente funciones como la alteración, 
      compra o movimiento de defensas estratégicas en medio del asalto de una oleada enemiga, enviando avisos informativos en pantalla al usuario.
   ========================================================================================================
*/