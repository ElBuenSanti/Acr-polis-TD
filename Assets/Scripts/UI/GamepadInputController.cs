using UnityEngine;
using UnityEngine.InputSystem;

public class GamepadInputController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridSelector selector;
    [SerializeField] private RadialMenuUI radialMenu;

    private void Awake()
    {
        if (selector == null)
            selector = FindAnyObjectByType<GridSelector>();

        if (radialMenu == null)
            radialMenu = FindAnyObjectByType<RadialMenuUI>();
    }

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

    public void OnConfirm(InputValue value)
    {
        if (!value.isPressed)
            return;

        GameState state = GameStateController.Instance.currentState;

        if (state == GameState.RadialOpen)
        {
            radialMenu.Confirm();
            return;
        }

        if (state == GameState.MapIdle)
        {
            TrySelectTower();
            return;
        }

        if (state == GameState.PlacingTower)
        {
            BuildingManager.Instance.PlaceBuilding(selector.currentTile);
            GameStateController.Instance.SetState(GameState.MapIdle);
            return;
        }

        if (state == GameState.MovingTower)
        {
            BuildingManager.Instance.PlaceBuilding(selector.currentTile);
            GameStateController.Instance.SetState(GameState.MapIdle);
            return;
        }
    }

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

        if (state == GameState.PlacingTower || state == GameState.MovingTower)
        {
            GameStateController.Instance.SetState(GameState.MapIdle);
            return;
        }

        if (state == GameState.ShopOpen)
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

    private void TrySelectTower()
    {
        if (WaveSpawner.Instance.IsWaveRunning())
            return;

        Tile tile = selector.currentTile;

        if (tile == null)
            return;

        if (!tile.IsOccupied)
            return;

        ConstructionController construction = tile.GetComponentInChildren<ConstructionController>();

        if (construction == null)
            return;

        BuildingManager.Instance.Select(construction);

        if (radialMenu != null)
        {
            radialMenu.Open(construction);
        }

        GameStateController.Instance.SetState(GameState.RadialOpen);
    }
}