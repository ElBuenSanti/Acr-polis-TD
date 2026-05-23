using UnityEngine;

public class WaveDebugInput : MonoBehaviour
{
    // Monitorea de forma continua el búfer de entrada de hardware para detectar atajos de teclado en tiempo de desarrollo
    // LÍNEA RARA / COMPLEJA: Intercepción del Búfer de Teclado en el Bucle Principal para Inyección Dinámica de Eventos de Juego ('Update').
    // Se ejecuta de manera síncrona en cada frame del ciclo de vida del motor. Evalúa mediante la instrucción condicional 'Input.GetKeyDown(KeyCode.S)' 
    // si el interruptor mecánico de la tecla 'S' fue presionado en este fotograma exacto. Al detectar el flanco de subida del pulso, se salta el flujo 
    // convencional de temporizadores e intercepta el Singleton global del generador de enemigos ejecutando 'WaveSpawner.Instance.StartWave()'. 
    // Esto actúa como una puerta trasera de depuración ("Debug Backdoor"), permitiendo al equipo de control de calidad forzar el inicio inmediato 
    // de las hordas de combate sin cumplir los requisitos previos de la interfaz o la economía interna.
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            WaveSpawner.Instance.StartWave(); //COPUIAR Y PEGAR INICIO DE OLEDA
            Debug.Log("Inicio Oleada");
        }
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el **Inyector de Comandos y Atajo de Depuración para el Ciclo de Oleadas** (`WaveDebugInput`). 
   Su única y exclusiva responsabilidad en el flujo de desarrollo es servir como una herramienta de atajo (Hot Dialog / Cheat Key) 
   para el equipo de programación y QA (Quality Assurance), permitiendo omitir las fases de espera (tiempos de preparación, compras o transiciones) 
   e iniciar las fases de combate de forma instantánea mediante hardware.

   Características arquitectónicas clave:
   1. Arquitectura de Atajos de Desarrollo (Debug Sandboxing): Este componente está diseñado para ser desacoplado o desactivado 
      completamente en la versión final de producción (Release Build). Al centralizar el comando en un script aislado en lugar de 
      ensuciar el código del `WaveSpawner` con entradas de teclado locales, se previene que los jugadores finales ejecuten trucos o 
      provoquen fallos de sincronización en el ejecutable final.
   2. Interconexión por Acoplamiento Monolítico Seguro (Singleton Consumption): El script consume el patrón de diseño Singleton 
      (`WaveSpawner.Instance`). Al no almacenar variables locales, punteros persistentes ni estados internos del juego, el componente 
      se mantiene extremadamente ligero y agnóstico, limitándose a enviar un único pulso de activación remota en el frame exacto de la pulsación.
   3. Captura Síncrona por Interrupción de Polling (Input Buffer Optimization): Al usar `GetKeyDown` dentro del método nativo `Update`, 
      el motor de Unity lee directamente el estado del periférico mapeado en el frame actual. Esto garantiza una respuesta táctil 
      e inmediata de la llamada a la acción, evitando que el comando de depuración se ejecute múltiples veces por error si el desarrollador 
      mantiene la tecla presionada por más de un fotograma.
   ========================================================================================================
*/