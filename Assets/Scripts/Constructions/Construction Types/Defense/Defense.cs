using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Defense : BaseConstruction
{
    public GameObject arrowPrefab;
    private TargetFinder targetFinder;
    private Transform currentTarget;

    private float range;


    //Defense Initialization
    protected override void Awake()
    {
        base.Awake();
        targetFinder = FindAnyObjectByType<TargetFinder>();
    }

    public override void Initialize(ConstructionData newData)
    {
        base.Initialize(newData);
        timeToBeDestroyed = 2f;

        StartWaveDependentSpawn(SpawnArrows);
    }


    //Functions
    protected override void ApplyStats()
    {
        base.ApplyStats();
        range = data.range;
    }

    //Arrow Generation
    void SpawnArrows()
    {
        currentTarget = targetFinder.FindTarget<Enemy>(transform, range);

        if (currentTarget == null)
        {
            return;
        }

        Vector3 spawnPos = transform.position + Vector3.up * 1.5f;

        GameObject newArrow = pooling.CreateObject(arrowPrefab, transform);

        newArrow.transform.position = spawnPos;

        if (newArrow.TryGetComponent<Arrow>(out var arrow))
        {
            arrow.Initialize(data, spawnPos, currentTarget.position, this);
        }
    }

}
