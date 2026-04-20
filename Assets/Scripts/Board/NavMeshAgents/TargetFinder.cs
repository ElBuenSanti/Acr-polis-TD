using UnityEngine;
using System.Collections.Generic;

public class TargetFinder : MonoBehaviour
{
    public Transform FindTarget<T>(Transform origin, float range) where T : MonoBehaviour
    {
        Transform nearestTarget = null;
        float minDistance = Mathf.Infinity;


        var originTeam = origin.GetComponent<TeamAssigner>();

        IEnumerable<MonoBehaviour> targets = GetTargets<T>();

        foreach (var unit in targets)
        {
            if (unit == null) continue;


            if (!unit.TryGetComponent<T>(out _))
                continue;


            var targetTeam = unit.GetComponent<TeamAssigner>();
            if (originTeam != null && targetTeam != null)
            {
                if (originTeam.GetTeam() == targetTeam.GetTeam())
                    continue;
            }

            float distance = Vector3.Distance(origin.position, unit.transform.position);


            if (distance > range)
                continue;


            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTarget = unit.transform;
            }
        }

        return nearestTarget;
    }

    IEnumerable<MonoBehaviour> GetTargets<T>()
    {

        if (typeof(T) == typeof(Enemy) || typeof(T) == typeof(HandToHandSoldier))
            return NavMeshAgentBehaviour.AllUnits;


        if (typeof(T) == typeof(BaseConstruction))
            return BaseConstruction.AllConstructions;

        return new List<MonoBehaviour>();
    }
}

/*
using UnityEngine;
using System.Collections.Generic;

public class TargetFinder : MonoBehaviour
{
    public Transform FindTarget<T>(Transform origin, float range) where T : MonoBehaviour
    {
        Transform nearestTarget = null;
        float minDistance = Mathf.Infinity;

        IEnumerable<MonoBehaviour> targets = GetTargets<T>();

        foreach (var unit in targets)
        {
            if (unit == null) continue;

            float distance = Vector3.Distance(origin.position, unit.transform.position);

            if (distance > range)
                continue;

            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTarget = unit.transform;
            }
        }

        return nearestTarget;
    }

    IEnumerable<MonoBehaviour> GetTargets<T>()
    {
        if (typeof(T) == typeof(Enemy))
            return NavMeshAgentBehaviour.AllUnits;

        if (typeof(T) == typeof(BaseConstruction))
            return BaseConstruction.AllConstructions;

        return new List<MonoBehaviour>(); // fallback vacío
    }
}
/*
using UnityEngine;

public class TargetFinder : MonoBehaviour
{

    public Transform FindTarget<T>(Transform origin, float range)
    {
        Transform nearestTarget = null;
        float minDistance = Mathf.Infinity;

        var target = NavMeshAgentBehaviour.AllUnits;
        var target = BaseConstruction.AllUnits;

        foreach (var unit in NavMeshAgentBehaviour.AllUnits)
        {
            if (unit == null) continue;

            if (!unit.TryGetComponent<T>(out _))
                continue;

            float distance = Vector3.Distance(origin.position, unit.transform.position);

            if (distance > range)
                continue;

            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTarget = unit.transform;
            }
        }

        return nearestTarget;
    }
}
    /*
    public Transform FindTarget<T>(Transform origin, float range) where T : NavMeshAgentBehaviour
    {
        Transform nearestTarget = null;
        float minDistance = Mathf.Infinity;

        var originTeam = origin.GetComponent<TeamAssigner>();

        if (originTeam == null)
            return null;

        foreach (var unit in NavMeshAgentBehaviour.AllUnits)
        {
            if (unit is not T)
                continue;

            if (unit.IsDead)
                continue;
            
            if (unit.GetTeam() == originTeam.GetTeam())
                continue;
            

            float distance = Vector3.Distance(origin.position, unit.transform.position);

            if (distance > range)
                continue;

            if (distance < minDistance)
            {
                minDistance = distance;
                nearestTarget = unit.transform;
            }
        }

        return nearestTarget;
    }
    /*
    public Transform FindTarget<Type>(Transform origin) where Type : Component //Type debe ser componente de unity o sea un script
    {
        Type[] allTargets = Object.FindObjectsByType<Type>(FindObjectsInactive.Exclude, FindObjectsSortMode.None); //que busque todos los que tienen ese componente, que estén activos, y sin importar un orden específico

        if (allTargets.Length == 0)
        {
            return null; //no hay nada que buscar
        }

        Transform nearestTarget = null;
        float minDistance = Mathf.Infinity; //para que al comparar, cualquiera sea más cercano
        Vector3 currentPosition = origin.position; //la llamada es desde otra clase, es la posición del que esta buscando

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

