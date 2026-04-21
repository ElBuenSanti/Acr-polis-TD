using System.Collections;
using UnityEngine;

public class Barracks : BaseConstruction
{
    public GameObject soldierPrefab;
    private float timeToSpwanInBetweenSoldiers = 1.5f;
    [SerializeField] private Transform spawnPoint;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public override void Initialize(ConstructionData newData)
    {
        base.Initialize(newData);
        timeToBeDestroyed = 2f;

        /*
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        spawnCoroutine = StartCoroutine(SpawnLoop(SpawnSoldier));
        */

        StartWaveDependentSpawn(SpawnSoldier);
        
    }

    //Funciones

    public override void ResetConstruction()
    {
        base.ResetConstruction();
    }

    void SpawnSoldier()
    {
        StartCoroutine(SpawnSoldiersWithDelay());
    }

    IEnumerator SpawnSoldiersWithDelay()
    {
        for (int i = 0; i < 3; i++)
        {
            if (!WaveSpawner.Instance.IsWaveRunning())
                yield break;
            GameObject newSoldier = pooling.CreateObject(soldierPrefab, transform);

            Vector3 spawnPos = transform.position + Vector3.right * 2f;
            newSoldier.transform.position = spawnPos;

            if (newSoldier.TryGetComponent<HandToHandSoldier>(out var soldier))
            {
                soldier.Initialize(data, this);
            }

            float timer = 0f;
            float wait = timeToSpwanInBetweenSoldiers;

            while (timer < wait)
            {
                if (!WaveSpawner.Instance.IsWaveRunning())
                    yield break; // 

                timer += Time.deltaTime;
                yield return null;
            }
        }
    }

    /*
    IEnumerator SpawnSoldiersWithDelay()
    {
        for (int i = 0; i < 3; i++)
        {
            if (!WaveSpawner.Instance.IsWaveRunning())
            {
                yield break;
            }
            //GameObject newSoldier = pooling.CreateObject(soldierPrefab, transform);
            GameObject newSoldier = pooling.CreateObject(soldierPrefab, transform);

            Vector3 spawnPos = transform.position + Vector3.right * 1f;
            newSoldier.transform.position = spawnPos;

            if (newSoldier.TryGetComponent<HandToHandSoldier>(out var soldier))
            {
                soldier.Initialize(data, this);
            }

            yield return new WaitForSeconds(timeToSpwanInBetweenSoldiers);
        }
    }
    */
}