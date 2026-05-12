using System;
using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public static WaveSpawner Instance;

    public event Action OnWaveEnded;
    public event Action OnWaveStarted;
    public event Action<float> OnWaveProgressChanged;
    public event Action<int, int> OnWaveIndexChanged;

    public WaveData[] waves;
    public Transform[] spawnPoints;

    private Pooling pooling;
    private GodType currentGod = GodType.Base;

    private int currentWaveIndex;
    private bool waveRunning;

    private void Awake()
    {
        Instance = this;
        pooling = FindAnyObjectByType<Pooling>();
    }

    private void OnEnable()
    {
        Temple.OnGodSelected += SetGod;
        FinalBoss.OnFinalBossDeath += HandleEndingConditions;
        Temple.OnTempleDestruction += HandleEndingConditions;
    }

    private void OnDisable()
    {
        Temple.OnGodSelected -= SetGod;
        FinalBoss.OnFinalBossDeath -= HandleEndingConditions;
        Temple.OnTempleDestruction -= HandleEndingConditions;
    }

    public void StartWave()
    {
        if (waveRunning)
            return;

        if (currentWaveIndex >= waves.Length)
        {
            ShowStatus("No hay más oleadas disponibles");
            return;
        }

        WaveData currentWave = waves[currentWaveIndex];

        if (currentWave.waveType == Wave.FinalBattle && currentGod == GodType.Base)
        {
            ShowStatus("Debes mejorar el templo antes de la oleada final");

            if (GameplaySoundPlayer.Instance != null)
                GameplaySoundPlayer.Instance.PlayInvalidPlacement();

            return;
        }

        waveRunning = true;

        PlayWaveAudio();

        OnWaveIndexChanged?.Invoke(GetCurrentWaveNumber(), GetTotalWaves());
        OnWaveStarted?.Invoke();

        ShowStatus("Oleada iniciada");

        if (currentWave.waveType == Wave.FinalBattle)
        {
            SpawnFinalBoss(currentWave);
            currentWaveIndex++;
        }
        else
        {
            StartCoroutine(RunWave(currentWave));
        }
    }

    public bool IsWaveRunning()
    {
        return waveRunning;
    }

    private void SetGod(GodType god)
    {
        currentGod = god;
        ShowStatus("Spawner recibió dios: " + god);
    }

    private void PlayWaveAudio()
    {
        int waveNumber = GetCurrentWaveNumber();
        int totalWaves = GetTotalWaves();

        if (waveNumber == totalWaves)
        {
            if (GameplaySoundPlayer.Instance != null)
                GameplaySoundPlayer.Instance.PlayFinalWave();

            if (MusicManager.Instance != null)
                MusicManager.Instance.PlayFinalWaveMusic();

            return;
        }

        if (waveNumber == 5 || waveNumber == 10 || waveNumber == 15)
        {
            if (GameplaySoundPlayer.Instance != null)
                GameplaySoundPlayer.Instance.PlayMilestoneWave();

            if (MusicManager.Instance != null)
                MusicManager.Instance.PlayMilestoneMusic();

            return;
        }

        if (GameplaySoundPlayer.Instance != null)
            GameplaySoundPlayer.Instance.PlayWaveStart();

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayCombatMusic();
    }

    private void HandleEndingConditions()
    {
        waveRunning = false;

        if (MusicManager.Instance != null)
            MusicManager.Instance.StopAllMusicAndAmbience();

        OnWaveEnded?.Invoke();
    }

    private void SpawnEnemy(WaveData wave)
    {
        EnemyWaveEntry enemyType = GetRandomEnemyByPercentage(wave);
        Transform spawnPoint = GetRandomSpawnPoint();

        GameObject obj = pooling.CreateObject(enemyType.enemy.enemyPrefab, spawnPoint);
        obj.transform.position = spawnPoint.position;
        obj.transform.rotation = spawnPoint.rotation;

        Enemy enemy = obj.GetComponent<Enemy>();
        enemy.Initialize(enemyType.enemy);
    }

    private void SpawnFinalBoss(WaveData wave)
    {
        ShowStatus("Boss Final");

        EnemyData bossData = wave.GetBossForGod(currentGod);

        if (bossData == null)
        {
            ShowStatus("No hay boss para el dios: " + currentGod);
            return;
        }

        Transform spawnPoint = GetRandomSpawnPoint();

        GameObject obj = pooling.CreateObject(bossData.enemyPrefab, spawnPoint);
        obj.transform.position = spawnPoint.position;
        obj.transform.rotation = spawnPoint.rotation;

        Enemy enemy = obj.GetComponent<Enemy>();
        enemy.Initialize(bossData);

        ShowStatus("Boss spawneado: " + bossData.name + " para dios: " + currentGod);
    }

    private Transform GetRandomSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
            return transform;

        return spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
    }

    private IEnumerator RunWave(WaveData wave)
    {
        float timer = 0f;

        while (timer < wave.waveDuration && waveRunning)
        {
            SpawnEnemy(wave);

            yield return new WaitForSeconds(wave.spawnFrequency);

            timer += wave.spawnFrequency;
            OnWaveProgressChanged?.Invoke(timer / wave.waveDuration);
        }

        currentWaveIndex++;
        waveRunning = false;

        OnWaveProgressChanged?.Invoke(1f);

        ShowStatus("Fin de la Oleada");

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayConstructionMusic();

        OnWaveEnded?.Invoke();
    }

    private EnemyWaveEntry GetRandomEnemyByPercentage(WaveData wave)
    {
        float randomValue = UnityEngine.Random.Range(0f, 100f);
        float currentPercentage = 0f;

        foreach (EnemyWaveEntry enemyEntry in wave.enemies)
        {
            currentPercentage += enemyEntry.percentageInWave;

            if (randomValue <= currentPercentage)
                return enemyEntry;
        }

        return wave.enemies[0];
    }

    private void ShowStatus(string message)
    {
        if (StatusMessageUI.Instance != null)
            StatusMessageUI.Instance.ShowMessage(message);

        Debug.Log(message);
    }

    public int GetCurrentWaveNumber()
    {
        return currentWaveIndex + 1;
    }

    public int GetTotalWaves()
    {
        return waves.Length;
    }
}