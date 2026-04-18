using UnityEngine;
using System.Collections;

public class HandToHandSoldier : NavMeshAgentBehaviour
{
    public Barracks barrack;

    private Transform currentTarget;
    private TargetFinder targetFinder;

    [SerializeField] private float searchInterval = 1.5f;

    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1f;

    [SerializeField] private float stunTime = 1.5f;
    [SerializeField] private float deathTime = 1.5f;

    private float lastAttackTime;

    private float range;

    protected override void Awake()
    {
        base.Awake();
        targetFinder = FindAnyObjectByType<TargetFinder>();
    }

    public void Initialize(ConstructionData data, Barracks barrack)
    {
        this.barrack = barrack;

        resistance = data.aditamentResistance;
        attackDamage = data.attackDamage;
        movementSpeed = data.movementSpeed;
        range = data.range;

        if (agent != null)
            agent.speed = movementSpeed;

        SetTeam(Team.Ally);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        currentTarget = null;

        if (agent != null)
        {
            agent.isStopped = false;
            agent.ResetPath();
        }

        StartCoroutine(SearchTargetRoutine());
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        StopAllCoroutines();
    }

    void Update()
    {
        if (IsDead)
            return;

        if (IsStunned)
        {
            agent.isStopped = true;
            return;
        }

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
            Debug.Log("Atacando a enemigo");
        }
    }

    IEnumerator SearchTargetRoutine()
    {
        while (isActiveAndEnabled && !IsDead)
        {
            if (!IsStunned && currentTarget == null)
            {
                currentTarget = targetFinder.FindTarget<Enemy>(transform, range);
            }

            yield return new WaitForSeconds(searchInterval);
        }
    }

    public override void OnDamage()
    {
        base.OnDamage();

        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);

        stunCoroutine = StartCoroutine(StunnedRoutine());
    }

    IEnumerator StunnedRoutine()
    {
        yield return new WaitForSeconds(stunTime);

        IsStunned = false;

        if (agent != null)
            agent.isStopped = false;
    }

    public override void OnDeath()
    {
        base.OnDeath();

        currentTarget = null;

        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(deathTime);
        Die();
    }
}

/*
using UnityEngine;
using System.Collections;

public class HandToHandSoldier : NavMeshAgentBehaviour
{
    public Barracks barrack;

    [SerializeField] private Transform currentTarget;
    private TargetFinder targetFinder;

    [SerializeField] private float searchInterval = 1.5f;
    [SerializeField] private float timeToDie = 1.5f;
    [SerializeField] private float timeToBeStunned = 1.5f;

    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1f;

    private float lastAttackTime;

    private float range;
    private float resourceRate;


    protected override void Awake()
    {
        base.Awake();
        targetFinder = FindAnyObjectByType<TargetFinder>();
    }

    public void Initialize(ConstructionData data, Barracks barrack)
    {
        this.barrack = barrack;
        resistance = data.aditamentResistance;
        attackDamage = data.attackDamage;
        movementSpeed = data.movementSpeed;

        foreach (var w in data.willObtaied)
        {
            resourceRate = w.amount;
        }

        range = data.range;

        if (agent != null)
        {
            agent.speed = movementSpeed;
        }

        SetTeam(Team.Ally);
        Debug.Log("Soldado creado");
        Debug.Log($"Mis datos son: resistencia: {resistance} danio: {attackDamage} generación de recursos({resourceRate}) velocidad de mov:({movementSpeed})");
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        isDead = false;
        isStunned = false;
        currentTarget = null;

        if (agent != null)
        {
            agent.isStopped = false;
            agent.ResetPath();
            //agent.velocity = Vector3.zero;
        }

        StartCoroutine(SearchTargetRoutine());
    }

    protected override void OnDisable() //
    {
        base.OnDisable();
        StopAllCoroutines();
    }

    void Update()
    {
        if (isDead)
            return;

        if (IsStunned)
        {
            agent.isStopped = true;
            return;
        }

        if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy)
        {
            currentTarget = null;
            return;
        }
        /*
        if (isStunned || currentTarget == null)
            return;
    

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

    //Funciones
    public override void OnDamage()
    {
        base.OnDamage();
        //animación de daño de soldado
        Debug.Log("Soldado recibió danio");
        if (stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine);
        }

        stunCoroutine = StartCoroutine(StunnedRoutine());
    }

    public override void OnDeath()
    {
        base.OnDeath();
        currentTarget = null;
        //animación de muerte de soldado
        Debug.Log("Soldado Murió");
        StartCoroutine(DeathRoutine());
    }

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
        }
    }




    //Coorutinas
    IEnumerator SearchTargetRoutine()
    {
        while (isActiveAndEnabled && !isDead)
        {
            if(!isStunned && currentTarget == null)
            {
                currentTarget = targetFinder.FindTarget<Enemy>(transform, range);
            }
            yield return new WaitForSeconds(searchInterval);
        }
    }

    IEnumerator StunnedRoutine()
    {
        yield return new WaitForSeconds(timeToBeStunned);
        isStunned = false;
        if (agent != null)
        {
            agent.isStopped = false;

            if (currentTarget == null)
            {
                agent.ResetPath();
            }
        }
    }

    IEnumerator DeathRoutine()
    {
        yield return new WaitForSeconds(timeToDie); 
        Die();
    }

}
*/