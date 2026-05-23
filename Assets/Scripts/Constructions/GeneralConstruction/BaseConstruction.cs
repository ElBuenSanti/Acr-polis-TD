using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseConstruction : TeamAssigner, IDamageable
{
    protected ConstructionData data;
    public ConstructionData Data => data;

    private NavMeshObstacle obstacle;

    protected Tile parentTile;
    protected Pooling pooling;

    protected float resistance;
    //protected float actionVelocity;
    protected float cost;
    protected float timeToSpawnAditaments;

    protected Coroutine spawnCoroutine;
    protected Coroutine destructionRoutine;
    protected Coroutine waveRoutine;

    protected float timeToBeDestroyed;

    private HealthBarHolder healthBarHolder;

    public bool IsDestroyed { get; protected set; }

    public static List<BaseConstruction> AllConstructions = new List<BaseConstruction>();



    // Construction Creation
    // Inyecta el contenedor de configuración, resetea las subrutinas temporales y reconstruye las variables físicas
    public virtual void Initialize(ConstructionData newData)
    {
        data = newData;
        SetTeam(Team.Ally);
        //timeToSpawnAditaments = data.actionVelocity;
        StopAllCoroutines();

        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        ApplyStats();
        SetupNavObstacle();
    }

    // Cachea componentes locales esenciales como el gestor de reciclaje e interfaz de barras de vida
    protected virtual void Awake()
    {
        pooling = FindAnyObjectByType<Pooling>();
        healthBarHolder = GetComponent<HealthBarHolder>();
    }

    // LÍNEA RARA / COMPLEJA: Registro Centralizado en Matriz Estática y Habilitación de Moduladores de Entorno ('OnEnable').
    // Al activarse la entidad en la escena, esta se auto-indexa en la colección global estática 'AllConstructions'. 
    // Esto permite que sistemas externos (como la validación de prerrequisitos del 'WaveSpawner') auditen la presencia 
    // de defensas mediante consultas lineales directas en memoria compartida, eludiendo costosas búsquedas dinámicas 
    // en la jerarquía del motor de juego. Simultáneamente, activa el obstáculo físico si este ya se encuentra instanciado.
    protected virtual void OnEnable()
    {
        AllConstructions.Add(this);
        IsDestroyed = false;

        if (obstacle != null)
            obstacle.enabled = true;
    }

    // LÍNEA RARA / COMPLEJA: Purga Estricta de Referencias Colectivas y Desactivación Secuencial de Hilos ('OnDisable').
    // Remueve de inmediato su propia instancia de la lista estática para evitar fugas de memoria o referencias muertas ('NullReference'). 
    // Interrumpe de golpe todos los bucles de ejecución asíncronos activos invocando 'StopAllCoroutines()', limpia el puntero 
    // del sub-hilo de spawn y apaga el componente 'NavMeshObstacle' para notificar al sistema de navegación que el espacio está libre.
    protected virtual void OnDisable()
    {
        AllConstructions.Remove(this);
        StopAllCoroutines();
        spawnCoroutine = null;

        if (obstacle != null)
            obstacle.enabled = false;
    }




    // Functions

    // Sincroniza los valores numéricos basados en los datos del Asset y actualiza el estado de los componentes visuales del HUD
    protected virtual void ApplyStats()
    {
        resistance = data.resistance;
        timeToSpawnAditaments = data.actionVelocity;
        //actionVelocity = data.actionVelocity;
        Debug.Log($"Construcción Inicializada: {data.type} Lv{data.level} ({data.god})");

        if (healthBarHolder != null)
            healthBarHolder.UpdateHealth(resistance, GetMaxHealth());
    }

    // Aborta de forma fulminante llamadas diferidas y destruye el hilo de spawn activo de la estructura
    public virtual void ResetConstruction()
    {
        CancelInvoke();

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    // Evalúa los modificadores de vitalidad de la estructura reduciendo la resistencia y activando las secuencias de colapso
    public void ReceiveDamage(float damage)
    {
        if (IsDestroyed)
        {
            return;
        }

        resistance -= damage;

        if (healthBarHolder != null)
            healthBarHolder.UpdateHealth(resistance, GetMaxHealth());

        if (resistance <= 0)
        {
            IsDestroyed = true;
            OnDestruction();
            return;
        }
    }

    // Dispara las pistas sonoras de colapso ambiental e inicia la secuencia diferida de remoción física
    public virtual void OnDestruction()
    {
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);

        if (GameplaySoundPlayer.Instance != null)
            GameplaySoundPlayer.Instance.PlayWallDestroyed();
        destructionRoutine = StartCoroutine(DestructionRoutine(timeToBeDestroyed));
        Die();
    }


    // Libera la casilla lógica del mapa de cuadrícula (Tile) y retira el objeto de la simulación activa
    public virtual void Die()
    {
        if (parentTile != null)
        {
            parentTile.SetOccupied(false);
        }

        gameObject.SetActive(false);
    }

    // Inicializa el bucle subordinado de spawn condicionado al estatus de la oleada de invasión
    protected void StartWaveDependentSpawn(System.Action spawnAction)
    {
        if (waveRoutine != null)
            StopCoroutine(waveRoutine);

        waveRoutine = StartCoroutine(RunWhileWave(spawnAction));
    }

    // Enlaza la estructura a una coordenada lógica específica de la cuadrícula
    public void SetTile(Tile tile)
    {
        parentTile = tile;
    }

    // Devuelve la referencia de la baldosa que soporta físicamente a la estructura
    public Tile GetTile()
    {
        return parentTile;
    }

    // LÍNEA RARA / COMPLEJA: Inyección Procedural de Obstáculos y Esculpido Dinámico del Mapa de Navegación ('SetupNavObstacle').
    // Utiliza el método seguro 'TryGetComponent' para localizar el componente de obstrucción; si no existe en la malla física, 
    // lo inyecta dinámicamente en tiempo de ejecución. Configura su forma a caja ('Box') y activa la propiedad fundamental 
    // 'carving = true' junto con 'carveOnlyStationary = true'. Esto instruye al sistema de Inteligencia Artificial que "esculpa" 
    // un agujero permanente en la malla transitable del escenario en cuanto la construcción se detenga, forzando a los agentes 
    // enemigos a recalcular rutas óptimas de asalto alrededor de la edificación de forma reactiva.
    void SetupNavObstacle()
    {
        if (!TryGetComponent(out obstacle))
            obstacle = gameObject.AddComponent<NavMeshObstacle>();

        obstacle.shape = NavMeshObstacleShape.Box;
        obstacle.carving = true;
        obstacle.carveOnlyStationary = true;

        obstacle.size = new Vector3(1.5f, 1.5f, 1.5f); // ancho, alto, profundidad
        obstacle.center = Vector3.zero;
    }

    // Retorna el valor flotante representativo del umbral de integridad actual
    public virtual float GetResistance()
    {
        return resistance;
    }

    // Retorna el límite superior teórico de salud extraído del contenedor de datos estáticos
    public virtual float GetMaxHealth()
    {
        return data.resistance;
    }


    // Corrutinas
    // Ejecuta de forma cíclica e indefinida el delegado de acción respetando los intervalos de frecuencia mientras el componente esté activo
    protected IEnumerator SpawnLoop(System.Action spawnAction)
    {
        while (isActiveAndEnabled)
        {
            spawnAction?.Invoke();
            yield return new WaitForSeconds(timeToSpawnAditaments);
        }
    }

    // Retarda la llamada de desactivación física un tiempo determinado simulando efectos o animaciones de colapso post-mortem
    protected IEnumerator DestructionRoutine(float deathTime)
    {
        yield return new WaitForSeconds(deathTime);

        Die();
        destructionRoutine = null;
    }

    // LÍNEA RARA / COMPLEJA: Máquina de Estados Asíncrona Orientada al Estatus de Combate Global ('RunWhileWave').
    // Implementa un bucle infinito estructurado en base a dos compuertas lógicas de espera (`yield return null`). El primer bloque 
    // retiene el flujo suspendido mientras el Spawner central informe que no hay una batalla en curso. Al iniciar la horda, 
    // escapa de la restricción e inicia la corrutina `SpawnLoop` pasando el delegado de acción. El segundo bloque retiene el hilo 
    // manteniendo la producción activa durante el combate; en el instante en que la oleada concluye, el bucle progresa, 
    // detiene el sub-hilo de producción de aditamentos de forma limpia y reinicia el ciclo de monitoreo continuo.
    protected IEnumerator RunWhileWave(System.Action spawnAction)
    {
        while (true)
        {
            while (!WaveSpawner.Instance.IsWaveRunning())
                yield return null;

            spawnCoroutine = StartCoroutine(SpawnLoop(spawnAction));

            while (WaveSpawner.Instance.IsWaveRunning())
                yield return null;

            if (spawnCoroutine != null)
            {
                {
                    StopCoroutine(spawnCoroutine);
                    spawnCoroutine = null;
                }
            }
        }
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script constituye la Clase Base Abstracta Fundamental de Estructuras (BaseConstruction). Diseñada bajo los 
   principios de la programación orientada a objetos (POO), sirve como plantilla y contrato estructural absoluto para 
   cualquier tipo de edificación del juego (muros, torretas, cuarteles). Gestiona de forma unificada el sistema de 
   recibo de daño (`IDamageable`), la vinculación espacial con el sistema de casillas del terreno (`Tile`) y la alteración 
   dinámica del entorno de navegación por Inteligencia Artificial.

   Características clave:
   1. Interrupción Dinámica de Caminos de I.A. (NavMesh Carving): Al portar y configurar un `NavMeshObstacle` con la propiedad 
      de esculpido activa (`carving`), cada edificación altera la topología del mapa interactivo en tiempo real. Los enemigos 
      detectan físicamente la estructura como un muro impenetrable y desvían sus vectores de movimiento de manera dinámica.
   2. Control de Producción Sincronizado por Fases: Ofrece soporte nativo a través de corrutinas (`RunWhileWave`) para que las 
      estructuras derivadas produzcan elementos (unidades, munición, recursos) únicamente durante el fragor de las batallas, 
      congelando toda actividad y consumo de procesamiento de forma limpia durante las fases tranquilas de construcción.
   3. Registro Estático para Auditoría Rápida: Al mantener la lista colectiva global `AllConstructions`, el script permite 
      que otros directores lógicos del videojuego conozcan al instante la cantidad de defensas operativas en el mapa, 
      centralizando la salud de la base del jugador sin acoplamientos rígidos ni consultas redundantes.
   ========================================================================================================
*/