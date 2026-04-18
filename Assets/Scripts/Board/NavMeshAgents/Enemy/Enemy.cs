using UnityEngine;

public class Enemy : NavMeshAgentBehaviour
{
    [SerializeField] protected EnemyData data;

    private float actionVelocity;
    private float attackRange;

    protected override void Awake()
    {
        base.Awake();
        SetTeam(Team.Enemy);
    }

    public virtual void Initialize(EnemyData newData)
    {
        data = newData;
        resistance = data.resistance;
        attackDamage = data.attackDamage;
        movementSpeed = data.movementSpeed;
        actionVelocity = data.actionVelocity;
        attackRange = data.attackRange;

        if (agent != null)
            agent.speed = movementSpeed;

        //SetTeam(Team.Enemy);
    }

    public override void OnDeath()
    {
        base.OnDeath();
        Die();
    }
}