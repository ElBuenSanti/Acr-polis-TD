using System.Collections;
using UnityEngine;

public class GameStartMessageUI : MonoBehaviour
{
    // LÍNEA RARA / COMPLEJA: Inicialización de Ciclo de Vida Convertida en Subrutina Asíncrona Diferida ('Start').
    // Unity permite por arquitectura que el método 'Start' devuelva un tipo 'IEnumerator' en lugar de 'void'. Al hacer esto, el motor 
    // transforma automáticamente la inicialización en una corrutina nativa. Al arrancar la escena, suspende su ejecución inmediatamente 
    // durante '0.5f' segundos (medio segundo de tiempo real) mediante 'yield return new WaitForSeconds'. Este retraso estratégico garantiza 
    // que el resto de subsistemas críticos del juego (gráficos, managers, Singletons de UI) se hayan cargado por completo en memoria, 
    // evitando excepciones de puntero nulo ('NullReferenceException') al intentar imprimir el banner inicial en pantalla.
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);

        if (StatusMessageUI.Instance != null)
        {
            StatusMessageUI.Instance.ShowMessage(
                "Construye defensas antes de iniciar la primera oleada."
            );
        }
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el **Disparador del Anuncio Introductorio de Interfaz de Usuario** (`GameStartMessageUI`). Su única y 
   exclusiva responsabilidad dentro de la arquitectura del software es servir como un componente autónomo de bienvenida. Al iniciar 
   la partida, espera un breve instante de estabilización y posteriormente inyecta un texto instructivo y tutorial dentro del sistema 
   flotante de notificaciones globales (`StatusMessageUI`) para guiar los primeros pasos del jugador antes de que comience la acción.

   Características arquitectónicas clave:
   1. Optimización del Flujo de Inicialización (Delayed Execution): Modificar la firma del método `Start` para que actúe como 
      corrutina es una excelente práctica para sistemas de UI. Evita el bloqueo del hilo principal de renderizado y permite dar un 
      pequeño margen de tiempo (0.5 segundos) para que las transiciones de pantalla o desvanecimientos iniciales hayan concluido.
   2. Consumo de Interfaz Desacoplado por Singleton: El script no tiene referencias directas a elementos visuales de texto (`TextMeshPro`), 
      paneles ni animadores. Se comunica de forma completamente limpia a través de la instancia estática global `StatusMessageUI.Instance`. 
      Esto permite que el mensaje se envíe correctamente sin importar cómo esté diseñado el panel de notificaciones por detrás.
   3. Blindaje de Puntero Nulo (Null Safety): Al encerrar la llamada dentro de la compuerta condicional `if (Instance != null)`, el 
      componente se autoprotege. Si por razones de depuración o pruebas de desarrollo se carga una escena de juego sin la interfaz 
      de usuario correspondiente, el script aborta en silencio de forma segura sin arrojar errores en la consola de Unity.
   ========================================================================================================
*/