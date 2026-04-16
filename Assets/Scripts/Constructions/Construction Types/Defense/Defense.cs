using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Defense : BaseConstruction
{
    public GameObject arrowPrefab;

    private TargetFinder targetFinder;
    private Transform currentTarget;


    protected override void Awake()
    {
        base.Awake();
        targetFinder = FindAnyObjectByType<TargetFinder>();
    }

    public override void Initialize(ConstructionData newData)
    {
        base.Initialize(newData);

        timeToSpawnAditaments = data.actionVelocity;
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        spawnCoroutine = StartCoroutine(SpawnLoop(SpawnArrows));
    }

    void SpawnArrows()
    {

        currentTarget = targetFinder.FindTarget<Enemy>(transform);

        if (currentTarget == null)
        {
            return;
        }

        float distance = Vector3.Distance(transform.position, currentTarget.position);

        if (distance > range)
            return;

        GameObject newArrow = pooling.CreateObject(arrowPrefab, transform);

        if (newArrow.TryGetComponent<Arrow>(out var arrow))
        {
            arrow.Initialize(data, transform.position, currentTarget.position, this);
        }
    }

}
