using UnityEngine;

public class FireBall : Projectile
{
    private FinalBoss finalBoss;

    //Extracting Values of fire ball
    public void Initialize(EnemyData data, Vector3 start, Vector3 target, FinalBoss finalBoss)
    {
        this.finalBoss = finalBoss;

        Setup(start, target, data.attackDamage, data.proyectileVelocity);
        SetTeam(Team.Enemy);

        Debug.Log($"Bola de fuego inicializada: attackDamage: {attackDamage} velocity: ({proyectileVelocity})");
    }


    //When colliding with something...
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

        Debug.Log("La bola de fuego esta haciendo danio");

        hasHit = true;
        target.ReceiveDamage(attackDamage);

        Hit();
    }

    protected override void OnHit()
    {
        Debug.Log("La bola de fuego ha impactado");
        //AQUÍ ANIMACIÓN DE BOLA DE FUEGO COLISIONANDO
    }


}
