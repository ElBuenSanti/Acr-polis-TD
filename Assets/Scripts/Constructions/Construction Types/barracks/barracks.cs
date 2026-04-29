using System.Collections;
using UnityEngine;

public class Barracks : BaseConstruction
{
    public GameObject soldierPrefab;
    private float timeToSpwanInBetweenSoldiers = 1.5f;
    private bool waveSpawnStarted;

    private Coroutine spawnSequence;

    private TargetFinder targetFinder; //

    protected override void Awake()
    {
        base.Awake();
        targetFinder = FindAnyObjectByType<TargetFinder>(); //
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public override void Initialize(ConstructionData newData)
    {
        base.Initialize(newData);
        timeToBeDestroyed = 2f;

        StartWaveDependentSpawn(SpawnSoldier);
        
    }

    //Funciones

    public override void ResetConstruction()
    {
        base.ResetConstruction();
    }

    void SpawnSoldier()
    {
        if (spawnSequence != null) return;

        
        Transform nearestEnemy = FindNearestEnemy();

        if (nearestEnemy == null)
            return;

        if (WallBetweenBarracksAndEnemy(nearestEnemy))
            return;
       
        spawnSequence = StartCoroutine(SpawnSoldiersWithDelay());
    }

    IEnumerator SpawnSoldiersWithDelay()
    {
        for (int i = 0; i < 3; i++)
        {
            if (!WaveSpawner.Instance.IsWaveRunning())
                break; 

            GameObject newSoldier = pooling.CreateObject(soldierPrefab, transform);

            Vector3 spawnPos = transform.position + transform.right * 2f; 
            newSoldier.transform.position = spawnPos;

            if (newSoldier.TryGetComponent<HandToHandSoldier>(out var soldier))
            {
                soldier.Initialize(data, this);
            }

            yield return new WaitForSeconds(timeToSpwanInBetweenSoldiers); 
        }

        spawnSequence = null; 
    }

    Transform FindNearestEnemy()
    {
        return targetFinder.FindTarget<Enemy>(transform, 100f);
    }

    bool WallBetweenBarracksAndEnemy(Transform enemy)
    {
        float enemyDistance = Vector3.Distance(transform.position, enemy.position);

        foreach (BaseConstruction construction in BaseConstruction.AllConstructions)
        {
            if (construction == null)
                continue;

            if (construction.Data.type != ConstructionType.Wall)
                continue;

            float wallDistance = Vector3.Distance(transform.position, construction.transform.position);

            if (wallDistance < enemyDistance)
            {
                return true;
            }
        }

        return false;
    }

    /*
    IEnumerator SpawnSoldiersWithDelay()
    {
        for (int i = 0; i < 3; i++)
        {
            if (!WaveSpawner.Instance.IsWaveRunning())
                yield break;
           

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
    */

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