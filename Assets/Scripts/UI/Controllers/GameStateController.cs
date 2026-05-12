using UnityEngine;

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
    public static GameStateController Instance;

    public GameState currentState;

    private GridSelector selector;

    void Awake()
    {
        Instance = this;
        selector = FindAnyObjectByType<GridSelector>();
    }

    void Start()
    {
        SetState(GameState.MapIdle);
    }

    public void SetState(GameState newState)
    {
        currentState = newState;
        Debug.Log("Estado: " + newState);
    }

    public bool IsBusy()
    {
        return currentState != GameState.MapIdle;
    }
}