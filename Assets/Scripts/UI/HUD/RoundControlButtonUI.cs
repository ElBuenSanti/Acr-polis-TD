using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Handles the main round control button behavior
public class RoundControlButtonUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button roundControlButton;
    [SerializeField] private TMP_Text roundControlText;

    private void Start()
    {
        if (roundControlButton != null)
        {
            roundControlButton.onClick.AddListener(OnRoundControlButtonPressed);
        }

        UpdateButtonVisual(GameStateManager.Instance.CurrentState);
    }

    private void OnDestroy()
    {
        if (roundControlButton != null)
        {
            roundControlButton.onClick.RemoveListener(OnRoundControlButtonPressed);
        }
    }

    private void OnEnable()
    {
        GameEvents.StartListening(EventNames.StateChanged, OnStateChanged);
    }

    private void OnDisable()
    {
        GameEvents.StopListening(EventNames.StateChanged, OnStateChanged);
    }

    // Called when the round control button is pressed
    private void OnRoundControlButtonPressed()
    {
        if (GameStateManager.Instance == null)
        {
            Debug.LogWarning("RoundControlButtonUI: GameStateManager instance is missing.");
            return;
        }

        GameState currentState = GameStateManager.Instance.CurrentState;

        switch (currentState)
        {
            case GameState.Preparation:
            case GameState.BuildingPlacement:
            case GameState.StructureSelected:
            case GameState.RadialUpgradeOpen:
                GameStateManager.Instance.ChangeState(GameState.Combat);
                break;

            case GameState.Combat:
                GameStateManager.Instance.ChangeState(GameState.Paused);
                break;

            case GameState.Paused:
                GameStateManager.Instance.ChangeState(GameState.Combat);
                break;

            case GameState.BlessingSelection:
            case GameState.Victory:
            case GameState.Defeat:
                Debug.Log("Round control button is locked in the current state.");
                break;
        }
    }

    // Called when the game state changes
    private void OnStateChanged(object eventData)
    {
        if (eventData is GameState newState)
        {
            UpdateButtonVisual(newState);
        }
    }

    // Update the button text based on the current game state
    private void UpdateButtonVisual(GameState currentState)
    {
        if (roundControlText == null)
        {
            return;
        }

        switch (currentState)
        {
            case GameState.Preparation:
            case GameState.BuildingPlacement:
            case GameState.StructureSelected:
            case GameState.RadialUpgradeOpen:
                roundControlText.text = "Start";
                break;

            case GameState.Combat:
                roundControlText.text = "Pause";
                break;

            case GameState.Paused:
                roundControlText.text = "Resume";
                break;

            case GameState.BlessingSelection:
                roundControlText.text = "Locked";
                break;

            case GameState.Victory:
                roundControlText.text = "Victory";
                break;

            case GameState.Defeat:
                roundControlText.text = "Defeat";
                break;
        }
    }
}