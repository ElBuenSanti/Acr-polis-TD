using UnityEngine;

public class EndGameUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private EndGamePanelUI victoryPanel;
    [SerializeField] private EndGamePanelUI defeatPanel;

    private void OnEnable()
    {
        FinalBoss.OnFinalBossDeath += ShowVictory;
        Temple.OnTempleDestruction += ShowDefeat;
    }

    private void OnDisable()
    {
        FinalBoss.OnFinalBossDeath -= ShowVictory;
        Temple.OnTempleDestruction -= ShowDefeat;
    }

    private void ShowVictory()
    {
        if (victoryPanel != null)
            victoryPanel.Open();
    }

    private void ShowDefeat()
    {
        if (defeatPanel != null)
            defeatPanel.Open();
    }
}