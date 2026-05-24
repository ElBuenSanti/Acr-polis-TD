using UnityEngine;

public class FXManager : MonoBehaviour
{
    public static FXManager Instance;

    public Pooling pooling;

    [Header("FX Prefabs")]
    public GameObject particulesEffects;

    void Awake()
    {
        Instance = this;
    }

    public void PlayFX(GameObject fxPrefab, Transform spawnPoint, Color color)
    {
        GameObject fx = pooling.CreateObject(fxPrefab, spawnPoint);

        fx.transform.position = spawnPoint.position;
        fx.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);

        fx.SetActive(true);

        ParticleSystem ps = fx.GetComponentInChildren<ParticleSystem>();

        if (ps == null) return;

        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        ps.Clear(true);

        var main = ps.main;
        main.startColor = color;

        ps.Simulate(0f, true, true);
        ps.Play(true);
    }

    /*
    public void PlayFX(GameObject fxPrefab, Transform spawnPoint, Color color)
    {
        GameObject fx = pooling.CreateObject(fxPrefab,spawnPoint);
        fx.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        ParticleSystem ps = fx.GetComponent<ParticleSystem>();


        if (ps != null)
        {
            var main = ps.main;

            main.startColor = color;

            ps.Clear();
            ps.Play();
        }
    }
    */
}