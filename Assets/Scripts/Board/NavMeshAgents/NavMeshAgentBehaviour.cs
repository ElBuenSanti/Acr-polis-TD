using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class NavMeshAgentBehaviour : TeamAssigner, IDamageable
{
    protected NavMeshAgent agent;
    protected TargetFinder targetFinder;

    protected Coroutine stunCoroutine;
    protected Coroutine disappearRoutineCoroutine;

    private HealthBarHolder healthBarHolder;

    public static List<NavMeshAgentBehaviour> AllUnits = new List<NavMeshAgentBehaviour>();

    protected float resistance;
    protected float attackDamage;
    protected float movementSpeed;


    protected bool waveEnded;

    public bool IsDead { get; protected set; }
    public bool IsStunned { get; protected set; }


    // Nav Mesh Agents Creation
    // Cachea los componentes físicos esenciales de navegación e interfaz HUD sobreelevada asignando un radio rígido al obstáculo agente
    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.radius = 0.8f;
        targetFinder = FindAnyObjectByType<TargetFinder>();
        healthBarHolder = GetComponent<HealthBarHolder>();
    }

    // LÍNEA RARA / COMPLEJA: Inyección del Agente en el Registro de Grafo Global y Suscripción del Observador de Estado de Eventos ('OnEnable').
    // Al activarse el componente en la escena, indexa de forma atómica su propia referencia virtual dentro de la lista estática globalizada 
    // 'AllUnits'. Acto seguido, establece un acoplamiento laxo preventivo mediante el patrón de diseño Observer (Observador): si el gestor 
    // 'WaveSpawner.Instance' se encuentra activo en memoria, asocia mediante asignación compuesta ('+=') el método delegado local 
    // 'HandleWaveEnd' al evento de difusión pública 'OnWaveEnded'. Esto garantiza que la unidad reaccione instantáneamente al cierre 
    // de la oleada sin realizar consultas cíclicas (polling) en el bucle principal.
    protected virtual void OnEnable()
    {
        AllUnits.Add(this);
        waveEnded = false;

        if (WaveSpawner.Instance != null)
            WaveSpawner.Instance.OnWaveEnded += HandleWaveEnd;

        IsDead = false;
        IsStunned = false;

        if (agent != null)
        {
            agent.isStopped = false;
            agent.ResetPath();
        }

    }

    // LÍNEA RARA / COMPLEJA: Remoción del Registro Estático y Desvinculación de Delegados de Memoria Volátil ('OnDisable').
    // Invocado de forma automática cuando el objeto se apaga o retorna al Pool de reciclaje. Ejecuta una rutina estricta de limpieza física 
    // removiéndose a sí mismo de la colección 'AllUnits' para evitar fugas de memoria o referencias muertas (null pointer ghosting). 
    // Adicionalmente, desvincula mediante sustracción ('-=') el método delegado del evento global de oleadas y ejecuta 'StopAllCoroutines()', 
    // interrumpiendo abruptamente subprocesos asíncronos activos de aturdimiento o desaparición diferida, blindando la integridad de hilos en CPU.
    protected virtual void OnDisable()
    {
        AllUnits.Remove(this);

        if (WaveSpawner.Instance != null)
            WaveSpawner.Instance.OnWaveEnded -= HandleWaveEnd;

        StopAllCoroutines();
    }

    // Functions

    // Movement
    // Despacha una orden directa al resolvedor cinemático de rutas de Unity inyectando un vector tridimensional de destino si el agente está libre
    protected virtual void MoveTo(Vector3 destination)
    {
        if (agent == null || IsDead || IsStunned)
            return;

        agent.SetDestination(destination);
    }


    // Damage and deactivation
    // Procesa los decrementos matemáticos de salud coordinando la actualización de la barra de vida e interceptando el umbral crítico de colapso
    public virtual void ReceiveDamage(float damage)
    {
        if (IsDead)
        {
            return;
        }

        resistance -= damage;

        if (healthBarHolder != null)
            healthBarHolder.UpdateHealth(resistance, GetMaxHealth());

        if (resistance <= 0)
        {
            IsDead = true;
            OnDeath();
            return;
        }
        OnDamage();
    }

    // Interrumpe la locomoción e inicia la máquina de estados asíncrona de aturdimiento pisando cualquier temporizador de hit previo
    protected virtual void OnDamage()
    {
        if (IsDead)
            return;

        IsStunned = true;

        if (agent != null)
            agent.isStopped = true;

        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);

        stunCoroutine = StartCoroutine(StunnedRoutine(GetStunTime()));
    }

    // Orquesta la congelación estructural de la unidad disparando el temporizador diferido de remoción visual
    protected virtual void OnDeath()
    {
        PreparingAgentToDisappear();

        disappearRoutineCoroutine = StartCoroutine(DisappearRoutine(GetDeathTime()));
    }

    // Detiene de forma absoluta las subrutinas temporales de comportamiento y bloquea las velocidades físicas del NavMesh
    protected virtual void PreparingAgentToDisappear()
    {
        if (agent != null)
            agent.isStopped = true;

        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);

        if (disappearRoutineCoroutine != null)
            StopCoroutine(disappearRoutineCoroutine);
    }

    // Desactiva la presencia física del componente en la jerarquía activa del motor
    protected virtual void Disappear()
    {
        gameObject.SetActive(false);
    }

    // Get Time of...

    protected virtual float GetStunTime()
    {
        return 0;
    }

    protected virtual float GetDeathTime()
    {
        return 0;
    }

    protected virtual float GetWinningTime()
    {
        return 0;
    }

    protected virtual float GetLoosingTime()
    {
        return 0;
    }

    // When wave ends, do...
    protected virtual void HandleWaveEnd()
    {
        if (IsDead) return;
        waveEnded = true;
    }

    protected virtual void OnWinningWave()
    {
        PreparingAgentToDisappear();
        disappearRoutineCoroutine = StartCoroutine(DisappearRoutine(GetWinningTime()));
    }

    protected virtual void OnLoosingWave()
    {
        PreparingAgentToDisappear();
        disappearRoutineCoroutine = StartCoroutine(DisappearRoutine(GetLoosingTime()));
    }

    // LÍNEA RARA / COMPLEJA: Resolución Polimórfica Lineal del Eje Central Estructural ('GetTemple').
    // Itera secuencialmente sobre la colección global 'BaseConstruction.AllConstructions'. Utiliza el operador de emparejamiento 
    // de patrones de C# 'is' para evaluar dinámicamente si el elemento actual hereda o comparte el tipo específico de la clase 'Temple'. 
    // En el instante exacto en que la aserción resulta verdadera, realiza un casteo implícito atómico asignando la referencia a la variable 
    // local 'temple' y la devuelve de inmediato, permitiendo a la entidad localizar el núcleo del mapa sin dependencias rígidas (hardcoded).
    protected Temple GetTemple()
    {
        foreach (var c in BaseConstruction.AllConstructions)
        {
            if (c is Temple temple)
                return temple;
        }

        return null;
    }

    // Verifica si el puntero del templo principal es válido y valida que no se encuentre en un estado destruido o inactivo
    protected bool IsTempleAlive()
    {
        var temple = GetTemple();
        return temple != null && !temple.IsDestroyed;
    }



    // Cooroutines

    // Bucle asíncrono que retiene el estado de invalidez del personaje y restaura el control de movimiento del agente tras el desvanecimiento
    protected IEnumerator StunnedRoutine(float stunTime)
    {
        yield return new WaitForSeconds(stunTime);

        IsStunned = false;

        if (agent != null)
            agent.isStopped = false;

        stunCoroutine = null;
    }

    // Retarda la remoción del objeto de la jerarquía para permitir la conclusión de efectos de partículas o transiciones lógicas
    protected IEnumerator DisappearRoutine(float time)
    {
        yield return new WaitForSeconds(time);

        Disappear();
        disappearRoutineCoroutine = null;
    }

    protected virtual float GetMaxHealth()
    {
        return resistance;
    }

}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como la **Clase Abstracta Maestra e Interfaz del Ciclo de Vida de Agentes NavMesh** (`NavMeshAgentBehaviour`). 
   Su rol dentro de la arquitectura del software es servir como la raíz polimórfica universal de la que deben heredar todas las 
   entidades móviles inteligentes del juego, incluyendo tanto a las hordas atacantes (`Enemy`) como a las tropas aliadas que defienden 
   el mapa (`HandToHandSoldier`). Consolida la locomoción guiada de Unity, el contrato de daño `IDamageable` y el control de estados.

   Características arquitectónicas clave:
   1. Unificación Polimórfica del Sistema de Daño (Health & Stun States): Administra de manera centralizada la reducción de vida, 
      las llamadas de actualización a los componentes de UI (`HealthBarHolder`) y la máquina de estados de interrupción por daño. Al 
      automatizar las corrutinas de aturdimiento (`StunnedRoutine`), garantiza que cualquier unidad se detenga coherentemente al ser golpeada.
   2. Arquitectura Orientada a Eventos y Desacoplamiento (Event-Driven Design): Gracias a la suscripción nativa al evento `OnWaveEnded` 
      del `WaveSpawner`, las unidades no necesitan consultar constantemente en un Update si la ronda terminó. El sistema les notifica 
      en masa mediante un disparo atómico, reduciendo de manera radical el uso de procesamiento innecesario.
   3. Registro en Grafo Central y Gestión de Ciclo Limpio: Al usar la lista estática `AllUnits`, el software mantiene un índice en 
      tiempo real de todas las criaturas activas en el escenario táctico. Su sistema automático de desregistro en `OnDisable` actúa como 
      un escudo hermético contra las fugas de memoria, asegurando que los objetos desactivados limpien sus hilos por completo.
   ========================================================================================================
*/