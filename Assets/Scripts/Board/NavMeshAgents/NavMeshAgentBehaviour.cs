using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class NavMeshAgentBehaviour : TeamAssigner, IDamageable
{
    protected NavMeshAgent agent;
    protected TargetFinder targetFinder;

    protected Coroutine stunCoroutine;
    protected Coroutine disappearRoutineCoroutine;

    public static List<NavMeshAgentBehaviour> AllUnits = new List<NavMeshAgentBehaviour>();

    protected float resistance;
    protected float attackDamage;
    protected float movementSpeed;

    protected bool waveEnded;

    public bool IsDead { get; protected set; }
    public bool IsStunned { get; protected set; }


    //Nav Mesh Agents Creation
    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.radius = 0.8f;
        targetFinder = FindAnyObjectByType<TargetFinder>();
    }

    protected virtual void OnEnable()
    {
        AllUnits.Add(this);
        waveEnded = false;

        if (WaveSpawner.Instance != null)
            WaveSpawner.Instance.OnWaveEnded += HandleWaveEnd;

        IsDead = false; 
        IsStunned = false; 

        if (agent != null)
        {
            agent.isStopped = false; 
            agent.ResetPath();
        }

    }

    protected virtual void OnDisable()
    {
        AllUnits.Remove(this);

        if (WaveSpawner.Instance != null)
            WaveSpawner.Instance.OnWaveEnded -= HandleWaveEnd;

        StopAllCoroutines();
    }

    //Functions

    //Movement
    protected virtual void MoveTo(Vector3 destination)
    {
        if (agent == null || IsDead || IsStunned) 
            return;

        agent.SetDestination(destination);
    }
            
   
    //Damage and deactivation
    public virtual void ReceiveDamage(float damage)
    {
        if (IsDead)
        {
            return; 
        }

        resistance -= damage; 

        if (resistance <= 0) 
        {
            IsDead = true; 
            OnDeath();
            return; 
        }
        OnDamage();
    }

    protected virtual void OnDamage()
    {
        if (IsDead)
            return;

        IsStunned = true;

        if (agent != null)
            agent.isStopped = true;

        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);

        stunCoroutine = StartCoroutine(StunnedRoutine(GetStunTime()));
    }

    protected virtual void OnDeath()
    {
        PreparingAgentToDisappear();

        disappearRoutineCoroutine = StartCoroutine(DisappearRoutine(GetDeathTime()));
    }

    protected virtual void PreparingAgentToDisappear()
    {
        if (agent != null)
            agent.isStopped = true;

        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);

        if (disappearRoutineCoroutine != null)
            StopCoroutine(disappearRoutineCoroutine);
    }

    protected virtual void Disappear()
    {
        gameObject.SetActive(false); 
    }

    //Get Time of...

    protected virtual float GetStunTime()
    {
        return 0;
    }

    protected virtual float GetDeathTime()
    {
        return 0;
    }

    protected virtual float GetWinningTime()
    {
        return 0;
    }

    protected virtual float GetLoosingTime()
    {
        return 0;
    }

    //When wave ends, do...
    protected virtual void HandleWaveEnd()
    {
        if (IsDead) return;
        waveEnded = true;
    }

    protected virtual void OnWinningWave()
    {
        PreparingAgentToDisappear();
        disappearRoutineCoroutine = StartCoroutine(DisappearRoutine(GetWinningTime()));
    }

    protected virtual void OnLoosingWave()
    {
        PreparingAgentToDisappear();
        disappearRoutineCoroutine = StartCoroutine(DisappearRoutine(GetLoosingTime()));
    }
    
    protected Temple GetTemple()
    {
        foreach (var c in BaseConstruction.AllConstructions)
        {
            if (c is Temple temple)
                return temple;
        }

        return null;
    }

    protected bool IsTempleAlive()
    {
        var temple = GetTemple();
        return temple != null && !temple.IsDestroyed;
    }



    //Cooroutines

    protected IEnumerator StunnedRoutine(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);

        IsStunned = false;

        if (agent != null)
            agent.isStopped = false;

        stunCoroutine = null;
    }

    protected IEnumerator DisappearRoutine(float time)
    {
        yield return new WaitForSeconds(time);

        Disappear();
        disappearRoutineCoroutine = null;
    }

}


