using System.Collections;
using UnityEngine;
using System;

public class WaveSpawner : MonoBehaviour
{
    public static WaveSpawner Instance;

    public event Action OnWaveEnded;
    public event Action OnWaveStarted;

    public WaveData[] waves;
    public Transform[] spawnPoints;
    private Pooling pooling;

    private GodType currentGod = GodType.Base;

    private int currentWaveIndex;
    private bool waveRunning;



    void Awake()
    {
        Instance = this;
        pooling = FindAnyObjectByType<Pooling>();
    }

    void OnEnable()
    {
        Temple.OnGodSelected += SetGod; //Listens to the selected god of the temple
        FinalBoss.OnFinalBossDeath += HandleEndingConditions; //Listens when the final boss died
        Temple.OnTempleDestruction += HandleEndingConditions; //Listens when the temple is destroyed
    }

    void OnDisable()
    {
        Temple.OnGodSelected -= SetGod;
        FinalBoss.OnFinalBossDeath -= HandleEndingConditions;
        Temple.OnTempleDestruction -= HandleEndingConditions;
    }

    //Functions

    void SetGod(GodType god)
    {
        currentGod = god;
        ShowStatus("Spawner recibió dios: " + god);
    }

    //Wave manager
    public void StartWave()
    {
        if (waveRunning)
            return;

        if (currentWaveIndex >= waves.Length)
        {
            Debug.Log("No hay más oleadas disponibles");
            return;
        }


        if (waves[currentWaveIndex].waveType == Wave.FinalBattle)
        {
            if (currentGod == GodType.Base)
            {
                Debug.LogError("No se ha seleccionado un camino divino, si desea pasar a la oleada final, debe mejorar el templo");
                ShowStatus("No se ha seleccionado un camino divino, si desea pasar a la oleada final, debe mejorar el templo");

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

    public bool IsWaveRunning()
    {
        return waveRunning;
    }

    void HandleEndingConditions()
    {
        waveRunning = false;
        OnWaveEnded?.Invoke();
    }



    //Enemy / Final Boss generator
    void SpawnEnemy(WaveData wave)
    {
        //var enemyType = wave.enemies[UnityEngine.Random.Range(0, wave.enemies.Count)];
        var enemyType = GetRandomEnemyByPercentage(wave);

        Transform spawnPoint = GetRandomSpawnPoint();

        GameObject obj = pooling.CreateObject(enemyType.enemy.enemyPrefab, spawnPoint);
        obj.transform.position = spawnPoint.position;
        obj.transform.rotation = spawnPoint.rotation;

        Enemy enemy = obj.GetComponent<Enemy>();
        enemy.Initialize(enemyType.enemy);
    }

    void SpawnFinalBoss(WaveData wave)
    {
        ShowStatus("Boss Final");

        EnemyData bossData = wave.GetBossForGod(currentGod);

        if (bossData == null)
        {
            Debug.LogError("No hay boss para el dios: " + currentGod);
            ShowStatus("No hay boss para el dios: " + currentGod);
            return;
        }

        Transform spawnPoint = GetRandomSpawnPoint();

        GameObject obj = pooling.CreateObject(bossData.enemyPrefab, spawnPoint);

        Enemy enemy = obj.GetComponent<Enemy>();
        enemy.Initialize(bossData);

        ShowStatus("Boss spawneado: " + bossData.name + " para dios: " + currentGod);

    }

    Transform GetRandomSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
            return transform;

        return spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
    }


    //Cooroutine 
    IEnumerator RunWave(WaveData wave)//PARA BARRA DE OLEADA
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

        ShowStatus("Fin de la Oleada");

        OnWaveEnded?.Invoke();
    }

    EnemyWaveEntry GetRandomEnemyByPercentage(WaveData wave)
    {
        float randomValue = UnityEngine.Random.Range(0f, 100f);

        float currentPercentage = 0f;

        foreach (var enemyEntry in wave.enemies)
        {
            currentPercentage += enemyEntry.percentageInWave;

            if (randomValue <= currentPercentage)
            {
                return enemyEntry;
            }
        }

        return wave.enemies[0];
    }


    private void ShowStatus(string message)
    {
        if (StatusMessageUI.Instance != null)
            StatusMessageUI.Instance.ShowMessage(message);

        Debug.Log(message);
    }
}