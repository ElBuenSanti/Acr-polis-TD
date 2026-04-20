using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public static WaveSpawner Instance;

    public WaveData[] waves;
    public Transform[] spawnPoints;

    private Pooling pooling;

    private int currentWaveIndex;
    private bool waveRunning;

    void Awake()
    {
        Instance = this;
        pooling = FindAnyObjectByType<Pooling>();
    }

    public void StartWave()
    {
        if (waveRunning)
            return;

        if (currentWaveIndex >= waves.Length)
        {
            Debug.Log("No hay más oleadas");
            return;
        }

        StartCoroutine(RunWave(waves[currentWaveIndex]));
    }

    IEnumerator RunWave(WaveData wave)
    {
        waveRunning = true;

        float timer = 0f;

        while (timer < wave.waveDuration)
        {
            SpawnEnemy(wave);

            yield return new WaitForSeconds(wave.spawnFrequency);

            timer += wave.spawnFrequency;
        }

        currentWaveIndex++;
        waveRunning = false;
        Debug.Log("Fin de la oleada");
    }

    void SpawnEnemy(WaveData wave)
    {
        var enemyType = wave.enemies[Random.Range(0, wave.enemies.Count)];

        Transform spawnPoint = GetRandomSpawnPoint();

        GameObject obj = pooling.CreateObject(enemyType.enemy.enemyPrefab, spawnPoint);

        Enemy enemy = obj.GetComponent<Enemy>();
        enemy.Initialize(enemyType.enemy);
    }

    Transform GetRandomSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
            return transform;

        return spawnPoints[Random.Range(0, spawnPoints.Length)];
    }

    public bool IsWaveRunning()
    {
        return waveRunning;
    }
}