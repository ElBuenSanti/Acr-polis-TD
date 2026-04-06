using UnityEngine;
using UnityEngine.AI;

public abstract class NavMeshAgentBehaviour : MonoBehaviour
{

    protected NavMeshAgent agent;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public virtual void MoveTo(Vector3 destination)
    {
        if (agent != null)
        {
            agent.SetDestination(destination);
        }
            
    }

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
}
