using UnityEngine;

public class TimeScaleInitializer : MonoBehaviour
{
    // Método de ciclo de vida de Unity que se ejecuta automáticamente al iniciar la escena
    private void Start()
    {
        // LÍNEA RARA / COMPLEJA: 'Time.timeScale' controla la velocidad a la que transcurre el tiempo en el motor de Unity.
        // Un valor de '1f' significa velocidad normal (tiempo real). Si se configurara en '0.5f', el juego correría en cámara lenta (a la mitad).
        // Si se configurara en '0f', el juego se pausaría por completo (congelando físicas, animaciones y temporizadores basados en DeltaTime).
        // REGLA DE ORO: Cuando pausas el juego cambiando el timeScale a 0, este valor se queda guardado en la memoria global del motor. 
        // Si cambias de escena o reinicias el nivel, ¡el juego seguirá pausado! Este script asegura restablecer el tiempo al iniciar.
        Time.timeScale = 1f;
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como un "Restablecedor o Inicializador Temporal" (TimeScaleInitializer) especializado para escenas.

   Características clave:
   1. Garantía de Continuidad: Su único y vital propósito es asegurar que el juego "despierte" a velocidad normal (100%) 
      en cuanto la escena actual sea cargada por el motor de Unity.
   2. Solución a Bugs de Pausa: Resuelve de forma limpia un problema clásico en el desarrollo de videojuegos: cuando un jugador 
      pausa la partida (poniendo la escala de tiempo en 0) y decide salir al menú principal o reiniciar el nivel desde la interfaz, 
      la nueva escena se carga congelada. Colocar este script en el arranque del mapa fuerza al motor a reanudar el flujo del tiempo.
   3. Código Seguro y Ultra-ligero: No almacena datos en memoria, no requiere configuraciones en el inspector y se destruye 
      lógicamente su procesamiento en cuanto cumple su función en el primer frame del nivel.
   ========================================================================================================
*/