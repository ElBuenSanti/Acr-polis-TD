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
    protected Pooling pooling;

    protected float resistance;
    //protected float actionVelocity;
    protected float cost; 
    protected float timeToSpawnAditaments;
    
    protected Coroutine spawnCoroutine;
    protected Coroutine destructionRoutine;
    protected Coroutine waveRoutine;

    protected float timeToBeDestroyed;

    public bool IsDestroyed { get; protected set; }

    public static List<BaseConstruction> AllConstructions = new List<BaseConstruction>();



    //Construction Creation
    public virtual void Initialize(ConstructionData newData) 
    {
        data = newData; 
        SetTeam(Team.Ally);
        //timeToSpawnAditaments = data.actionVelocity;
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




    //Functions

    protected virtual void ApplyStats()
    {
        resistance = data.resistance;
        timeToSpawnAditaments = data.actionVelocity;
        //actionVelocity = data.actionVelocity;
        Debug.Log($"Construcción Inicializada: {data.type} Lv{data.level} ({data.god})");
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

    //Damage and Death
    public void ReceiveDamage(float damage)
    {
        if (IsDestroyed)
        {
            return; 
        }

        resistance -= damage; 

        if (resistance <= 0) 
        {
            IsDestroyed = true; 
            OnDestruction();
            return; 
        }
    }

    public virtual void OnDestruction()
    {
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        if (GameplaySoundPlayer.Instance != null)
            GameplaySoundPlayer.Instance.PlayWallDestroyed();
        destructionRoutine = StartCoroutine(DestructionRoutine(timeToBeDestroyed));
        Die();
    }


    public virtual void Die()
    {
        if (parentTile != null)
        {
            parentTile.SetOccupied(false);
        }

        gameObject.SetActive(false);
    }

    //Wave Routine
    protected void StartWaveDependentSpawn(System.Action spawnAction)
    {
        if (waveRoutine != null)
            StopCoroutine(waveRoutine);

        waveRoutine = StartCoroutine(RunWhileWave(spawnAction));
    }

    //Getter and Setter Functions
    public void SetTile(Tile tile)
    {
        parentTile = tile;
    }

    public Tile GetTile()
    {
        return parentTile;
    }

    //Dinamic Nav Mesh

    void SetupNavObstacle()
    {
        if (!TryGetComponent(out obstacle))
            obstacle = gameObject.AddComponent<NavMeshObstacle>();

        obstacle.shape = NavMeshObstacleShape.Box;
        obstacle.carving = true;
        obstacle.carveOnlyStationary = true;

        obstacle.size = new Vector3(1.5f, 1.5f, 1.5f); // ancho, alto, profundidad
        obstacle.center = Vector3.zero; 
    }

    //For walls
    public virtual float GetResistance()
    {
        return resistance;
    }

    public virtual float GetMaxHealth()
    {
        return data.resistance;
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
            while (!WaveSpawner.Instance.IsWaveRunning())
                yield return null;

            spawnCoroutine = StartCoroutine(SpawnLoop(spawnAction));

            while (WaveSpawner.Instance.IsWaveRunning())
                yield return null;

            if (spawnCoroutine != null)
            {
                StopCoroutine(spawnCoroutine);
                spawnCoroutine = null;
            }
        }
    }
}
