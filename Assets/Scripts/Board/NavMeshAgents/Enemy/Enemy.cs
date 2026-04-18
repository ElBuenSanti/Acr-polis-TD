using UnityEngine;

public class Enemy : NavMeshAgentBehaviour
{

    protected override void Awake()
    {
        SetTeam(Team.Enemy);
        resistance = 50f;
    }

    public override void OnDeath()
    {
        base.OnDeath();
        Die();
    }
}
