using UnityEngine;

public class GameSceneAudioStarter : MonoBehaviour
{
    // Método de ciclo de vida de Unity que se ejecuta automáticamente en el primer frame donde el objeto está activo
    private void Start()
    {
        // LÍNEA RARA / COMPLEJA: Realiza una validación de seguridad ('!= null') antes de comunicarse con otro script.
        // Si el 'MusicManager' (el encargado global de la música) no existe en la escena o no ha cargado, 
        // esta línea evita que el juego se rompa o lance un error de tipo "NullReferenceException" en la consola de Unity.
        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayConstructionMusic(); // Llama al método encargado de reproducir la música de la fase de construcción
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como un "Gatillo" o Iniciador Automático (Starter) de audio especializado para una escena en específico.

   Características clave:
   1. Automatización al Cargar la Escena: Su único propósito es arrancar de forma automática la música ambiental 
      correspondiente en cuanto el jugador entra al nivel o mapa donde este script se encuentre colocado.
   2. Comunicación Inter-modular: Utiliza el patrón Singleton para conectarse de manera segura con el 'MusicManager', 
      solicitándole que reproduzca la pista musical de la fase de "Construcción".
   3. Código Seguro y Ligero: No almacena datos, no consume recursos de memoria en bucles y está protegido con 
      condicionales para que no genere errores en el flujo del juego si los sistemas globales de audio no están listos.
   ========================================================================================================
*/