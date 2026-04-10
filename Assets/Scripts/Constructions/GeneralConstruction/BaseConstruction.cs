using UnityEngine;

public abstract class BaseConstruction : MonoBehaviour
{
    protected ConstructionData data; //configuración actual
    protected float resistance;
    protected float damage;
    protected float resourceRate;
    protected float actionVelocity;
    protected float range;
    protected float projectileVelocity;
    protected float cost;

    public virtual void Initialize(ConstructionData newData) //recibe la información de la construcción
    {
        data = newData; //la guarda
        ApplyStats();
    }

    protected virtual void ApplyStats()
    {
        resistance = data.resistance;
        damage = data.damage;
        resourceRate = data.resourceRate;
        actionVelocity = data.actionVelocity;
        range = data.Range;
        projectileVelocity = data.proyectileVelocity;
        cost = data.cost;

        Debug.Log($"Initialized {data.type} Lv{data.level} ({data.god})");
    }

    public virtual void ResetConstruction()
    {
        CancelInvoke();
        StopAllCoroutines();
    }
}
