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

        WaveSpawner.Instance.OnWaveIndexChanged += UpdateWaveIndex;
        WaveSpawner.Instance.OnWaveStarted += OnWaveStarted;
        WaveSpawner.Instance.OnWaveEnded += OnWaveEnded;

        isSubscribed = true;
    }

    private void Unsubscribe()
    {
        if (!isSubscribed || WaveSpawner.Instance == null)
            return;

        WaveSpawner.Instance.OnWaveIndexChanged -= UpdateWaveIndex;
        WaveSpawner.Instance.OnWaveStarted -= OnWaveStarted;
        WaveSpawner.Instance.OnWaveEnded -= OnWaveEnded;

        isSubscribed = false;
    }

    private void UpdateCampaignProgress()
    {
        if (waveFillImage == null || totalWaves <= 0)
            return;

        float progress = (float)currentWave / totalWaves;
        waveFillImage.fillAmount = Mathf.Clamp01(progress);
    }

    private void UpdateWaveIndex(int wave, int total)
    {
        currentWave = wave;
        totalWaves = total;

        if (waveText != null)
            waveText.text = GetWaveTitle(currentWave);

        UpdateCampaignProgress();
        UpdateMilestoneVisual();
    }

    private void OnWaveStarted()
    {
        currentWave = WaveSpawner.Instance.GetCurrentWaveNumber();
        totalWaves = WaveSpawner.Instance.GetTotalWaves();

        if (waveText != null)
        {
            waveText.gameObject.SetActive(true);
            waveText.text = GetWaveTitle(currentWave);
        }

        UpdateCampaignProgress();
        UpdateMilestoneVisual();
    }

    private void OnWaveEnded()
    {
        SetConstructionPhase();
    }

    private void SetConstructionPhase()
    {
        UpdateCampaignProgress();

        if (waveText != null)
            waveText.gameObject.SetActive(false);

        if (milestoneText != null)
            milestoneText.text = "Fase de construcción";

        if (waveFillImage != null && currentWave != totalWaves)
            waveFillImage.color = normalColor;
    }

    private void UpdateMilestoneVisual()
    {
        if (waveFillImage == null || milestoneText == null)
            return;

        if (currentWave == totalWaves)
        {
            waveFillImage.color = finalWaveColor;
            milestoneText.text = "BATALLA FINAL";
        }
        else if (currentWave == 15)
        {
            waveFillImage.color = milestoneColor;
            milestoneText.text = "Presagio final";
        }
        else if (currentWave == 10)
        {
            waveFillImage.color = milestoneColor;
            milestoneText.text = "Ira divina";
        }
        else if (currentWave == 5)
        {
            waveFillImage.color = milestoneColor;
            milestoneText.text = "Primer asalto";
        }
        else
        {
            waveFillImage.color = normalColor;
            milestoneText.text = "Defiende la Acrópolis";
        }
    }

    private string GetWaveTitle(int wave)
    {
        if (wave == totalWaves)
            return "OLEADA FINAL";

        if (wave == 15)
            return "OLEADA XV · PRESAGIO FINAL";

        if (wave == 10)
            return "OLEADA X · IRA DIVINA";

        if (wave == 5)
            return "OLEADA V · PRIMER ASALTO";

        return "OLEADA " + wave;
    }
}