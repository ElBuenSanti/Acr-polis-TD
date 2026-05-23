using System.Collections;
using UnityEngine;

public class TutorialAutoStarter : MonoBehaviour
{
    [SerializeField] private float delayBeforeOpen = 0f;

    // LÍNEA RARA / COMPLEJA: Sobrecarga del Ciclo de Vida de Unity mediante Corrutina Automática ('Start').
    // Al declarar el método de inicio nativo 'Start' con el tipo de retorno 'IEnumerator', Unity lo ejecuta 
    // automáticamente como una corrutina en lugar de un método síncrono secuencial clásico. Esto le permite al objeto 
    // suspender su propio hilo de ejecución en el primer frame y ceder el control al motor físico sin necesidad de 
    // invocar explícitamente el método auxiliar 'StartCoroutine()'.
    private IEnumerator Start()
    {
        // LÍNEA RARA / COMPLEJA: Retraso Estricto de un Frame de Renderizado para Sincronización Lógica ('yield return null').
        // Detiene la ejecución del script por un frame completo, reanudándola en el siguiente ciclo justo antes de las funciones 
        // de actualización. Esta pausa técnica es crucial para garantizar que todas las instancias Singleton de la UI de la escena 
        // (como 'TutorialPanelUI.Instance') hayan ejecutado por completo sus propios métodos 'Awake' e inicializaciones de datos, 
        // previniendo de manera absoluta excepciones de puntero nulo por carrera de ejecución en el inicio de la escena.
        yield return null;

        if (TutorialPanelUI.Instance != null &&
            TutorialPanelUI.Instance.ShouldShowTutorial())
        {
            TutorialPanelUI.Instance.Open();
        }
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Iniciador Automatizado y Diferido del Tutorial (TutorialAutoStarter). Es un componente utilitario 
   de automatización de interfaz de usuario diseñado para disparar la apertura de ventanas de guía o aprendizaje al cargar la 
   escena, interactuando directamente con el Singleton controlador de la UI (`TutorialPanelUI`).

   Características clave:
   1. Prevención de Condiciones de Carrera (Race Conditions): Al utilizar un retraso de un cuadro (`yield return null`), 
      el script soluciona de forma elegante el clásico problema de Unity donde un objeto intenta acceder a una instancia 
      Singleton en el mismo instante exacto en que esta se está registrando en memoria, logrando un flujo determinista.
   2. Consulta Lógica y Persistencia Condicional: Antes de forzar la apertura del lienzo visual, realiza una doble auditoría: 
      primero valida la existencia del gestor gráfico en el entorno (`!= null`) y, posteriormente, lee el método de bandera 
      `ShouldShowTutorial()`. Esto evita molestar al usuario si este ya ha completado las misiones de aprendizaje o si ha 
      marcado la casilla de omitir guías en configuraciones anteriores.
   3. Arquitectura Limpia y Desacoplada: No requiere un bucle de actualización constante en el método `Update`, lo que 
      significa que una vez que evalúa y ejecuta su función condicional única en el segundo cuadro del juego, el hilo de 
      la corrutina termina de forma natural, dejando el script durmiente con un consumo de procesamiento virtualmente nulo.
   ========================================================================================================
*/