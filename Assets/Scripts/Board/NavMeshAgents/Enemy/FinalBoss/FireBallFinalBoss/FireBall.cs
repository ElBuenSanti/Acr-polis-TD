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
        if (hasHit) return;

        var team = other.GetComponentInParent<TeamAssigner>();

        if (team == null || team.GetTeam() == Team.Enemy) //no ataque a otros enemigos, solo cosas aliadas
            return;

        if (other.TryGetComponent<IDamageable>(out var target))
        {
            hasHit = true;
            target.ReceiveDamage(attackDamage);
            Debug.Log("Atacando con bola de fuego");
        }

        Hit();
    }

    protected override void OnHit()
    {
        Debug.Log("Golpeo bola de fuego y se reproduce animación");
        //aqui animación
    }


}
