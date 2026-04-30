using UnityEngine;
using System.Collections;

public class Projectile : TeamAssigner
{
    protected float attackDamage;
    protected float proyectileVelocity;

    protected Vector3 startPoint;
    protected Vector3 targetPoint;

    protected float time;
    protected float arcHeight = 5f;

    protected bool isHitting;
    protected bool hasHit;


    //Assining proyectile info
    protected virtual void Setup(Vector3 start, Vector3 target, float damage, float velocity)
    {
        startPoint = start;
        targetPoint = target;

        attackDamage = damage;
        proyectileVelocity = velocity;

        transform.position = startPoint;
        time = 0f;
    }

    protected virtual void OnEnable()
    {
        time = 0f;
        isHitting = false;
        hasHit = false;
    }

    protected virtual void Update()
    {
        time += Time.deltaTime * proyectileVelocity;

        float t = time;

        if (t >= 1f)
        {
            Hit();
            return;
        }

        Vector3 position = Vector3.Lerp(startPoint, targetPoint, t); //antes pos

        position.y += arcHeight * (t * (1 - t)) * 4;

        transform.position = position;
    }

    //Functions

    //When hitting something...
    protected virtual void Hit()
    {
        if (isHitting) return;

        isHitting = true;
        StartCoroutine(HitRoutine());
    }

    protected virtual void OnHit()
    {
    }


    //Cooroutines
    IEnumerator HitRoutine()
    {
        OnHit();

        yield return new WaitForSeconds(0.5f);

        gameObject.SetActive(false);
    }
}
