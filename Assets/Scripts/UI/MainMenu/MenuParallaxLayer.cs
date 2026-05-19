using UnityEngine;

public class MenuParallaxLayer : MonoBehaviour
{
    [SerializeField] private Vector2 moveAmount = new Vector2(15f, 8f);
    [SerializeField] private float speed = 3f;

    private Vector3 startPosition;

    // Almacena la ubicación de origen para usarla como el centro del plano del efecto de profundidad
    private void Start()
    {
        startPosition = transform.localPosition;
    }

    // Calcula de forma continua la distorsión de la capa basándose en el movimiento del mouse
    private void Update()
    {
        // LÍNEA RARA / COMPLEJA: Normalización y Centrado de Coordenadas del Puntero.
        // 'Input.mousePosition' entrega los píxeles directos de la pantalla (ej. de 0 a 1920 en X). Al dividirlo entre 
        // 'Screen.width' (el ancho total de la pantalla), convertimos ese valor a un rango universal de entre 0.0f y 1.0f.
        // REGLA DE ORO: Restarle '0.5f' es un truco matemático crítico. Mueve el "punto de origen (0,0)" desde la esquina inferior izquierda 
        // de la pantalla hacia el centro exacto del monitor. Ahora, si el mouse está a la izquierda el resultado será negativo (hasta -0.5f), 
        // si está a la derecha será positivo (hasta 0.5f) y si está en el centro valdrá exactamente cero.
        Vector2 normalizedMouse = new Vector2(
            Input.mousePosition.x / Screen.width - 0.5f,
            Input.mousePosition.y / Screen.height - 0.5f
        );

        // Calcula el desfasamiento final multiplicando la coordenada normalizada por la fuerza asignada en el inspector
        Vector3 targetPosition = startPosition + new Vector3(
            normalizedMouse.x * moveAmount.x,
            normalizedMouse.y * moveAmount.y,
            0f
        );

        // LÍNEA RARA / COMPLEJA: Amortiguación asíncrona mediante 'Vector3.Lerp' y 'Time.unscaledDeltaTime'.
        // Desplaza la capa desde su posición actual hacia la posición objetivo de forma fluida.
        // Al multiplicar el multiplicador de velocidad por 'Time.unscaledDeltaTime', garantizamos que el fondo del menú se mueva 
        // de manera suave y orgánica a la misma velocidad en cualquier computadora, sin importar los FPS o si el juego de fondo está pausado.
        transform.localPosition = Vector3.Lerp(
            transform.localPosition,
            targetPosition,
            Time.unscaledDeltaTime * speed
        );
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Controlador de Efecto Paralaje para Menús (MenuParallaxLayer). Su propósito fundamental 
   es generar una ilusión óptica de tridimensionalidad y profundidad ("Parallax") en interfaces en 2D, haciendo que las capas 
   de fondo, decoraciones o ilustraciones se desplacen sutilmente siguiendo el movimiento del puntero del ratón.

   Características clave:
   1. Generación de Efecto de Profundidad de Campo: Al colocar este script en múltiples imágenes del fondo del menú 
      (por ejemplo: una capa para las montañas lejanas, otra para los árboles y otra para el marco del HUD) y configurarles 
      valores de desplazamiento ('moveAmount') diferentes a cada una, los objetos más cercanos se moverán más rápido que 
      los lejanos, simulando un entorno 3D vivo y envolvente.
   2. Conversión Matemática Independiente de Resoluciones: Gracias a la normalización matemática del mouse dividida entre 
      las dimensiones de la pantalla ('Screen.width' y 'Screen.height'), el efecto visual se comportará exactamente igual y 
      con la misma proporción de movimiento ya sea que el jugador ejecute el juego en un monitor a 1080p, 4K o pantallas ultra-anchas.
   3. Renderizado y Desplazamiento Fluido (Lerp): Evita saltos bruscos en las texturas de la interfaz. Si el mouse se mueve de 
      un extremo a otro de la pantalla de forma súbita, la interpolación lineal actúa como una suspensión amortiguada, deslizando 
      los elementos estéticos con elegancia cinematográfica.
   ========================================================================================================
*/