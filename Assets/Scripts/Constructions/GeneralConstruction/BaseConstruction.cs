using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseConstruction : TeamAssigner, IDamageable
{
    protected ConstructionData data;
    public ConstructionData Data => data;
    private NavMeshObstacle obstacle;

    protected Tile parentTile;

    protected float resistance;
    //protected float resourceRate;
    protected float actionVelocity;
    protected float cost; 

    protected Pooling pooling;
    protected float timeToSpawnAditaments;
    
    protected Coroutine spawnCoroutine;
    protected Coroutine destructionRoutine;
    protected Coroutine waveRoutine;

    protected float timeToBeDestroyed;

    public bool IsDestroyed { get; protected set; }

    public static List<BaseConstruction> AllConstructions = new List<BaseConstruction>();

    public virtual void Initialize(ConstructionData newData) //recibe la información de la construcción
    {
        data = newData; //la guarda
        SetTeam(Team.Ally);
        timeToSpawnAditaments = data.actionVelocity;
        StopAllCoroutines();

        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        ApplyStats();
        SetupNavObstacle();
    }

    protected virtual void Awake()
    {
        pooling = FindAnyObjectByType<Pooling>();
    }

    protected virtual void OnEnable()
    {
        AllConstructions.Add(this);
        IsDestroyed = false;

        if (obstacle != null)
            obstacle.enabled = true;
    }

    protected virtual void OnDisable()
    {
        AllConstructions.Remove(this);
        StopAllCoroutines();
        spawnCoroutine = null;

        if (obstacle != null)
            obstacle.enabled = false;
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

    public void ReceiveDamage(float damage)
    {
        if (IsDestroyed)
        {
            return; //si ya esta destruida, ya no aplicarle daño
        }

        resistance -= damage; //se le quita vida

        if (resistance <= 0) //si llega a 0
        {
            IsDestroyed = true; //ya esta muerto
            OnDestruction();
            return; //evitar que se actuive ondamage
        }
    }


    public virtual void OnDestruction()
    {
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        destructionRoutine = StartCoroutine(DestructionRoutine(timeToBeDestroyed));
        Die();
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

    protected IEnumerator DestructionRoutine(float deathTime)
    {
        yield return new WaitForSeconds(deathTime);

        Die();
        destructionRoutine = null;
    }

    protected IEnumerator RunWhileWave(System.Action spawnAction)
    {
        while (true)
        {
            // esperar inicio de wave
            while (!WaveSpawner.Instance.IsWaveRunning())
                yield return null;

            // empezar spawn
            spawnCoroutine = StartCoroutine(SpawnLoop(spawnAction));

            // esperar fin de wave
            while (WaveSpawner.Instance.IsWaveRunning())
                yield return null;

            // detener spawn
            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
                spawnCoroutine = null;
            }
        }
    }

    protected void StartWaveDependentSpawn(System.Action spawnAction)
    {
        if (waveRoutine != null)
            StopCoroutine(waveRoutine);

        waveRoutine = StartCoroutine(RunWhileWave(spawnAction));
    }

    public virtual void Die()
    {
        if (parentTile != null)
        {
            parentTile.SetOccupied(false);
        }

        gameObject.SetActive(false); 
    }

    public void SetTile(Tile tile)
    {
        parentTile = tile;
    }

    void SetupNavObstacle()
    {
        if (!TryGetComponent(out obstacle))
            obstacle = gameObject.AddComponent<NavMeshObstacle>();

        obstacle.shape = NavMeshObstacleShape.Box;
        obstacle.carving = true;
        obstacle.carveOnlyStationary = true;

        obstacle.size = new Vector3(1.5f, 1.5f, 1.5f); // ancho, alto, profundidad
        obstacle.center = Vector3.zero; // puedes ajustarlo si está desalineado
    }

    public Tile GetTile()
    {
        return parentTile;
    }

    protected void StopWaveDependentSpawn()
    {
        if (waveRoutine != null)
        {
            StopCoroutine(waveRoutine);
            waveRoutine = null;
        }

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

}
