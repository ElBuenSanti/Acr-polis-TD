using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Handles the main round control button behavior
public class RoundControlButtonUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button roundControlButton;
    [SerializeField] private TMP_Text roundControlIconText;
    [SerializeField] private TMP_Text roundControlLabelText;

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

    // Update the button icon and optional label based on the main gameplay state
    private void RefreshButtonVisual()
    {
        if (GameStateManager.Instance == null)
        {
            return;
        }

        GameState currentState = GameStateManager.Instance.CurrentState;
        GameState mainState = GameStateManager.Instance.LastMainState;

        if (currentState == GameState.BlessingSelection)
        {
            SetVisual("■", "Locked");
            return;
        }

        if (currentState == GameState.Victory)
        {
            SetVisual("✔", "Victory");
            return;
        }

        if (currentState == GameState.Defeat)
        {
            SetVisual("✖", "Defeat");
            return;
        }

        switch (mainState)
        {
            case GameState.Preparation:
                SetVisual("▶", "Start");
                break;

            case GameState.Combat:
                SetVisual("❚❚", "Pause");
                break;

            case GameState.Paused:
                SetVisual("▶", "Resume");
                break;

            default:
                SetVisual("▶", "Start");
                break;
        }
    }

    // Apply icon and optional label text
    private void SetVisual(string iconText, string labelText)
    {
        if (roundControlIconText != null)
        {
            roundControlIconText.text = iconText;
        }

        if (roundControlLabelText != null)
        {
            roundControlLabelText.text = labelText;
        }
    }
}