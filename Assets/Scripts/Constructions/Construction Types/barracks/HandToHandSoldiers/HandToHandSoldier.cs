using UnityEngine;
using System.Collections;

public class HandToHandSoldier : NavMeshAgentBehaviour
{
    public Barracks barrack;

    [SerializeField] private Transform currentTarget;

    [SerializeField] private float searchInterval = 1.5f;
    [SerializeField] private float timeToDie = 1.5f;
    [SerializeField] private float timeToBeStunned = 1.5f;

    private float resourceRate;

    private TargetFinder targetFinder;

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
        resourceRate = data.resourceRate;
        movementSpeed = data.movementSpeed;

        if (agent != null)
        {
            agent.speed = movementSpeed;
        }

        SetTeam(Team.Ally);
        Debug.Log("Soldado creado");
        Debug.Log($"Mis datos son: {resistance} {attackDamage} ({resourceRate}) ({movementSpeed})");
    }

    void Start()
    {
        StartCoroutine(SearchTargetRoutine());
    }

    void OnEnable()
    {
        isDead = false;
        isStunned = false;
        currentTarget = null;

        if (agent != null)
        {
            agent.isStopped = false;
            agent.ResetPath();
            agent.velocity = Vector3.zero;
        }

        StartCoroutine(SearchTargetRoutine());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }

        if (!isStunned && currentTarget != null)
        {
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




    //Coorutinas
    IEnumerator SearchTargetRoutine()
    {
        while (isActiveAndEnabled && !isDead)
        {
            if(!isStunned && currentTarget == null)
            {
                currentTarget = targetFinder.FindTarget<Enemy>(transform);
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
