using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : NavMeshAgentBehaviour
{
    [SerializeField] protected EnemyData data;

    public Transform currentTarget;
    public Transform mainTarget;
    public Transform combatTarget;
    private Temple temple;
    private float maxResistance;

    private float combatDistance;
    private float distance;

    protected float actionVelocity;
    protected float attackRange;
    protected float searchRange = 20f;

    [SerializeField] private float combatRange = 5f;
    [SerializeField] private float searchInterval = 1.5f;
    [SerializeField] private float stunTime = 1f;
    [SerializeField] private float deathTime = 1.5f;
    [SerializeField] private float winningTime = 2f;
    [SerializeField] private float loosingTime = 2f;
    protected float lastAttackTime;


    // Enemy Creation
    // Invocación explícita del constructor base para heredar la configuración física y asignación estricta al bando hostil
    protected override void Awake()
    {
        base.Awake();
        SetTeam(Team.Enemy);

    }


    protected override void ShootParticles()
    {
        colorOfParticles = new Color(1f, 0f, 0.5f);
        base.ShootParticles();
    }

    // Desempaqueta el contenedor ScriptableObject e inyecta dinámicamente las estadísticas de combate en el agente de navegación
    public virtual void Initialize(EnemyData newData)
    {
        data = newData;
        resistance = data.resistance;
        attackDamage = data.attackDamage;
        movementSpeed = data.movementSpeed;
        actionVelocity = data.actionVelocity;
        attackRange = data.attackRange;
        maxResistance = data.resistance;

        Debug.Log($"Enemigo Inicializado: Resistencia: {data.resistance}, danio: {data.attackDamage}");

        if (agent != null)
            agent.speed = movementSpeed;
    }

    // Inicializa el estado de punteros de objetivos y arranca la máquina de estados asíncrona de escaneo perimetral
    protected override void OnEnable()
    {
        base.OnEnable();

        currentTarget = null;
        mainTarget = null;
        combatTarget = null;
        temple = GetTemple();

        StartCoroutine(SearchTargetRoutine());
    }

    // Invoca la limpieza de hilos y referencias de la clase abstracta padre al desactivar la entidad en la escena
    protected override void OnDisable()
    {
        base.OnDisable();
    }

    // LÍNEA RARA / COMPLEJA: Máquina de Decisiones de Combate Basada en Jerarquía de Objetivos Activos ('Update').
    // Evalúa preventivamente flags de control de la IA. Si posee una amenaza cuerpo a cuerpo ('combatTarget'), calcula su magnitud escalar 
    // al cuadrado y la compara contra el rango de combate; si excede el umbral, rompe el enganche táctico asignando 'null'. Acto seguido, 
    // resuelve la precedencia de objetivos mediante una estructura condicional encadenada: si el objetivo de combate existe y está activo 
    // en la escena, se convierte en la prioridad absoluta; si no, degrada la atención hacia el objetivo estructural principal ('mainTarget'). 
    // Si ambos son nulos, aborta el ciclo. Finalmente, evalúa la distancia frente al objetivo elegido: si está dentro del rango de ataque, 
    // congela al agente NavMesh y ejecuta el método de asalto; de lo contrario, reactiva la locomoción hacia sus coordenadas.
    void Update()
    {
        if (IsDead || IsStunned)
            return;

        if (combatTarget != null)
        {
            combatDistance = CalculateDistance(combatTarget);

            if (combatDistance > combatRange * combatRange)
                combatTarget = null;
        }

        if (combatTarget != null && combatTarget.gameObject.activeInHierarchy)
        {
            currentTarget = combatTarget;
        }
        else if (mainTarget != null && mainTarget.gameObject.activeInHierarchy)
        {
            currentTarget = mainTarget;
        }
        else
        {
            return;
        }

        distance = CalculateDistance(currentTarget);

        if (distance <= attackRange * attackRange)
        {
            agent.isStopped = true;
            Attack(currentTarget);
        }
        else
        {
            agent.isStopped = false;
            MoveTo(currentTarget.position);
        }
    }

    // Functions 

    // LÍNEA RARA / COMPLEJA: Optimización de Rendimiento CPU Mediante Magnitud Vectorial Cuadrática ('CalculateDistance').
    // Ejecuta una operación matemática de sustracción posicional entre el origen del enemigo y el transformador del objetivo. 
    // En lugar de calcular la distancia lineal usando 'Vector3.Distance' (que internamente ejecuta una costosa raíz cuadrada basada en el 
    // teorema de Pitágoras), extrae la propiedad '.sqrMagnitude'. Al conservar los valores elevados al cuadrado, permite realizar 
    // comparaciones de proximidad de forma idéntica pero con un consumo de procesamiento drásticamente inferior, vital para optimizar hordas.
    float CalculateDistance(Transform target)
    {
        return (transform.position - target.position).sqrMagnitude;
    }

    // Controla la cadencia de fuego o golpes aplicando un temporizador basado en el reloj del motor frente a la velocidad de acción
    protected virtual void Attack(Transform currentTarget)
    {
        if (Time.time < lastAttackTime + actionVelocity)
            return;

        lastAttackTime = Time.time;

        if (currentTarget == null)
            return;

        if (currentTarget.TryGetComponent<IDamageable>(out var damagable))
        {
            damagable.ReceiveDamage(attackDamage);

            if (GameplaySoundPlayer.Instance != null)
                GameplaySoundPlayer.Instance.PlayEnemyHit();
            // Debug.Log("Atacando al targt del enemigo");

        }
    }

    // Dispara las alertas visuales y auditivas de recepción de daño interrumpiendo el flujo de comportamiento estándar
    protected override void OnDamage()
    {
        base.OnDamage();
        // AQUÍ ANIMACIÓN DE DANIO DE ENEMIG
        // Debug.Log("El enemigo recibió danio");
        if (GameplaySoundPlayer.Instance != null)
            GameplaySoundPlayer.Instance.PlayEnemyHit();
    }


    // Limpia las referencias de persecución activa y orquesta la secuencia asíncrona de remoción física de la horda
    protected override void OnDeath()
    {
        currentTarget = null;
        // AQUÍ ANIMACIÓN DE MUERTE DE ENEMIGO
        if (GameplaySoundPlayer.Instance != null)
            GameplaySoundPlayer.Instance.PlayEnemyDeath();
        Debug.Log("enemigo Murió");
        base.OnDeath();
    }


    // Get time of...
    protected override float GetStunTime()
    {
        return stunTime;
    }

    protected override float GetDeathTime()
    {
        return deathTime;
    }

    protected override float GetWinningTime()
    {
        return winningTime;
    }

    protected override float GetLoosingTime()
    {
        return loosingTime;
    }


    // Intercepta la conclusión de la fase de asalto para desviar la máquina de estados a secuencias cosméticas de victoria o derrota
    protected override void HandleWaveEnd()
    {
        base.HandleWaveEnd();

        if (IsTempleAlive())
        {
            OnLoosingWave();
        }
        else
        {
            OnWinningWave();
        }
    }

    protected override void OnLoosingWave()
    {
        base.OnLoosingWave();
        // AQUÍ AIMACIÓN DE DERROTA DE ENEMIGO
    }

    protected override void OnWinningWave()
    {
        base.OnWinningWave();
        // AQUÍ AIMACIÓN DE VICTORIA DE ENEMIGO
    }

    protected override float GetMaxHealth()
    {
        return maxResistance;
    }
    protected override void Disappear()
    {
        base.Disappear();
        Color redColor = new Color(1f, 0f, 0f);
        FXManager.Instance.PlayFX(FXManager.Instance.particulesEffects, transform, colorOfParticles);
    }



    // Cooroutine
    // LÍNEA RARA / COMPLEJA: Escaneo Periódico de Sub-Procesamiento para la Mitigación de Sobrecarga de IA ('SearchTargetRoutine').
    // Bucle asíncrono condicionado a la vitalidad del agente que optimiza la carga computacional en hordas masivas de enemigos. En lugar de 
    // forzar un escaneo de entorno en cada frame dentro de 'Update', distribuye de forma diferida las consultas espaciales suspendiendo la 
    // ejecución según los segundos definidos en 'searchInterval'. En cada pulso del ciclo, utiliza el componente 'targetFinder' de forma doble: 
    // busca estructuras inmóviles ('BaseConstruction') en un rango amplio de 20 unidades y, en paralelo, rastrea amenazas defensivas móviles 
    // de tipo soldados de infantería ('HandToHandSoldier') en un área reducida, actualizando los punteros tácticos de manera controlada.
    IEnumerator SearchTargetRoutine()
    {
        while (isActiveAndEnabled && !IsDead)
        {
            if (IsStunned)
            {
                yield return new WaitForSeconds(searchInterval);
                continue;
            }

            mainTarget = targetFinder.FindTarget<BaseConstruction>(transform, searchRange);

            combatTarget = targetFinder.FindTarget<HandToHandSoldier>(transform, combatRange);

            yield return new WaitForSeconds(searchInterval);
        }
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el **Controlador Maestro de Inteligencia Artificial y Comportamiento Hostil** (`Enemy`). 
   Heredando de la clase especializada `NavMeshAgentBehaviour`, se acopla directamente con el motor de navegación de Unity 
   para resolver de forma reactiva la persecución, el cálculo de rutas óptimas y el asalto a las infraestructuras de defensa 
   aliadas del jugador. Implementa un sistema jerárquico de prioridades de combate y optimizaciones algebraicas estrictas.

   Características arquitectónicas clave:
   1. Jerarquía Dinámica de Objetivos (Target Precedence): La IA posee un comportamiento cognitivo de dos niveles. Su objetivo general 
      o estratégico es arrasar con los edificios (`mainTarget`), pero si una unidad de infantería aliada (`HandToHandSoldier`) entra en su 
      rango inmediato de combate, cambia instantáneamente su atención hacia ella (`combatTarget`) para defenderse, regresando a su ruta 
      original en caso de neutralizar la amenaza o perder su rastro.
   2. Mitigación de Costo de Cómputo (Fórmula SqrMagnitude): Las distancias tridimensionales entre el enemigo y sus objetivos se evalúan 
      excluyendo las costosas operaciones de raíces cuadradas del procesador. Al comparar contra el rango de ataque elevado al cuadrado 
      (`attackRange * attackRange`), se reduce masivamente el impacto computacional del bucle `Update`, permitiendo procesar cientos de 
      unidades en pantalla de forma simultánea.
   3. Desacoplamiento de Animación e Interfaz de Daños: El flujo mecánico del combate está completamente separado de la capa visual. 
      El script utiliza delegados de ciclo de vida (`OnDamage`, `OnDeath`, `HandleWaveEnd`) dejando espacios seguros listos para la 
      inyección procedural de animaciones y partículas, mientras transfiere el daño de forma atómica a través del contrato `IDamageable`.
   ========================================================================================================
*/