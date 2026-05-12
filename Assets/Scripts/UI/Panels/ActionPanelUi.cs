using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ActionPanelUI : MonoBehaviour
{
    [Header("Move Button")]
    [SerializeField] private CanvasGroup moveButtonGroup;
    [SerializeField] private TextMeshProUGUI moveText;
    [SerializeField] private Image xIcon;

    [Header("Visual")]
    [SerializeField] private float activeAlpha = 1f;
    [SerializeField] private float inactiveAlpha = 0.35f;

    private void Update()
    {
        RefreshMoveButton();
    }

    private void RefreshMoveButton()
    {
        bool hasSelection = BuildingManager.Instance != null &&
                            BuildingManager.Instance.selectedConstruction != null;

        bool waveRunning = WaveSpawner.Instance != null &&
                           WaveSpawner.Instance.IsWaveRunning();

        bool isMoving = GameStateController.Instance.currentState == GameState.MovingTower;

        if (moveButtonGroup == null)
            return;

        if (isMoving)
        {
            moveButtonGroup.alpha = activeAlpha;

            if (moveText != null)
                moveText.text = "Mover";

            return;
        }

        if (hasSelection && !waveRunning)
        {
            moveButtonGroup.alpha = activeAlpha;

            if (moveText != null)
                moveText.text = "Mover";
        }
        else
        {
            moveButtonGroup.alpha = inactiveAlpha;

            if (moveText != null)
                moveText.text = "Mover";
        }
    }
}