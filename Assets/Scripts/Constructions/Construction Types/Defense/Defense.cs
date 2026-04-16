using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Defense : BaseConstruction
{
    public GameObject arrowPrefab;

    private TargetFinder targetFinder;
    private Transform currentTarget;

    private float distanceToShoot;

    private float aditamentResistance;
    private float damage;
    private float range;
    private float projectileVelocity;

    protected override void Awake()
    {
        base.Awake();
        targetFinder = FindAnyObjectByType<TargetFinder>();
    }

    public override void Initialize(ConstructionData newData)
    {
        base.Initialize(newData);

        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        spawnCoroutine = StartCoroutine(SpawnLoop(SpawnArrows));
    }


    //Funciones
    protected override void ApplyStats()
    {
        base.ApplyStats();
    }

    void SpawnArrows()
    {
        currentTarget = targetFinder.FindTarget<Enemy>(transform);

        if (currentTarget == null)
        {
            return;
        }

        distanceToShoot = Vector3.Distance(transform.position, currentTarget.position);

        if (distanceToShoot > range)
            return;

        GameObject newArrow = pooling.CreateObject(arrowPrefab, transform);

        if (newArrow.TryGetComponent<Arrow>(out var arrow))
        {
            arrow.Initialize(data, transform.position, currentTarget.position, this);
        }
    }

}
