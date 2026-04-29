using System.Collections;
using UnityEngine;
using System;

public class WaveSpawner : MonoBehaviour
{
    public static WaveSpawner Instance;

    public WaveData[] waves;
    public Transform[] spawnPoints;
    private GodType currentGod = GodType.Base;

    private Pooling pooling;

    private int currentWaveIndex;
    private bool waveRunning;

    public event Action OnWaveEnded;
    public event Action OnWaveStarted;

    void Awake()
    {
        Instance = this;
        pooling = FindAnyObjectByType<Pooling>();
    }

    void OnEnable()
    {
        Temple.OnGodSelected += SetGod;
        FinalBoss.OnFinalBossDeath += HandleEndingConditions;
        Temple.OnTempleDestruction += HandleEndingConditions;
    }

    void OnDisable()
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
            Debug.Log("No hay más oleadas");
            return;
        }


        if (waves[currentWaveIndex].waveType == Wave.FinalBattle)
        {
            if (currentGod == GodType.Base)
            {
                Debug.LogError("No se ha seleccionado un dios");
                return;
            }

            SpawnFinalBoss(waves[currentWaveIndex]);
            currentWaveIndex++;
        }
        else
        {
            StartCoroutine(RunWave(waves[currentWaveIndex]));
        }

        OnWaveStarted?.Invoke();
        waveRunning = true;

    }

    void SetGod(GodType god)
    {
        currentGod = god;
        Debug.Log("Spawner recibió dios: " + god);
    }


    IEnumerator RunWave(WaveData wave)
    {
        waveRunning = true;

        float timer = 0f;

        while (timer < wave.waveDuration && waveRunning)
        {
            SpawnEnemy(wave);

            yield return new WaitForSeconds(wave.spawnFrequency);

            timer += wave.spawnFrequency;
        }
        currentWaveIndex++;
        waveRunning = false;

        Debug.Log("Fin de la oleada");

        OnWaveEnded?.Invoke();
    }

    void SpawnEnemy(WaveData wave)
    {
        var enemyType = wave.enemies[UnityEngine.Random.Range(0, wave.enemies.Count)];

        Transform spawnPoint = GetRandomSpawnPoint();

        //GameObject obj = pooling.CreateObject(enemyType.enemy.enemyPrefab, spawnPoint);

        GameObject obj = pooling.CreateObject(enemyType.enemy.enemyPrefab, spawnPoint);
        obj.transform.position = spawnPoint.position;
        obj.transform.rotation = spawnPoint.rotation;

        Enemy enemy = obj.GetComponent<Enemy>();
        enemy.Initialize(enemyType.enemy);
    }

    void SpawnFinalBoss(WaveData wave)
    {
        Debug.Log("Aquí boss Final");

        EnemyData bossData = wave.GetBossForGod(currentGod);

        if (bossData == null)
        {
            Debug.LogError("No hay boss para el dios: " + currentGod);
            return;
        }

        Transform spawnPoint = GetRandomSpawnPoint();

        GameObject obj = pooling.CreateObject(bossData.enemyPrefab, spawnPoint);

        Enemy enemy = obj.GetComponent<Enemy>();
        enemy.Initialize(bossData);

        Debug.Log("Boss spawneado: " + bossData.name + " para dios: " + currentGod);

    }

    Transform GetRandomSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
            return transform;

        return spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
    }

    public bool IsWaveRunning()
    {
        return waveRunning;
    }

    /*
    void HandleFinalBossDeath()
    {
        waveRunning = false;
        OnWaveEnded?.Invoke(); //finaliza la oleada
    }
    */

    void HandleEndingConditions()
    {
        waveRunning = false;
        OnWaveEnded?.Invoke();
    }
}