using UnityEngine;

public class PulseGlowUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup glow;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float minAlpha = 0.25f;
    [SerializeField] private float maxAlpha = 0.8f;

    // Modula cíclicamente la opacidad del lienzo en cada frame utilizando una función trigonométrica continua
    private void Update()
    {
        // LÍNEA RARA / COMPLEJA: Normalización y Remapeo Matemático de Rango de una Onda Sinusoidal.
        // La función 'Mathf.Sin' oscila de forma matemática natural en un rango de [-1f, 1f].
        // Para transformar esa oscilación en un factor utilizable en un Lerp, se le suma '1f' para mover el rango a [0f, 2f],
        // y luego se multiplica por '0.5f' (o divide entre 2), logrando un rango perfectamente normalizado de [0f, 1f].
        // Finalmente, 'Mathf.Lerp' toma este factor para oscilar suavemente la opacidad entre 'minAlpha' y 'maxAlpha'.
        float pulse = Mathf.Lerp(
            minAlpha,
            maxAlpha,
            (Mathf.Sin(Time.unscaledTime * speed) + 1f) * 0.5f
        );

        // LÍNEA RARA / COMPLEJA: Uso de Tiempo Desacoplado Global ('Time.unscaledTime').
        // En lugar de usar 'Time.time' (que se ve afectado por las pausas o ralentizaciones del juego), utiliza el reloj físico 
        // real de la CPU. Esto garantiza que si el juego se detiene mediante un menú de pausa congelando el tiempo físico, 
        // los efectos visuales ambientales o brillos decorativos del HUD sigan animándose con total fluidez y dinamismo.
        glow.alpha = pulse;
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como un Efecto Visual de Brillo Pulsante Automatizado (PulseGlowUI). Es un componente cosmético 
   altamente eficiente para interfaces de usuario, diseñado para dar dinamismo a elementos destacados del HUD 
   (como botones de acción, indicadores de alerta o marcos decorativos) mediante una modulación lumínica constante.

   Características clave:
   1. Oscilación Matemática Pura sin Corrutinas: Utiliza las propiedades ondulatorias del seno trigonométrico en 
      lugar de procesos iterativos complejos o interpolaciones basadas en corrutinas. Esto reduce el consumo de 
      procesamiento y evita la recolección de basura (Garbage Collection), manteniendo el rendimiento óptimo del HUD.
   2. Inmunidad a Pausas del Motor (Unscaled Time): Al basar su cálculo en 'Time.unscaledTime', el comportamiento 
      gráfico queda completamente blindado ante alteraciones de la escala temporal del software, permitiendo que 
      la interfaz mantenga una respuesta visual "viva" incluso durante pantallas de fin de juego o menús suspendidos.
   3. Manipulación Eficiente mediante CanvasGroup: Modifica directamente el canal alfa del componente 'CanvasGroup' 
      en vez de alterar los componentes individuales de color de múltiples imágenes hijas. Esto permite que un conjunto 
      entero de elementos visuales (bordes, textos, iconos) pulsen al unísono de forma centralizada y optimizada.
   ========================================================================================================
*/