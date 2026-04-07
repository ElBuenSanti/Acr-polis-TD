using UnityEngine;
using System.Collections;

public class HandToHandSoldier : NavMeshAgentBehaviour
{
    public Barracks barrack;

    [SerializeField] private float speed = 5f;
    [SerializeField] private int soldierHealth = 100;
    [SerializeField] private Transform currentTarget;

    [SerializeField] private float searchInterval = 1.5f;
    [SerializeField] private float timeToDie = 1.5f;
    [SerializeField] private float timeToBeStunned = 1.5f;

    private TargetFinder targetFinder;

    protected override void Awake()
    {
        base.Awake();
        health = soldierHealth;

        Debug.Log("Soldado creado");
        targetFinder = FindAnyObjectByType<TargetFinder>();
        if (agent != null)
        {
            agent.speed = speed;
        }
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
        }
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
