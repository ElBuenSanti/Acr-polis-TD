using UnityEngine;

public class Plaza : BaseConstruction
{

    //Plaza Initialization
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

        StartWaveDependentSpawn(SpawnWill);
    }


    //Resource Generation
    protected virtual void SpawnWill()
    {
        foreach (var w in data.willObtaied)
        {
            WillManager.Instance.AddMoney(w.type, w.amount);
        }
    }
}
