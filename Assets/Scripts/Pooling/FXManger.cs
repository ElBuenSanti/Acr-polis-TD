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
        GameObject fx = pooling.CreateObject(fxPrefab,spawnPoint);
        ParticleSystem ps = fx.GetComponent<ParticleSystem>();


        if (ps != null)
        {
            var main = ps.main;

            main.startColor = color;

            ps.Clear();
            ps.Play();
        }
    }
}