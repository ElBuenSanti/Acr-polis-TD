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

        RefreshButtonVisual();
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
        GameState mainState = GameStateManager.Instance.LastMainState;

        switch (currentState)
        {
            case GameState.Preparation:
                GameStateManager.Instance.ChangeState(GameState.Combat);
                break;

            case GameState.Combat:
                GameStateManager.Instance.ChangeState(GameState.Paused);
                break;

            case GameState.Paused:
                GameStateManager.Instance.ChangeState(GameState.Combat);
                break;

            case GameState.StructureSelected:
            case GameState.BuildingPlacement:
            case GameState.RadialUpgradeOpen:
                if (mainState == GameState.Combat)
                {
                    GameStateManager.Instance.ChangeState(GameState.Paused);
                }
                else
                {
                    GameStateManager.Instance.ChangeState(GameState.Combat);
                }
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
        RefreshButtonVisual();
    }

    // Update the button text based on the main gameplay state
    private void RefreshButtonVisual()
    {
        if (roundControlText == null || GameStateManager.Instance == null)
        {
            return;
        }

        GameState currentState = GameStateManager.Instance.CurrentState;
        GameState mainState = GameStateManager.Instance.LastMainState;

        if (currentState == GameState.BlessingSelection)
        {
            roundControlText.text = "Locked";
            return;
        }

        if (currentState == GameState.Victory)
        {
            roundControlText.text = "Victory";
            return;
        }

        if (currentState == GameState.Defeat)
        {
            roundControlText.text = "Defeat";
            return;
        }

        switch (mainState)
        {
            case GameState.Preparation:
                roundControlText.text = "Start";
                break;

            case GameState.Combat:
                roundControlText.text = "Pause";
                break;

            case GameState.Paused:
                roundControlText.text = "Resume";
                break;

            default:
                roundControlText.text = "Start";
                break;
        }
    }
}