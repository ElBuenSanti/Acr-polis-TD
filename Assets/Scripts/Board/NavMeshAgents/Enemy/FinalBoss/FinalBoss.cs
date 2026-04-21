using UnityEngine;
using System.Collections;

public class FinalBoss : Enemy
{
    private Pooling pooling;
    [SerializeField] private GameObject fireBallPrefab;
    protected override void Awake()
    {
        base.Awake();
        pooling = FindAnyObjectByType<Pooling>();
    }
    protected override void Attack(Transform currentTarget)
    {
        if (Time.time < lastAttackTime + actionVelocity) 
            return;

        lastAttackTime = Time.time;

        if (currentTarget == null) 
            return;

        Vector3 spawnPos = transform.position + Vector3.up * 1.5f;

        GameObject newFireBall = pooling.CreateObject(fireBallPrefab, transform);

        newFireBall.transform.position = spawnPos;

        if (newFireBall.TryGetComponent<FireBall>(out var fireBall))
        {
            fireBall.Initialize(data, spawnPos, currentTarget.position, this);
        }
    }
}
