using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : NavMeshAgentBehaviour
{
    [SerializeField] protected EnemyData data;

    public Transform currentTarget;
    public Transform mainTarget;   // estructura
    public Transform combatTarget; // soldado cercano
    private float combatDistance;
    private float distance;
    [SerializeField] private float combatRange = 5f;

    [SerializeField] private float searchInterval = 1.5f;

    private float actionVelocity;
    private float attackRange;
    private float searchRange = 20f;

    [SerializeField] private float stunTime = 1f;
    [SerializeField] private float deathTime = 1.5f;

    private float lastAttackTime;

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

        Debug.Log($"Enemigo vida: {data.resistance} danio: {data.attackDamage}");

        if (agent != null)
            agent.speed = movementSpeed;
    }

    protected override void OnEnable()
    {
        base.OnEnable(); 

        currentTarget = null;

        /*
        if (WaveSpawner.Instance != null)
            WaveSpawner.Instance.OnWaveEnded += HandleWaveEnd;
        */

        StartCoroutine(SearchTargetRoutine()); 
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        /*
        if (WaveSpawner.Instance != null)
            WaveSpawner.Instance.OnWaveEnded -= HandleWaveEnd;
        */
    }

    void Update()
    {
        if (IsDead || IsStunned)
            return;

        if (combatTarget != null)
        {
            combatDistance = CalculateDistance(combatTarget);
                //Vector3.Distance(transform.position, combatTarget.position);

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
            //Vector3.Distance(transform.position, currentTarget.position);

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

    //Funciones
    float CalculateDistance(Transform target)
    {
        return (transform.position - target.position).sqrMagnitude;
        //Vector3.Distance(transform.position, target.position);
    }

    void Attack(Transform currentTarget)
    {
        if (Time.time < lastAttackTime + actionVelocity) //si aun no pasa el tiempo para atacar, no hace nada
            return;

        lastAttackTime = Time.time;

        if (currentTarget == null) //si se vuelve null, sale de atacar
            return;

        if (currentTarget.TryGetComponent<IDamageable>(out var damagable))
        {
            damagable.ReceiveDamage(attackDamage); //ataca al enemigo
            Debug.Log("Atacando al soldado");

        }
    }

    public override void OnDamage()
    {
        base.OnDamage();
        //animación de daño de soldado
        Debug.Log("enemigo recibió danio");
    }


    public override void OnDeath()
    {
        currentTarget = null;
        //animación de muerte de soldado
        Debug.Log("enemigo Murió");
        base.OnDeath();
    }

    protected override float GetStunTime()
    {
        return stunTime;
    }

    protected override float GetDeathTime()
    {
        return deathTime;
    }

    protected override void HandleWaveEnd()
    {
        base.HandleWaveEnd();
        //if (IsDead) return;

        OnDeath();
    }



    //Coorutinas

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