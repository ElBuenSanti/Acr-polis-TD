using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Defense : BaseConstruction
{
    public GameObject arrowPrefab;

    private TargetFinder targetFinder;
    private Transform currentTarget;

    private float distanceToShoot;

    private float range;

    protected override void Awake()
    {
        base.Awake();
        targetFinder = FindAnyObjectByType<TargetFinder>();
    }

    public override void Initialize(ConstructionData newData)
    {
        base.Initialize(newData);
        timeToBeDestroyed = 2f;

        /*
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);
        */

        //spawnCoroutine = StartCoroutine(SpawnLoop(SpawnArrows));

        StartWaveDependentSpawn(SpawnArrows);
    }


    //Funciones
    protected override void ApplyStats()
    {
        base.ApplyStats();
        range = data.range;
    }


    void SpawnArrows()
    {
        //Debug.Log("AllUnits count: " + NavMeshAgentBehaviour.AllUnits.Count);
        currentTarget = targetFinder.FindTarget<Enemy>(transform, range);

        if (currentTarget == null)
        {
            return;
        }

        distanceToShoot = Vector3.Distance(transform.position, currentTarget.position);
        /*
        if (distanceToShoot > range)
            return;
        */

        GameObject newArrow = pooling.CreateObject(arrowPrefab, transform);

        if (newArrow.TryGetComponent<Arrow>(out var arrow))
        {
            arrow.Initialize(data, transform.position, currentTarget.position, this);
        }
    }

}
