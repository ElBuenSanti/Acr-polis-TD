using UnityEngine;

public abstract class BaseConstruction : MonoBehaviour
{
    protected ConstructionData data; //configuración actual
    protected float resistance;
    protected float soldierResistance;
    protected float damage;
    protected float resourceRate;
    protected float actionVelocity;
    protected float movementSpeed;
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
        soldierResistance = data.soldierResistance;
        damage = data.attackDamage;
        resourceRate = data.resourceRate;
        actionVelocity = data.actionVelocity;
        movementSpeed = data.movementSpeed;
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
