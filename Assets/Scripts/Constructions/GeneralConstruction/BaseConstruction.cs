using System.Collections;
using UnityEngine;

public abstract class BaseConstruction : MonoBehaviour
{
    protected ConstructionData data; //configuración actual
    protected float resistance;
    protected float aditamentResistance;
    protected float damage;
    protected float resourceRate;
    protected float actionVelocity;
    protected float movementSpeed;
    protected float range;
    protected float projectileVelocity;
    protected float cost;

    protected Pooling pooling;
    protected float timeToSpawnAditaments;
    protected Coroutine spawnCoroutine;

    public virtual void Initialize(ConstructionData newData) //recibe la información de la construcción
    {
        data = newData; //la guarda
        timeToSpawnAditaments = data.actionVelocity;
        ApplyStats();
    }

    protected virtual void Awake()
    {
        pooling = FindAnyObjectByType<Pooling>();
    }

    protected virtual void OnDisable()
    {
        StopAllCoroutines();
        spawnCoroutine = null;
    }

    protected virtual void ApplyStats()
    {
        resistance = data.resistance;
        aditamentResistance = data.aditamentResistance;
        damage = data.attackDamage;
        resourceRate = data.resourceRate;
        actionVelocity = data.actionVelocity;
        movementSpeed = data.movementSpeed;
        range = data.range;
        projectileVelocity = data.proyectileVelocity;
        cost = data.cost;

        Debug.Log($"Initialized {data.type} Lv{data.level} ({data.god})");
    }

    public virtual void ResetConstruction()
    {
        CancelInvoke();

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    //Corrutinas

    protected IEnumerator SpawnLoop(System.Action spawnAction)
    {
        while (isActiveAndEnabled)
        {
            spawnAction?.Invoke();
            yield return new WaitForSeconds(timeToSpawnAditaments);
        }
    }

}
