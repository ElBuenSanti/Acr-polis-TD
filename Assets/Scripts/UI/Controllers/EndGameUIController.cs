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
        if (MusicManager.Instance != null)
            MusicManager.Instance.StopAllMusicAndAmbience();

        if (GameplaySoundPlayer.Instance != null)
            GameplaySoundPlayer.Instance.PlayVictory();

        if (victoryPanel != null)
            victoryPanel.Open();
    }

    private void ShowDefeat()
    {
        if (MusicManager.Instance != null)
            MusicManager.Instance.StopAllMusicAndAmbience();

        if (GameplaySoundPlayer.Instance != null)
            GameplaySoundPlayer.Instance.PlayDefeat();

        if (defeatPanel != null)
            defeatPanel.Open();
    }
}