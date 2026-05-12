using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : NavMeshAgentBehaviour
{
    [SerializeField] protected EnemyData data;

    public Transform currentTarget;
    public Transform mainTarget;   
    public Transform combatTarget;
    private Temple temple;

    private float combatDistance;
    private float distance;

    protected float actionVelocity;
    protected float attackRange;
    protected float searchRange = 20f;

    [SerializeField] private float combatRange = 5f;
    [SerializeField] private float searchInterval = 1.5f;
    [SerializeField] private float stunTime = 1f;
    [SerializeField] private float deathTime = 1.5f;
    [SerializeField] private float winningTime = 2f;
    [SerializeField] private float loosingTime = 2f;
    protected float lastAttackTime;


    //Enemy Creation
    protected override void Awake()
    {
        base.Awake();
        SetTeam(Team.Enemy);

    }

    public virtual void Initialize(EnemyData newData)
    {
        data = newData;
        resistance = data.resistance;
        attackDamage = data.attackDamage;
        movementSpeed = data.movementSpeed;
        actionVelocity = data.actionVelocity;
        attackRange = data.attackRange;

        Debug.Log($"Enemigo Inicializado: Resistencia: {data.resistance}, danio: {data.attackDamage}");

        if (agent != null)
            agent.speed = movementSpeed;
    }

    protected override void OnEnable()
    {
        base.OnEnable(); 

        currentTarget = null;
        mainTarget = null;
        combatTarget = null;
        temple = GetTemple();

        StartCoroutine(SearchTargetRoutine()); 
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    void Update()
    {
        if (IsDead || IsStunned)
            return;

        if (combatTarget != null)
        {
            combatDistance = CalculateDistance(combatTarget);

            if (combatDistance > combatRange * combatRange)
                combatTarget = null;
        }

        if (combatTarget != null && combatTarget.gameObject.activeInHierarchy)
        {
            currentTarget = combatTarget;
        }
        else if (mainTarget != null && mainTarget.gameObject.activeInHierarchy)
        {
            currentTarget = mainTarget;
        }
        else
        {
            return;
        }

        distance = CalculateDistance(currentTarget);

        if (distance <= attackRange * attackRange)
        {
            agent.isStopped = true;
            Attack(currentTarget);
        }
        else
        {
            agent.isStopped = false;
            MoveTo(currentTarget.position);
        }
    }

    //Functions 

    //Define target and attack
    float CalculateDistance(Transform target)
    {
        return (transform.position - target.position).sqrMagnitude;
    }

    protected virtual void Attack(Transform currentTarget)
    {
        if (Time.time < lastAttackTime + actionVelocity) 
            return;

        lastAttackTime = Time.time;

        if (currentTarget == null) 
            return;

        if (currentTarget.TryGetComponent<IDamageable>(out var damagable))
        {
            damagable.ReceiveDamage(attackDamage);

            if (GameplaySoundPlayer.Instance != null)
                GameplaySoundPlayer.Instance.PlayEnemyHit();
            //Debug.Log("Atacando al targt del enemigo");

        }
    }

    //Damage and deactivation
    protected override void OnDamage()
    {
        base.OnDamage();
        // AQUÍ ANIMACIÓN DE DANIO DE ENEMIG
        //Debug.Log("El enemigo recibió danio");
        if (GameplaySoundPlayer.Instance != null)
            GameplaySoundPlayer.Instance.PlayEnemyHit();
    }


    protected override void OnDeath()
    {
        currentTarget = null;
        //AQUÍ ANIMACIÓN DE MUERTE DE ENEMIGO
        if (GameplaySoundPlayer.Instance != null)
            GameplaySoundPlayer.Instance.PlayEnemyDeath();
        Debug.Log("enemigo Murió");
        base.OnDeath();
    }


    //Get time of...
    protected override float GetStunTime()
    {
        return stunTime;
    }

    protected override float GetDeathTime()
    {
        return deathTime;
    }

    protected override float GetWinningTime()
    {
        return winningTime;
    }

    protected override float GetLoosingTime()
    {
        return loosingTime;
    }


    //How does enemy handle the ending of the wave?
    protected override void HandleWaveEnd()
    {
        base.HandleWaveEnd();

        if (IsTempleAlive())
        {
            OnLoosingWave();   
        }
        else
        {
            OnWinningWave();  
        }
    }

    protected override void OnLoosingWave()
    {
        base.OnLoosingWave();
        //AQUÍ AIMACIÓN DE DERROTA DE ENEMIGO
    }

    protected override void OnWinningWave()
    {
        base.OnWinningWave();
        //AQUÍ AIMACIÓN DE VICTORIA DE ENEMIGO
    }



    //Cooroutine
    IEnumerator SearchTargetRoutine()
    {
        while (isActiveAndEnabled && !IsDead)
        {
            if (IsStunned)
            {
                yield return new WaitForSeconds(searchInterval);
                continue;
            }

            mainTarget = targetFinder.FindTarget<BaseConstruction>(transform, searchRange);

            combatTarget = targetFinder.FindTarget<HandToHandSoldier>(transform, combatRange);

            yield return new WaitForSeconds(searchInterval);
        }
    }
}