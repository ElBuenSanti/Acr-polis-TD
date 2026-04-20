using UnityEngine;

public class WaveDebugInput : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            WaveSpawner.Instance.StartWave();
            Debug.Log("Inicio Oleada");
        }
    }
}
