using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveHUDUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image waveFillImage;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI milestoneText;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.green;
    [SerializeField] private Color milestoneColor = Color.yellow;
    [SerializeField] private Color finalWaveColor = Color.red;

    private int currentWave;
    private int totalWaves;
    private bool isSubscribed;

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => WaveSpawner.Instance != null);

        Subscribe();

        currentWave = WaveSpawner.Instance.GetCurrentWaveNumber();
        totalWaves = WaveSpawner.Instance.GetTotalWaves();

        SetConstructionPhase();
    }

    private void OnDestroy()
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        if (isSubscribed)
            return;

        WaveSpawner.Instance.OnWaveProgressChanged += UpdateProgress;
        WaveSpawner.Instance.OnWaveIndexChanged += UpdateWaveIndex;
        WaveSpawner.Instance.OnWaveStarted += OnWaveStarted;
        WaveSpawner.Instance.OnWaveEnded += OnWaveEnded;

        isSubscribed = true;
    }

    private void Unsubscribe()
    {
        if (!isSubscribed || WaveSpawner.Instance == null)
            return;

        WaveSpawner.Instance.OnWaveProgressChanged -= UpdateProgress;
        WaveSpawner.Instance.OnWaveIndexChanged -= UpdateWaveIndex;
        WaveSpawner.Instance.OnWaveStarted -= OnWaveStarted;
        WaveSpawner.Instance.OnWaveEnded -= OnWaveEnded;

        isSubscribed = false;
    }

    private void UpdateProgress(float value)
    {
        if (waveFillImage != null)
            waveFillImage.fillAmount = Mathf.Clamp01(value);
    }

    private void UpdateWaveIndex(int wave, int total)
    {
        currentWave = wave;
        totalWaves = total;

        if (waveText != null)
            waveText.text = "Oleada " + currentWave + " / " + totalWaves;

        UpdateMilestoneVisual();
    }

    private void OnWaveStarted()
    {
        UpdateProgress(0f);

        if (waveText != null)
            waveText.gameObject.SetActive(true);

        UpdateWaveIndex(
            WaveSpawner.Instance.GetCurrentWaveNumber(),
            WaveSpawner.Instance.GetTotalWaves()
        );
    }

    private void OnWaveEnded()
    {
        SetConstructionPhase();
    }

    private void SetConstructionPhase()
    {
        UpdateProgress(0f);

        if (waveText != null)
            waveText.gameObject.SetActive(false);

        if (milestoneText != null)
            milestoneText.text = "Fase de construcción";

        if (waveFillImage != null)
            waveFillImage.color = normalColor;
    }

    private void UpdateMilestoneVisual()
    {
        if (waveFillImage == null || milestoneText == null)
            return;

        if (currentWave == totalWaves)
        {
            waveFillImage.color = finalWaveColor;
            milestoneText.text = "Batalla final";
        }
        else if (currentWave == 5 || currentWave == 10 || currentWave == 15)
        {
            waveFillImage.color = milestoneColor;
            milestoneText.text = "Oleada especial";
        }
        else
        {
            waveFillImage.color = normalColor;
            milestoneText.text = "Defiende la Acrópolis";
        }
    }
}