using System;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    [Header("Initial State")]
    [SerializeField] private GameState initialState = GameState.Preparation;

    public GameState CurrentState { get; private set; }

    public event Action<GameState, GameState> OnStateChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Another GameStateManager already exists in the scene.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        CurrentState = initialState;
    }

    private void Start()
    {
        Debug.Log("Current state: " + CurrentState);
    }

    // Change the current game state and notify listeners
    public bool ChangeState(GameState newState)
    {
        if (newState == CurrentState)
        {
            return false;
        }

        GameState previousState = CurrentState;
        CurrentState = newState;

        Debug.Log("State changed: " + previousState + " -> " + CurrentState);

        OnStateChanged?.Invoke(previousState, CurrentState);
        GameEvents.TriggerEvent(EventNames.StateChanged, CurrentState);

        return true;
    }

    // Check if the game is currently in a specific state
    public bool IsInState(GameState state)
    {
        return CurrentState == state;
    }

    // Check if gameplay actions should be blocked
    public bool IsGameplayBlocked()
    {
        return CurrentState == GameState.Paused ||
               CurrentState == GameState.BlessingSelection ||
               CurrentState == GameState.Victory ||
               CurrentState == GameState.Defeat;
    }
}