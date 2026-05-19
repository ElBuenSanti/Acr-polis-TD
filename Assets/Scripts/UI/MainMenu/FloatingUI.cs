using UnityEngine;

public class FloatingUI : MonoBehaviour
{
    [SerializeField] private float amplitude = 8f;
    [SerializeField] private float frequency = 1f;

    private Vector3 startPosition;

    // Almacena las coordenadas locales de inicio para tener un punto de referencia de oscilación
    private void Start()
    {
        startPosition = transform.localPosition;
    }

    // Calcula y actualiza continuamente la posición vertical del elemento de interfaz
    private void Update()
    {
        // LÍNEA RARA / COMPLEJA: Algoritmo de oscilación armónica usando la función trigonométrica 'Mathf.Sin' (Seno).
        // El seno es una función matemática que devuelve un valor que sube y baja suavemente de forma cíclica entre -1 y 1.
        // Multiplicamos 'Time.unscaledTime' por la frecuencia para determinar qué tan rápido se repite el ciclo de subida y bajada.
        // Al final, multiplicamos todo por la amplitud para definir cuántos píxeles o unidades físicas se moverá hacia arriba y hacia abajo.
        // REGLA DE ORO: Usamos 'Time.unscaledTime' (tiempo transcurrido real del sistema) en lugar del tiempo físico del juego. 
        // Gracias a esto, si el jugador abre el menú de pausa y el mundo se detiene por completo, las monedas del HUD, las flechas indicadoras 
        // o los textos flotantes seguirán flotando de manera fluida y elegante en la pantalla sin congelarse.
        Vector3 offset = Vector3.up *
            Mathf.Sin(Time.unscaledTime * frequency) *
            amplitude;

        transform.localPosition = startPosition + offset;
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como un "Efecto de Flotación Estética para Interfaces" (FloatingUI). Su función principal es 
   añadir un movimiento de vaivén u oscilación vertical suave a cualquier elemento visual (iconos, textos, carteles de HUD) 
   para darle un aspecto más dinámico, vivo y profesional al juego.

   Características clave:
   1. Animación Matemática Pura (Onda Senoidal): En lugar de crear complejas hojas de animación o usar herramientas de terceros, 
      utiliza trigonometría básica en código para simular una flotación perfecta que consume una cantidad de recursos 
      prácticamente nula para el procesador.
   2. Independencia del Tiempo de Juego (Unscaled Time): Al basar el avance de la onda senoidal en 'Time.unscaledTime', 
      el componente garantiza un comportamiento constante. Los menús flotantes se mantendrán flotando y reaccionando 
      incluso en situaciones donde el juego esté en cámara lenta o completamente congelado por una pantalla de pausa o Game Over.
   3. Respeto al Espacio Local: Modifica la propiedad 'transform.localPosition' sumando el desfasamiento a la posición de arranque. 
      Esto permite que, si el contenedor de la interfaz se mueve, se estira o cambia de resolución, el elemento seguirá 
      flotando de manera correcta relativo a su padre sin desalinearse ni romperse en la pantalla de juego.
   ========================================================================================================
*/