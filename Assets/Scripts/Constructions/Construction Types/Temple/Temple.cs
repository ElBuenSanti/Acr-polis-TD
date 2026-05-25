using System;
using UnityEngine;

public class Temple : Plaza
{
    public static Action<GodType> OnGodSelected;
    // private ConstructionController controller;

    public static event Action OnTempleDestruction;
    private GodType selectedGod = GodType.Base;


    // Temple Initialization
    // Invoca de forma segura la cadena de inicialización de la clase base 'Plaza' para registrar la estructura en el mapa
    protected override void Awake()
    {
        base.Awake();
        // controller = GetComponent<ConstructionController>();
    }

    // Remueve las suscripciones e interfaces de control de la estructura llamando a las rutinas de limpieza heredadas
    protected override void OnDisable()
    {
        base.OnDisable();
    }

    // Configura las estadísticas físicas del templo inyectando las propiedades serializadas del contenedor de datos
    public override void Initialize(ConstructionData newData)
    {
        base.Initialize(newData);
    }

    // Functions

    // LÍNEA RARA / COMPLEJA: Difusión del Anuncio de Consagración Teológica mediante Invocación Segura ('NotifyGodSelected').
    // Recibe como parámetro el enumerado 'GodType' y propaga de forma atómica la elección del usuario a través del delegado estático 
    // global 'OnGodSelected?.Invoke(god)'. El uso del operador condicional nulo ('?.') actúa como una compuerta de seguridad: si ningún 
    // subsistema del juego (como el gestor de oleadas 'WaveSpawner' o el generador de jefes) se ha suscrito al evento en este frame, 
    // la instrucción se aborta de forma silenciosa sin arrojar excepciones de puntero nulo, permitiendo mutar la jugabilidad dinámicamente.
    public void NotifyGodSelected(GodType god)
    {
        selectedGod = god;
        blessingChosen = true;

        Debug.Log("El templo fue consagrado al Dios: " + god);
        OnGodSelected?.Invoke(god);
    }


    // LÍNEA RARA / COMPLEJA: Sobrescritura del Flujo de Colapso de Estructura y Propagación del Estado de Derrota Global ('OnDestruction').
    // Reemplaza de forma polimórfica la rutina de destrucción estándar mediante la directiva 'override'. Primero ejecuta 'base.OnDestruction()' 
    // para procesar los efectos visuales de escombros y el desregistro en las listas de construcciones. Inmediatamente después, dispara 
    // de manera síncrona el evento estático 'OnTempleDestruction?.Invoke()'. Al estar declarado con la palabra clave 'event', blinda la 
    // difusión para que ninguna otra clase pueda resetear o falsificar este disparo, notifying instantáneamente al GameManager para que 
    // conmute la lógica de la partida hacia la pantalla de 'Game Over'.
    public override void OnDestruction()
    {
        base.OnDestruction();
        Debug.Log("Has perdido");
        OnTempleDestruction?.Invoke();
    }

    private bool blessingChosen;

    // Devuelve el estado de la bandera de control para verificar si el jugador ya consumió su bonificación divina en esta estructura
    public bool HasBlessingChosen()
    {
        return blessingChosen;
    }

    // Altera de forma irreversible el estado lógico del interruptor una vez que se ha canjeado un poder en el templo
    public void MarkBlessingChosen()
    {
        blessingChosen = true;
    }

    // Provee un canal de lectura público seguro para que controladores externos de UI o sistemas de juego consulten la deidad activa sin mutar el estado
    public GodType GetSelectedGod()
    {
        return selectedGod;
    }

}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el **Núcleo Lógico de Condición de Victoria/Derrota y Consagración del Templo** (`Temple`). 
   Hereda de la clase especializada `Plaza` (que a su vez extiende la lógica base de las construcciones) y se erige como la estructura 
   más crítica en la arquitectura de un juego de defensa de torres o estrategia. Controla mecánicamente la elección de la deidad patrona 
   que afectará el balance y el tipo de jefe final de la partida, además de centralizar el evento absoluto de fin de juego por destrucción.

   Características arquitectónicas clave:
   1. Eventos Estáticos Globales (Static Event Architecture): Al utilizar delegados estáticos (`OnGodSelected` y `OnTempleDestruction`), 
      el Templo se desacopla completamente del resto de sistemas. El script del juego que maneja la UI de fin de partida o el manager que 
      despacha enemigos no necesitan buscar el objeto Templo en la escena mediante operaciones costosas; simplemente escuchan la clase de 
      manera global (`Temple.OnTempleDestruction += MiMetodo`), optimizando el rendimiento.
   2. Polimorfismo y Preservación de Jerarquía (Base Invocation): Al sobreescribir `Awake`, `OnDisable` y `OnDestruction` utilizando `base.`, 
      el software garantiza que las mecánicas heredadas comunes a todos los edificios del mapa (como la actualización de mallas de 
      navegación NavMesh o los costes de recursos) sigan ejecutándose a la perfección antes de procesar las particularidades del Templo.
   3. Control de Estado Único (Blessing State Machine): La gestión de la variable `blessingChosen` actúa como un pestillo de seguridad 
      binario en el gameplay. Evita que el usuario explote el sistema solicitando múltiples bendiciones divinas o modificando la deidad 
      asignada de forma indefinida una vez que se han reclamado las bonificaciones en la partida.
   ========================================================================================================
*/