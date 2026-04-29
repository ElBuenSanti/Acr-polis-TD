using UnityEngine;

public class WaveDebugInput : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            WaveSpawner.Instance.StartWave(); //COPUIAR Y PEGAR INICIO DE OLEDA
            Debug.Log("Inicio Oleada");
        }
    }
}
