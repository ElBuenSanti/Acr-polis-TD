using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameSpeedUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI speedText;

    [SerializeField] private GameObject playIcon;
    [SerializeField] private GameObject x1Icon;
    [SerializeField] private GameObject x2Icon;

    private bool isSpeedTwo;

    private void Update()
    {
        RefreshVisual();
    }

    public void ToggleSpeed()
    {
        if (!WaveSpawner.Instance.IsWaveRunning())
        {
            WaveSpawner.Instance.StartWave();
            isSpeedTwo = false;
            Time.timeScale = 1f;
            return;
        }

        isSpeedTwo = !isSpeedTwo;
        Time.timeScale = isSpeedTwo ? 2f : 1f;
    }

    private void RefreshVisual()
    {
        bool waveRunning = WaveSpawner.Instance.IsWaveRunning();

        if (playIcon != null)
            playIcon.SetActive(!waveRunning);

        if (x1Icon != null)
            x1Icon.SetActive(waveRunning && !isSpeedTwo);

        if (x2Icon != null)
            x2Icon.SetActive(waveRunning && isSpeedTwo);

        if (speedText != null)
        {
            if (!waveRunning)
                speedText.text = "";
            else
                speedText.text = isSpeedTwo ? "x2" : "x1";
        }
    }

    private void OnEnable()
    {
        if (WaveSpawner.Instance != null)
            WaveSpawner.Instance.OnWaveEnded += ResetSpeed;
    }

    private void OnDisable()
    {
        if (WaveSpawner.Instance != null)
            WaveSpawner.Instance.OnWaveEnded -= ResetSpeed;
    }

    private void ResetSpeed()
    {
        isSpeedTwo = false;
        Time.timeScale = 1f;
    }
}