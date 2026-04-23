using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandToHandSoldier : NavMeshAgentBehaviour
{
    public Barracks barrack;

    private Transform currentTarget;

    [SerializeField] private float searchInterval = 1.5f;

    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 3f;

    [SerializeField] private float stunTime = 3f;
    [SerializeField] private float deathTime = 1.5f;
    [SerializeField] private float winningTime = 2f;
    [SerializeField] private float loosingTime = 2f;

    private float lastAttackTime;

    private List<WillProduction> willObtaied;

    private float range;

    private Coroutine survivalCoroutine;

    protected override void Awake()
    {
        base.Awake();
    }

    public void Initialize(ConstructionData data, Barracks barrack)
    {
        this.barrack = barrack;

        resistance = data.aditamentResistance;
        attackDamage = data.attackDamage;
        movementSpeed = data.movementSpeed;
        willObtaied = data.willObtaied;
        range = data.range;

        if (agent != null)
            agent.speed = movementSpeed;

        SetTeam(Team.Ally);
    }

    protected override void OnEnable()
    {
        base.OnEnable(); //se repite comportamiento

        currentTarget = null; //no hay target activo

        StartCoroutine(SearchTargetRoutine()); //se busca target
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        survivalCoroutine = null;

    }

    void Update()
    {
        if (waveEnded)
            return;

        if (IsDead)
            return; //si esta muerto no hace nada

        if (IsStunned)
        {
            //agent.isStopped = true;
            return; //si esta aturdido, tampoco 
        }

        if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy)  //si el target que tenía ha muerto o no tiene, sigue sin tener 
        {
            currentTarget = null;
            return;
        }

        float distance = Vector3.Distance(transform.position, currentTarget.position); //rango de ataque

        if (distance <= attackRange)
        {
            agent.isStopped = true; //se detiene para atacar
            Attack(); //ataca
        }
        else
        {
            agent.isStopped = false; //sigue caminando si no esta en rango
            MoveTo(currentTarget.position);
        }
    }

    //Funciones
    void Attack()
    {
        if (Time.time < lastAttackTime + attackCooldown) //si aun no pasa el tiempo para atacar, no hace nada
            return;

        lastAttackTime = Time.time;

        if (currentTarget == null) //si se vuelve null, sale de atacar
            return;

        if (currentTarget.TryGetComponent<IDamageable>(out var target))
        {
            target.ReceiveDamage(attackDamage); //ataca al enemigo
            SpawnWill(); //por golpe se spawnea la voluntad
            Debug.Log("Atacando a enemigo");

        }
    }

   void SpawnWill()
    {
        foreach (var w in willObtaied) //puede ser más de una voluntad generada
        {
            WillManager.Instance.AddMoney(w.type, w.amount);
        }
    }

    protected override void OnDamage()
    {
        base.OnDamage();
        //animación de daño de soldado
        Debug.Log("Soldado recibió danio");
    }


    protected override void OnDeath()
    {
        currentTarget = null;
        //animación de muerte de soldado
        Debug.Log("Soldado Murió");
        base.OnDeath();
    }

    protected override void HandleWaveEnd()
    {
        base.HandleWaveEnd();
        currentTarget = null;

        if (IsTempleAlive())
        {
            OnWinningWave();
        }
        else
        {
            OnLoosingWave();
        }
    }

    protected override void OnWinningWave()
    {
        base.OnWinningWave();
        Debug.Log("Animación de victoria soldado");
    }


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

    //Coorutinas
    IEnumerator SearchTargetRoutine()
    {
        while (isActiveAndEnabled && !IsDead && !waveEnded) //mientras esté activo y no muerto y la oleada este activa
        {
            if (!IsStunned && currentTarget == null) //si no esta aturdido y su target es nulo
            {
                currentTarget = targetFinder.FindTarget<Enemy>(transform, range); //busca al enemigo más cercano
            }

            yield return new WaitForSeconds(searchInterval);
        }
    }

}

