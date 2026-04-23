using UnityEngine;
using System.Collections;

public class Arrow : Projectile
{
    public Defense defense;
    public ConstructionData defenseData;

    private float resourceRate;
    //private bool hasHit;

    public void Initialize(ConstructionData data, Vector3 start, Vector3 target, Defense defense)
    {
        this.defense = defense;
        defenseData = data;

        /*
        foreach (var w in data.willObtaied)
        {
            resourceRate = w.amount;
        }
        */

        Setup(start, target, data.attackDamage, data.proyectileVelocity);
        SetTeam(Team.Ally);

        Debug.Log("Flecha creada");
        Debug.Log($"Mis datos son: attackDamage: {attackDamage} velocity: ({proyectileVelocity})");
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        //hasHit = false;
    }

    //Colisiones
    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        if (other.TryGetComponent<Enemy>(out var enemy))
        {
            if (other.TryGetComponent<IDamageable>(out var target))
            {
                hasHit = true;
                target.ReceiveDamage(attackDamage);
                SpawnWill(); //por golpe se spawnea la voluntad
                Debug.Log("Atacando a enemigo");
            }

            Hit();
        }
    }

    //Fnciones

    void SpawnWill()
    {
        foreach (var w in defenseData.willObtaied) //puede ser más de una voluntad generada
        {
            WillManager.Instance.AddMoney(w.type, w.amount);
        }
    }

    protected override void OnHit()
    {
        Debug.Log("Golpeo flecha y se reproduce animación de destrucción de flecha");
        //aqui animación
    }

}
