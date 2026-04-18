using System.Collections;
using UnityEngine;

public abstract class BaseConstruction : TeamAssigner
{
    protected ConstructionData data; 
    protected float resistance;
    //protected float resourceRate;
    protected float actionVelocity;
    protected float cost; 

    protected Pooling pooling;
    protected float timeToSpawnAditaments;
    protected Coroutine spawnCoroutine;

    public virtual void Initialize(ConstructionData newData) //recibe la información de la construcción
    {
        data = newData; //la guarda
        SetTeam(Team.Ally);
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

    //Funciones

    protected virtual void ApplyStats()
    {
        resistance = data.resistance;
        actionVelocity = data.actionVelocity;

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
