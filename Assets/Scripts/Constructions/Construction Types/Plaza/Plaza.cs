using UnityEngine;

public class Plaza : BaseConstruction
{
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
        //SetTeam(Team.Ally);

        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        spawnCoroutine = StartCoroutine(SpawnLoop(SpawnWill));
    }

    protected virtual void SpawnWill()
    {
        foreach (var w in data.willObtaied)
        {
            WillManager.Instance.AddMoney(w.type, w.amount);
        }
    }
}
