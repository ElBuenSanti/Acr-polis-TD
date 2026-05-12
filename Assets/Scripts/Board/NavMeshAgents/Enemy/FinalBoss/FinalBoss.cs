using UnityEngine;
using System;
using System.Collections;
using System.Runtime.CompilerServices;

public class FinalBoss : Enemy
{
    public static event Action OnFinalBossDeath;

    [SerializeField] private GameObject fireBallPrefab;
    private Pooling pooling;

    //Final Boss creation
    protected override void Awake()
    {
        base.Awake();
        pooling = FindAnyObjectByType<Pooling>();
    }


    //Attack and FireBall Generation
    protected override void Attack(Transform currentTarget)
    {
        if (Time.time < lastAttackTime + actionVelocity) 
            return;

        lastAttackTime = Time.time;

        if (currentTarget == null) 
            return;

        Vector3 spawnPos = transform.position + Vector3.up * 1.5f;

        GameObject newFireBall = pooling.CreateObject(fireBallPrefab, transform);

        if (GameplaySoundPlayer.Instance != null)
            GameplaySoundPlayer.Instance.PlayFire();

        newFireBall.transform.position = spawnPos;

        if (newFireBall.TryGetComponent<FireBall>(out var fireBall))
        {
            fireBall.Initialize(data, spawnPos, currentTarget.position, this);
        }
    }

    //When defeating the final boss...
    protected override void OnDeath()
    {
        base.OnDeath();
        if (GameplaySoundPlayer.Instance != null)
            GameplaySoundPlayer.Instance.PlayBossDeath();
        Debug.Log("¡Has ganado el juego!"); 
        OnFinalBossDeath?.Invoke(); 
    }
}
