using System.Collections;
using UnityEngine;

public class Barracks : BaseConstruction
{
    public GameObject soldierPrefab;
    private float timeToSpwanInBetweenSoldiers = 0.5f;

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

        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        spawnCoroutine = StartCoroutine(SpawnLoop(SpawnSoldier));
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
            GameObject newSoldier = pooling.CreateObject(soldierPrefab, transform);

            if (newSoldier.TryGetComponent<HandToHandSoldier>(out var soldier))
            {
                soldier.Initialize(data, this);
            }

            yield return new WaitForSeconds(timeToSpwanInBetweenSoldiers);
        }
    }
}