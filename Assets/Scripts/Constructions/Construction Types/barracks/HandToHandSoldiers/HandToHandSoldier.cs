using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandToHandSoldier : NavMeshAgentBehaviour
{
    public Barracks barrack;
    private Transform currentTarget;
    private List<WillProduction> willObtaied;

    [SerializeField] private float searchInterval = 1.5f;
    [SerializeField] private float attackCooldown = 3f;
    [SerializeField] private float stunTime = 3f;
    [SerializeField] private float deathTime = 1.5f;
    [SerializeField] private float winningTime = 2f;
    [SerializeField] private float loosingTime = 2f;
    private float lastAttackTime;

    [SerializeField] private float attackRange = 2f;
    private float range;

    //private Coroutine survivalCoroutine;


    //Soldier Creation
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        colorOfParticles = new Color(1f, 0.84f, 0f);
        base.Start();
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
        base.OnEnable();

        currentTarget = null; 

        StartCoroutine(SearchTargetRoutine()); 
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        //survivalCoroutine = null;

    }


    //Every frame
    void Update()
    {
        if (waveEnded)
            return;

        if (IsDead)
            return;

        if (IsStunned)
            return;

        if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy)  
        {
            currentTarget = null;
            return;
        }

        float distance = Vector3.Distance(transform.position, currentTarget.position); 

        if (distance <= attackRange)
        {
            agent.isStopped = true; 
            Attack(); 
        }
        else
        {
            agent.isStopped = false; 
            MoveTo(currentTarget.position);
        }
    }

    //Functions

    //Attacking Enemies
    void Attack()
    {
        if (Time.time < lastAttackTime + attackCooldown) 
            return;

        lastAttackTime = Time.time;

        if (currentTarget == null) 
            return;

        if (currentTarget.TryGetComponent<IDamageable>(out var target))
        {
            target.ReceiveDamage(attackDamage);
            if (GameplaySoundPlayer.Instance != null)
                GameplaySoundPlayer.Instance.PlaySoldierAttack();
            SpawnWill(); 
            Debug.Log("Atacando a enemigo");

        }
    }

   void SpawnWill()
    {
        foreach (var w in willObtaied) 
        {
            WillManager.Instance.AddMoney(w.type, w.amount);
        }
    }

    //Damage and deactivation
    protected override void OnDamage()
    {
        base.OnDamage();
        //AQUÍ ANIMACIÓN DE DAÑO DE SOLDADO
        Debug.Log("El soldado recibió danio");
    }


    protected override void OnDeath()
    {
        currentTarget = null;

        colorOfParticles = new Color(1f, 0.9f, 0f);
        FXManager.Instance.PlayFX(FXManager.Instance.particulesEffects, transform, colorOfParticles);

        //AQUÍ ANIMACIÓN DE MUERTE DEL SOLDADO
        if (GameplaySoundPlayer.Instance != null)
            GameplaySoundPlayer.Instance.PlaySoldierDeath();
        Debug.Log("El soldado ha muerto");
        base.OnDeath();
    }

    //How does soldier handle the ending of the wave?
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
        //AQUÍ AIMACIÓN DE VICTORIA DE SOLDADO
        colorOfParticles = new Color(1f, 0.84f, 0f);
        FXManager.Instance.PlayFX(FXManager.Instance.particulesEffects, transform, colorOfParticles);
    }

    protected override void OnLoosingWave()
    {
        base.OnLoosingWave();
        colorOfParticles = new Color(1f, 0f, 0f);
        FXManager.Instance.PlayFX(FXManager.Instance.particulesEffects, transform, colorOfParticles);
        //AQUÍ AIMACIÓN DE DERROTA DE SOLDADO
    }


    //Get Time of...
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


    //Cooroutines
    IEnumerator SearchTargetRoutine()
    {
        while (isActiveAndEnabled && !IsDead && !waveEnded) 
        {
            if (!IsStunned && currentTarget == null) 
            {
                currentTarget = targetFinder.FindTarget<Enemy>(transform, range); 
            }

            yield return new WaitForSeconds(searchInterval);
        }
    }

}

