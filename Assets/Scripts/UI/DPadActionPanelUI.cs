using UnityEngine;

public class DPadActionPanelUI : MonoBehaviour
{
    [Header("Highlights")]
    [SerializeField] private DPadActionHighlightUI upHighlight;
    [SerializeField] private DPadActionHighlightUI downHighlight;
    [SerializeField] private DPadActionHighlightUI leftHighlight;
    [SerializeField] private DPadActionHighlightUI rightHighlight;

    [Header("Action Groups")]
    [SerializeField] private CanvasGroup upGroup;
    [SerializeField] private CanvasGroup downGroup;
    [SerializeField] private CanvasGroup leftGroup;
    [SerializeField] private CanvasGroup rightGroup;

    [Header("Settings")]
    [SerializeField] private float highlightTime = 0.18f;
    [SerializeField] private float activeAlpha = 1f;
    [SerializeField] private float disabledAlpha = 0.35f;

    private float upTimer;
    private float downTimer;
    private float leftTimer;
    private float rightTimer;

    private void Update()
    {
        UpdateHighlight(ref upTimer, upHighlight);
        UpdateHighlight(ref downTimer, downHighlight);
        UpdateHighlight(ref leftTimer, leftHighlight);
        UpdateHighlight(ref rightTimer, rightHighlight);

        RefreshAvailability();
    }

    public void FlashUp()
    {
        upTimer = highlightTime;
        SetActive(upHighlight, true);
    }

    public void FlashDown()
    {
        downTimer = highlightTime;
        SetActive(downHighlight, true);
    }

    public void FlashLeft()
    {
        leftTimer = highlightTime;
        SetActive(leftHighlight, true);
    }

    public void FlashRight()
    {
        rightTimer = highlightTime;
        SetActive(rightHighlight, true);
    }

    private void RefreshAvailability()
    {
        bool waveRunning = WaveSpawner.Instance != null && WaveSpawner.Instance.IsWaveRunning();
        bool mapIdle = GameStateController.Instance.currentState == GameState.MapIdle;

        bool canUseGlobal = mapIdle && !waveRunning;

        SetGroup(upGroup, canUseGlobal);
        SetGroup(downGroup, canUseGlobal);
        SetGroup(rightGroup, canUseGlobal);

        //bool hasSelectedStructure =
        //    BuildingManager.Instance != null &&
        //    BuildingManager.Instance.selectedConstruction != null;

        bool hasSelectedStructure =
            FindObjectOfType<GridSelector>() != null &&
            FindObjectOfType<GridSelector>().currentTile != null &&
            FindObjectOfType<GridSelector>().currentTile.IsOccupied;

        SetGroup(leftGroup, canUseGlobal && hasSelectedStructure);
    }

    private void UpdateHighlight(ref float timer, DPadActionHighlightUI highlight)
    {
        if (timer <= 0f)
            return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
            SetActive(highlight, false);
    }

    private void SetActive(DPadActionHighlightUI highlight, bool active)
    {
        if (highlight != null)
            highlight.SetActive(active);
    }

    private void SetGroup(CanvasGroup group, bool active)
    {
        if (group == null)
            return;

        group.alpha = active ? activeAlpha : disabledAlpha;
    }
}