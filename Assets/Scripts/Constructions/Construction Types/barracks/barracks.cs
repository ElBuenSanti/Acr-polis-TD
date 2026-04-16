using System.Collections;
using UnityEngine;

public class Barracks : BaseConstruction
{
    //public Pooling soldierPooling;
    public GameObject soldierPrefab;

    //private float timeToSpawnSoldiers;
    //private Coroutine spawnCoroutine;

    protected override void Awake()
    {
        base.Awake();
    }

    public override void Initialize(ConstructionData newData)
    {
        base.Initialize(newData);

        //timeToSpawnAditaments = data.actionVelocity;

        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        spawnCoroutine = StartCoroutine(SpawnLoop(SpawnSoldier));
    }

    public override void ResetConstruction()
    {
        base.ResetConstruction();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    void SpawnSoldier()
    {
        GameObject newSoldier = pooling.CreateObject(soldierPrefab, transform);

        if (newSoldier.TryGetComponent<HandToHandSoldier>(out var soldier))
        {
            soldier.Initialize(data, this);
        }

    }
}