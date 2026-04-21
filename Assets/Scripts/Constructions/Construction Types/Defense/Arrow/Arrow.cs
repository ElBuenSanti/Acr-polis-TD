using UnityEngine;
using System.Collections;

public class Arrow : TeamAssigner
{
    public Defense defense;

    private float attackDamage;
    private float resourceRate;
    private float proyectileVelocity;

    private Vector3 startPoint;
    private Vector3 targetPoint;

    private float time;
    private float arcHeight = 5f;

    public void Initialize(ConstructionData data, Vector3 start, Vector3 target, Defense defense)
    {
        this.defense = defense;

        startPoint = start;
        targetPoint = target;

        attackDamage = data.attackDamage;
        proyectileVelocity = data.proyectileVelocity;

        foreach (var w in data.willObtaied)
        {
            resourceRate = w.amount;
            //prob. tipo de voluntad = w.
        }

        SetTeam(Team.Ally);

        transform.position = startPoint;
        time = 0f;

        Debug.Log("Flecha creada");
        Debug.Log($"Mis datos son: attackDamage: {attackDamage} velocity: ({proyectileVelocity})");
    }

   void OnEnable()
    {
        time = 0f;
    }

    void Update()
    {
        time += Time.deltaTime * proyectileVelocity;

        float t = time;

        if (t >= 1f)
        {
            Hit();
            return;
        }

        Vector3 pos = Vector3.Lerp(startPoint, targetPoint, t);

        pos.y += arcHeight * (t * (1 - t)) * 4;

        transform.position = pos;
    }

    //Colisiones
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Enemy>(out var enemy))
        {
            if (other.TryGetComponent<IDamageable>(out var target))
            {
                target.ReceiveDamage(attackDamage);
            }

            Hit();
        }
    }


    //Fnciones
    void Hit()
    {
        StartCoroutine(HitRoutine());
    }

    public virtual void OnHit()
    {
        Debug.Log("Golpeo flecha y se reproduce animación");
        //aqui animación
    }

    //Coorutinas

    IEnumerator HitRoutine()
    {
        OnHit(); 

        yield return new WaitForSeconds(0.5f); 

        gameObject.SetActive(false);
    }

}
