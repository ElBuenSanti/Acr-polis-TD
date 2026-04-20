using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class NavMeshAgentBehaviour : TeamAssigner, IDamageable
{

    protected NavMeshAgent agent;
    protected TargetFinder targetFinder;

    protected float resistance;
    protected float attackDamage;
    protected float movementSpeed;
    public bool IsDead { get; protected set; }
    public bool IsStunned { get; protected set; }

    protected Coroutine stunCoroutine;
    protected Coroutine deathCoroutine;

    public static List<NavMeshAgentBehaviour> AllUnits = new List<NavMeshAgentBehaviour>();

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        targetFinder = FindAnyObjectByType<TargetFinder>();
    }

    protected virtual void OnEnable()
    {
        AllUnits.Add(this);
        IsDead = false; //ambos empiezan vivos
        IsStunned = false; //ambos empiezan sin estar aturdidos

        if (agent != null)
        {
            agent.isStopped = false; //no está detenido
            agent.ResetPath();
        }

    }

    protected virtual void OnDisable()
    {
        AllUnits.Remove(this);
        StopAllCoroutines();
    }

    //Funciones
    public virtual void MoveTo(Vector3 destination)
    {
        if (agent == null || IsDead || IsStunned) //si no hay agente asignado, esta muerto o aturdido, no hace nada
            return;

        agent.SetDestination(destination); //se mueve hacia él
    }
            
   
    public virtual void ReceiveDamage(float damage)
    {
        if (IsDead)
        {
            return; //si esta muerto, ya no tiene sentido que reciva daño
        }

        resistance -= damage; //se le quita vida

        if (resistance <= 0) //si llega a 0
        {
            IsDead = true; //ya esta muerto
            OnDeath();
            return; //evitar que se actuive ondamage
        }
        OnDamage();
    }

    public virtual void OnDamage()
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


    public virtual void Die()
    {
        gameObject.SetActive(false); //se desactiva
    }

    public virtual void OnDeath()
    {
        if (agent != null)
            agent.isStopped = true;

        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);

        if (deathCoroutine != null)
            StopCoroutine(deathCoroutine);

        deathCoroutine = StartCoroutine(DeathRoutine(GetDeathTime()));
        //Die();
    }

    protected virtual float GetStunTime()
    {
        return 0;
    }

    protected virtual float GetDeathTime()
    {
        return 0;
    }


    //Coorutinas

    protected IEnumerator StunnedRoutine(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);

        IsStunned = false;

        if (agent != null)
            agent.isStopped = false;

        stunCoroutine = null;
    }

    protected IEnumerator DeathRoutine(float deathTime)
    {
        yield return new WaitForSeconds(deathTime);

        Die();
        deathCoroutine = null;
    }
}
















/*
public Transform FindTarget<Type>() where Type : Component //Type debe ser componente de unity o sea un script
{
    Type[] allTargets = Object.FindObjectsByType<Type>(FindObjectsInactive.Exclude, FindObjectsSortMode.None); //que busque todos los que tienen ese componente, que estén activos, y sin importar un orden específico

    if (allTargets.Length == 0)
    {
        return null; //no hay nada que buscar
    }

    Transform nearestTarget = null;
    float minDistance = Mathf.Infinity; //para que al comparar, cualquiera sea más cercano
    Vector3 currentPosition = transform.position; //la llamada es desde la clase hija, es la posición del agente navegable

    foreach (Type target in allTargets)
    {
        float distance = Vector3.Distance(currentPosition, target.transform.position); 
        if (distance < minDistance)
        {
            minDistance = distance;
            nearestTarget = target.transform;
        }
    }

    return nearestTarget;
}
*/


