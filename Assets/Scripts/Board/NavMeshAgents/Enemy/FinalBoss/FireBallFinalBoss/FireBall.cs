using UnityEngine;

public class FireBall : Projectile
{
    private FinalBoss finalBoss;
    private EnemyData enemyData;

    public void Initialize(EnemyData data, Vector3 start, Vector3 target, FinalBoss finalBoss)
    {
        this.finalBoss = finalBoss;

        enemyData = data;

        Setup(start, target, data.attackDamage, data.proyectileVelocity);
        SetTeam(Team.Enemy);

        Debug.Log("Bola de fuego creada");
        Debug.Log($"Mis datos son: attackDamage: {attackDamage} velocity: ({proyectileVelocity})");
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("COLISION CON: " + other.name);

        if (hasHit) return;

        var team = other.GetComponentInParent<TeamAssigner>();

        if (team == null)
        {
            Debug.Log("NO tiene TeamAssigner");
            return;
        }

        Debug.Log("Team: " + team.GetTeam());

        if (team.GetTeam() != Team.Ally)
        {
            Debug.Log("No es Ally");
            return;
        }

        var target = other.GetComponentInParent<IDamageable>();

        if (target == null)
        {
            Debug.Log("NO tiene IDamageable");
            return;
        }

        Debug.Log("Bola de fuego haciendo daño");

        hasHit = true;
        target.ReceiveDamage(attackDamage);

        Hit();
    }

    protected override void OnHit()
    {
        Debug.Log("Golpeo bola de fuego y se reproduce animación");
        //aqui animación
    }


}
