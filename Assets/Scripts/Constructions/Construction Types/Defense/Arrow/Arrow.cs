using UnityEngine;
using System.Collections;

public class Arrow : Projectile
{
    public Defense defense;
    public ConstructionData defenseData;


    //Extracting Arrow values
    public void Initialize(ConstructionData data, Vector3 start, Vector3 target, Defense defense)
    {
        this.defense = defense;

        defenseData = data;

        Setup(start, target, defenseData.attackDamage, defenseData.proyectileVelocity); //ANTES data.attack... en lugar de defenseData
        SetTeam(Team.Ally);

        if (GameplaySoundPlayer.Instance != null)
            GameplaySoundPlayer.Instance.PlayArrowShot();

        Debug.Log($"Flecha inicializada: attackDamage: {attackDamage} velocity: ({proyectileVelocity})");
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    //Functions

    //When colliding with something...
    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        if (other.TryGetComponent<Enemy>(out var enemy))
        {
            if (other.TryGetComponent<IDamageable>(out var target))
            {
                hasHit = true;
                target.ReceiveDamage(attackDamage);
                SpawnWill(); 
                Debug.Log("Atacando a enemigo");
            }

            Hit();
        }
    }

    void SpawnWill()
    {
        foreach (var w in defenseData.willObtaied) 
        {
            WillManager.Instance.AddMoney(w.type, w.amount);
        }
    }

    protected override void OnHit()
    {
        if (GameplaySoundPlayer.Instance != null)
            GameplaySoundPlayer.Instance.PlayArrowHit();

        Debug.Log("Golpeo flecha y se reproduce animación de destrucción de flecha");
        //AQUÍ ANIMACIÓN DE FLECHA COLISIONANDO
    }

}
