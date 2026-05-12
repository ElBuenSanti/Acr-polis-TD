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

    private void Awake()
    {
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

    // ---------------------------
    // MOVEMENT
    // ---------------------------
    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();

        if (GameStateController.Instance.currentState == GameState.BlessingSelection)
        {
            if (blessingChoiceUI != null)
                blessingChoiceUI.Move(input);

            return;
        }

        if (GameStateController.Instance.currentState == GameState.RadialOpen)
        {
            radialMenu.ReadStick(input);
            return;
        }

        if (GameStateController.Instance.currentState == GameState.MapIdle ||
            GameStateController.Instance.currentState == GameState.PlacingTower ||
            GameStateController.Instance.currentState == GameState.MovingTower)
        {
            selector.Move(input);
        }
    }

    // ---------------------------
    // CONFIRM (A)
    // ---------------------------
    public void OnConfirm(InputValue value)
    {
        
        if (!value.isPressed)
            return;

        if (uiSoundPlayer != null)
            uiSoundPlayer.PlayConfirm();

        GameState state = GameStateController.Instance.currentState;

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
            GameStateController.Instance.SetState(GameState.MapIdle);
            return;
        }

        // MOVING
        if (state == GameState.MovingTower)
        {
            BuildingManager.Instance.PlaceBuilding(selector.currentTile);
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
    public void OnCancel(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (uiSoundPlayer != null)
            uiSoundPlayer.PlayCancel();

        GameState state = GameStateController.Instance.currentState;

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
    public void OnMoveTower(InputValue value)
    {
        if (!value.isPressed)
            return;

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
    public void OnStartWave(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (GameStateController.Instance.currentState != GameState.MapIdle)
            return;

        if (gameSpeedUI != null)
        {
            gameSpeedUI.ToggleSpeed();
        }
    }

    // ---------------------------
    // PAUSE (START)
    // ---------------------------
    public void OnPause(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (pauseMenuUI != null)
        {
            pauseMenuUI.Toggle();
        }
    }




    // ---------------------------
    // SHOP (LT / RT)
    // ---------------------------
    public void OnOpenShopLeft(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (IsWaveBlockingAction())
            return;

        shopUI.Toggle();
    }

    public void OnOpenShopRight(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (IsWaveBlockingAction())
            return;

        shopUI.Toggle();
    }

    // ---------------------------
    // SHOP NAVIGATION (LB / RB)
    // ---------------------------
    public void OnShopLeft(InputValue value)
    {
        if (!value.isPressed)
            return;

        shopUI.MoveLeft();
    }

    public void OnShopRight(InputValue value)
    {
        if (!value.isPressed)
            return;

        shopUI.MoveRight();
    }

    // ---------------------------
    // SELECT TOWER
    // ---------------------------
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


        BaseConstruction baseConstruction = construction.GetComponent<BaseConstruction>();

        if (baseConstruction != null &&
            baseConstruction.Data != null &&
            baseConstruction.Data.type == ConstructionType.Temple &&
            baseConstruction.Data.god == GodType.Base)
        {
            BuildingManager.Instance.Select(construction);

            if (blessingChoiceUI != null)
                blessingChoiceUI.Open(construction);

            return;
        }

        if (radialMenu != null)
        {
            radialMenu.Open(construction);
        }

        GameStateController.Instance.SetState(GameState.RadialOpen);
    }

    private ConstructionController FindConstructionOnTile(Tile tile)
    {
        foreach (BaseConstruction construction in BaseConstruction.AllConstructions)
        {
            if (construction == null)
                continue;

            if (construction.GetTile() == tile)
            {
                return construction.GetComponent<ConstructionController>();
            }
        }

        return null;
    }

    public void OnDpad(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();

        if (input.magnitude < 0.4f)
        {
            dpadInUse = false;
            return;
        }

        if (dpadInUse)
            return;

        dpadInUse = true;

        if (GameStateController.Instance.currentState != GameState.MapIdle)
            return;

        if (IsWaveBlockingAction())
            return;

        if (input.y > 0.5f)
        {
            if (dpadPanelUI != null)
                dpadPanelUI.FlashUp();

            RepairWallShortcut();
        }
        else if (input.y < -0.5f)
        {
            if (dpadPanelUI != null)
                dpadPanelUI.FlashDown();

            BuildWallShortcut();
        }
        else if (input.x < -0.5f)
        {
            if (dpadPanelUI != null)
                dpadPanelUI.FlashLeft();

            MoveSelectedStructureShortcut();
        }
        else if (input.x > 0.5f)
        {
            if (dpadPanelUI != null)
                dpadPanelUI.FlashRight();

            FocusTempleShortcut();
        }
    }

    private bool IsWaveBlockingAction()
    {
        if (!WaveSpawner.Instance.IsWaveRunning())
            return false;

        if (StatusMessageUI.Instance != null)
            StatusMessageUI.Instance.ShowMessage("No puedes hacer esto durante la oleada");

        return true;
    }

    private void BuildWallShortcut()
    {
        BuildingManager.Instance.SetConstructionIndex(wallConstructionIndex);
        GameStateController.Instance.SetState(GameState.PlacingTower);

        if (StatusMessageUI.Instance != null)
            StatusMessageUI.Instance.ShowMessage("Construir muro");
    }

    private void RepairWallShortcut()
    {
        Wall wall = FindAnyObjectByType<Wall>();

        if (wall == null)
        {
            StatusMessageUI.Instance.ShowMessage("No hay muro para reparar");
            return;
        }

        wall.RepairGroup();
        StatusMessageUI.Instance.ShowMessage("Reparando muro");
    }

    private void MoveSelectedStructureShortcut()
    {
        Tile tile = selector.currentTile;

        if (tile == null || !tile.IsOccupied)
        {
            StatusMessageUI.Instance.ShowMessage("Colócate sobre una estructura para moverla");
            return;
        }

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

        if (baseConstruction.Data.god == GodType.Base)
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

        if (radialMenu != null)
            radialMenu.Open(temple);

        GameStateController.Instance.SetState(GameState.RadialOpen);
    }

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
