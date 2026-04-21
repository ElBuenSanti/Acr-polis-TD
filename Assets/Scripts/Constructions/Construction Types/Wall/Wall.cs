using Unity.VisualScripting;
using UnityEngine;

public class Wall : BaseConstruction
{
    private float healAmount;
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
        healAmount = data.aditamentResistance;

        StartWaveDependentSpawn(RecoverResistance);
    }

    protected virtual void RecoverResistance()
    {
        if (IsDestroyed)
        {
            return;
        }

        if(data.resistance == resistance)
        {
            return;
        }
        resistance += healAmount;

        Debug.Log($"Muralla regeneró {healAmount} de vida");
    }
}
