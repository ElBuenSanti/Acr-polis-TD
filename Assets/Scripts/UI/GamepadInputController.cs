using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadInputController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridSelector selector;
    [SerializeField] private RadialMenuUI radialMenu;
    [SerializeField] private ShopUI shopUI;

    private void Awake()
    {
        if (selector == null)
            selector = FindAnyObjectByType<GridSelector>();

        if (radialMenu == null)
            radialMenu = FindAnyObjectByType<RadialMenuUI>();

        if (shopUI == null)
            shopUI = FindAnyObjectByType<ShopUI>();
    }

    // ---------------------------
    // MOVEMENT
    // ---------------------------
    public void OnMove(InputValue value)
    {
        Vector2 input = value.Get<Vector2>();

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

        GameState state = GameStateController.Instance.currentState;

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
    }

    // ---------------------------
    // CANCEL (B)
    // ---------------------------
    public void OnCancel(InputValue value)
    {
        if (!value.isPressed)
            return;

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
    }

    // ---------------------------
    // MOVE TOWER (X)
    // ---------------------------
    public void OnMoveTower(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (WaveSpawner.Instance.IsWaveRunning())
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

        if (WaveSpawner.Instance.IsWaveRunning())
            return;

        WaveSpawner.Instance.StartWave();
    }

    // ---------------------------
    // PAUSE (START)
    // ---------------------------
    public void OnPause(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (GameStateController.Instance.currentState == GameState.Paused)
        {
            GameStateController.Instance.SetState(GameState.MapIdle);
        }
        else
        {
            GameStateController.Instance.SetState(GameState.Paused);
        }
    }

    // ---------------------------
    // SHOP (LT / RT)
    // ---------------------------
    public void OnOpenShopLeft(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (WaveSpawner.Instance.IsWaveRunning())
            return;

        shopUI.Toggle();
    }

    public void OnOpenShopRight(InputValue value)
    {
        if (!value.isPressed)
            return;

        if (WaveSpawner.Instance.IsWaveRunning())
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
}