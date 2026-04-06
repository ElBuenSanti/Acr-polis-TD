using UnityEngine;
using UnityEngine.AI;

public class HandToHandSoldier : NavMeshAgentBehaviour
{
    public Barracks barrack;

    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private Transform currentTarget;



    protected override void Awake()
    {
        base.Awake();
        Debug.Log("Soldado creado");

        if (agent != null)
        {
            agent.speed = speed;
        }
    }

    void Update()
    {
        currentTarget = FindTarget<Enemy>(); 

        if (currentTarget != null)
        {
            MoveTo(currentTarget.position);
        }
    }

}
