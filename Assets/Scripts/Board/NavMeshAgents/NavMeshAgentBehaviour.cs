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
    public static List<NavMeshAgentBehaviour> AllUnits = new List<NavMeshAgentBehaviour>();

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        targetFinder = FindAnyObjectByType<TargetFinder>();
    }

    protected virtual void OnEnable()
    {
        AllUnits.Add(this);
        IsDead = false;
        IsStunned = false;
        
    }

    protected virtual void OnDisable()
    {
        AllUnits.Remove(this);
    }

    //Funciones
    public virtual void MoveTo(Vector3 destination)
    {
        if (agent == null || IsDead || IsStunned)
            return;

        agent.SetDestination(destination);
    }
            
   
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

    public virtual void OnDamage()
    {
        IsStunned = true;
        if (agent != null)
        {
            agent.isStopped = true; 
        }
    }

    public virtual void Die()
    {
        gameObject.SetActive(false);
    }

    public virtual void OnDeath()
    {
        if (agent != null)
        {
            agent.isStopped = true;
        }
        if (stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine);
        }
        
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


