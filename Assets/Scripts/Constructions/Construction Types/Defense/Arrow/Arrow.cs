using UnityEngine;

public class Arrow : TeamAssigner, IDamageable
{
    public Defense defense;

    private float resistance;
    private float attackDamage;
    private float resourceRate;
    private float range;
    private float proyectileVelocity;

    private Vector3 startPoint;
    private Vector3 targetPoint;

    private float time;
    private float arcHeight = 3f;
    public void Initialize(ConstructionData data, Vector3 start, Vector3 target, Defense defense)
    {
        this.defense = defense;

        startPoint = start;
        targetPoint = target;

        resistance = data.aditamentResistance;
        attackDamage = data.attackDamage;
        resourceRate = data.resourceRate;
        range = data.range;
        proyectileVelocity = data.proyectileVelocity;

        SetTeam(Team.Ally);

        transform.position = startPoint;
        time = 0f;

        Debug.Log("Flecha creada");
        Debug.Log($"Mis datos son: {resistance} {attackDamage} ({resourceRate}) ({proyectileVelocity})");
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

    void Hit()
    {
        gameObject.SetActive(false);
    }

    public void ReceiveDamage(float damage)
    {

    }
}
