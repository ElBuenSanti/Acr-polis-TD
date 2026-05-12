using System.Collections;
using UnityEngine;

public class Barracks : BaseConstruction
{
    public GameObject soldierPrefab;
    private Coroutine spawnSequence;
    private TargetFinder targetFinder;

    private float timeToSpwanInBetweenSoldiers = 1.5f;

 
    //Barrack Creation
    protected override void Awake()
    {
        base.Awake();
        targetFinder = FindAnyObjectByType<TargetFinder>(); 
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

    //Functions

    public override void ResetConstruction()
    {
        base.ResetConstruction();
    }

    //Soldier Generation
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

    Transform FindNearestEnemy()
    {
        return targetFinder.FindTarget<Enemy>(transform, 100f);
    }



    bool WallBetweenBarracksAndEnemy(Transform enemy)
    {
        float enemyDistance = Vector3.Distance(transform.position, enemy.position);

        BaseConstruction closestWall = null;

        float closestWallDistance = Mathf.Infinity;

        foreach (BaseConstruction construction in BaseConstruction.AllConstructions)
        {
            if (construction == null)
                continue;

            if (construction.Data.type != ConstructionType.Wall)
                continue;

            float wallDistance = Vector3.Distance(transform.position, construction.transform.position);


            if (wallDistance < enemyDistance)
            {
                float distanceToEnemy = Vector3.Distance(enemy.position, construction.transform.position);

                if (distanceToEnemy < closestWallDistance)
                {
                    closestWallDistance = distanceToEnemy;
                    closestWall = construction;
                }
            }
        }

        if (closestWall == null)
        {
            Debug.Log("No hay muralla");
            return false;
        }

        Debug.Log("Muralla más cercana: " + closestWall);
        Debug.Log($"Resistencia muralla: {closestWall.GetResistance()}. Resistencia Max: {closestWall.GetMaxHealth()}");

        if (closestWall.GetResistance() <= (closestWall.GetMaxHealth() / 1.5))
        {
            return false;
        }
        else
        {
            return true;
        }

    }



    //Cooroutines
    IEnumerator SpawnSoldiersWithDelay()
    {
        for (int i = 0; i < 3; i++)
        {
            if (!WaveSpawner.Instance.IsWaveRunning())
                break; 

            GameObject newSoldier = pooling.CreateObject(soldierPrefab, transform);

            if (GameplaySoundPlayer.Instance != null)
                GameplaySoundPlayer.Instance.PlaySoldierSpawn();

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

    
}







/*
bool WallBetweenBarracksAndEnemy(Transform enemy)
{
    float enemyDistance = Vector3.Distance(transform.position, enemy.position);

    foreach (BaseConstruction construction in BaseConstruction.AllConstructions)
    {
        if (construction == null)
            continue;

        if (construction.Data.type != ConstructionType.Wall)
            continue;

        if (construction.GetResistance() < construction.GetMaxHealth() / 2)
            continue;

        float wallDistance = Vector3.Distance(transform.position, construction.transform.position);

        if (wallDistance < enemyDistance)
        {
            return true;
        }
    }

    return false;
}
*/