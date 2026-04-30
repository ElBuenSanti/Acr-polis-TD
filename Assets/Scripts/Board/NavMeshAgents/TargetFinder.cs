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
