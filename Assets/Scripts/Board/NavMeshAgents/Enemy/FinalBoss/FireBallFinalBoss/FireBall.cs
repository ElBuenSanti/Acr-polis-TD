using UnityEngine;

public class FireBall : Arrow
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

    
}
