using System;
using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    public static WaveSpawner Instance;

    public event Action OnWaveEnded;
    public event Action OnWaveStarted;
    public event Action<float> OnWaveProgressChanged;
    public event Action<int, int> OnWaveIndexChanged;

    public WaveData[] waves;
    public Transform[] spawnPoints;

    private Pooling pooling;
    private GodType currentGod = GodType.Base;

    private int currentWaveIndex;
    private bool waveRunning;

    // Inicializa la instancia estática Singleton y localiza el objeto gestor de reciclaje de memoria (Pooling)
    private void Awake()
    {
        Instance = this;
        pooling = FindAnyObjectByType<Pooling>();
    }

    // LÍNEA RARA / COMPLEJA: Vinculación Multicast Asíncrona de Eventos Estáticos Cruzados ('OnEnable').
    // Conecta métodos oyentes a delegados declarados como estáticos en clases externas ('Temple' y 'FinalBoss'). 
    // Al acoplar directivas como 'Temple.OnGodSelected += SetGod', el Spawner reacciona a cambios estructurales globales 
    // sin mantener dependencias directas o referencias rígidas en memoria con las instancias físicas de los templos del mapa.
    private void OnEnable()
    {
        Temple.OnGodSelected += SetGod;
        FinalBoss.OnFinalBossDeath += HandleEndingConditions;
        Temple.OnTempleDestruction += HandleEndingConditions;
    }

    // LÍNEA RARA / COMPLEJA: Desuscripción de Delegados Estáticos para Prevención de Fugas de Memoria ('OnDisable').
    // Rompe explícitamente el enlace de los métodos oyentes usando el operador de sustracción de asignación ('-='). 
    // Si omitiéramos este paso al destruir o desactivar el objeto, las clases estáticas retendrían referencias fantasma 
    // hacia los métodos de este Spawner inactivo, impidiendo que el recolector de basura (Garbage Collector) libere los recursos 
    // y disparando excepciones críticas de puntero nulo en ejecuciones subsecuentes.
    private void OnDisable()
    {
        Temple.OnGodSelected -= SetGod;
        FinalBoss.OnFinalBossDeath -= HandleEndingConditions;
        Temple.OnTempleDestruction -= HandleEndingConditions;
    }

    // Valida prerrequisitos tácticos y arranca formalmente la secuencia lógica de spawn de la horda activa
    public void StartWave()
    {
        if (waveRunning)
            return;

        if (!HasAnyDefenseBuilt())
        {
            ShowStatus("Construye defensas antes de iniciar la ronda");
            GameplaySoundPlayer.Instance.PlayInvalidPlacement();
            return;
        }

        if (currentWaveIndex >= waves.Length)
        {
            ShowStatus("No hay más oleadas disponibles");
            return;
        }

        WaveData currentWave = waves[currentWaveIndex];

        if (currentWave.waveType == Wave.FinalBattle && currentGod == GodType.Base)
        {
            ShowStatus("Debes mejorar el templo antes de la oleada final");

            if (GameplaySoundPlayer.Instance != null)
                GameplaySoundPlayer.Instance.PlayInvalidPlacement();

            return;
        }

        waveRunning = true;

        PlayWaveAudio();

        OnWaveIndexChanged?.Invoke(GetCurrentWaveNumber(), GetTotalWaves());
        OnWaveStarted?.Invoke();

        ShowStatus("Oleada iniciada");

        if (currentWave.waveType == Wave.FinalBattle)
        {
            SpawnFinalBoss(currentWave);
            currentWaveIndex++;
        }
        else
        {
            StartCoroutine(RunWave(currentWave));
        }
    }

    // Devuelve el estado lógico de ejecución de la batalla actual
    public bool IsWaveRunning()
    {
        return waveRunning;
    }

    // Sincroniza la deidad protectora elegida por el jugador para modular las condiciones de fin de partida
    private void SetGod(GodType god)
    {
        currentGod = god;
        ShowStatus("Spawner recibió dios: " + god);
    }

    // Direcciona y activa las pistas musicales y efectos ambientales según el índice de progresión de la ronda
    private void PlayWaveAudio()
    {
        int waveNumber = GetCurrentWaveNumber();
        int totalWaves = GetTotalWaves();

        if (waveNumber == totalWaves)
        {
            if (GameplaySoundPlayer.Instance != null)
                GameplaySoundPlayer.Instance.PlayFinalWave();

            if (MusicManager.Instance != null)
                MusicManager.Instance.PlayFinalWaveMusic();

            return;
        }

        if (waveNumber == 5 || waveNumber == 10 || waveNumber == 15)
        {
            if (GameplaySoundPlayer.Instance != null)
                GameplaySoundPlayer.Instance.PlayMilestoneWave();

            if (MusicManager.Instance != null)
                MusicManager.Instance.PlayMilestoneMusic();

            return;
        }

        if (GameplaySoundPlayer.Instance != null)
            GameplaySoundPlayer.Instance.PlayWaveStart();

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayCombatMusic();
    }

    // Apaga los bucles musicales de combate y dispara las alertas de interrupción al colapsar las condiciones de victoria o derrota
    private void HandleEndingConditions()
    {
        waveRunning = false;

        if (MusicManager.Instance != null)
            MusicManager.Instance.StopAllMusicAndAmbience();

        OnWaveEnded?.Invoke();
    }

    // Extrae una entidad enemiga balanceada por ratio e instruye al sistema de Pooling su instanciación en un punto aleatorio
    private void SpawnEnemy(WaveData wave)
    {
        EnemyWaveEntry enemyType = GetRandomEnemyByPercentage(wave);
        Transform spawnPoint = GetRandomSpawnPoint();

        GameObject obj = pooling.CreateObject(enemyType.enemy.enemyPrefab, spawnPoint);
        obj.transform.position = spawnPoint.position;
        obj.transform.rotation = spawnPoint.rotation;

        Enemy enemy = obj.GetComponent<Enemy>();
        enemy.Initialize(enemyType.enemy);
    }

    // Extrae el jefe final configurado específicamente para la deidad patrona activa y lo posiciona en el entorno de juego
    private void SpawnFinalBoss(WaveData wave)
    {
        ShowStatus("Boss Final");

        EnemyData bossData = wave.GetBossForGod(currentGod);

        if (bossData == null)
        {
            ShowStatus("No hay boss para el dios: " + currentGod);
            return;
        }

        Transform spawnPoint = GetRandomSpawnPoint();

        GameObject obj = pooling.CreateObject(bossData.enemyPrefab, spawnPoint);
        obj.transform.position = spawnPoint.position;
        obj.transform.rotation = spawnPoint.rotation;

        Enemy enemy = obj.GetComponent<Enemy>();
        enemy.Initialize(bossData);

        ShowStatus("Boss spawneado: " + bossData.name + " para dios: " + currentGod);
    }

    // Selecciona un nodo de transformación aleatorio a partir del arreglo de vectores de entrada configurados en el inspector
    private Transform GetRandomSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
            return transform;

        return spawnPoints[UnityEngine.Random.Range(0, spawnPoints.Length)];
    }

    // LÍNEA RARA / COMPLEJA: Bucle de Control Temporal e Inyección de Progreso Normalizado por Cuanto Lineal ('RunWave').
    // Controla asíncronamente el ciclo de vida de la ronda mediante un bucle 'while' condicionado por la duración teórica de la ola 
    // y la bandera 'waveRunning'. Invoca 'SpawnEnemy' y suspende el hilo con 'yield return new WaitForSeconds' usando la frecuencia de spawn. 
    // En cada iteración incrementa el acumulador físico de tiempo y calcula la razón de progreso fraccionario ($t = \text{timer} / \text{waveDuration}$), 
    // transmitiéndola inmediatamente a través del operador de coalescencia nula '?.Invoke()' a todos los elementos del HUD suscritos.
    private IEnumerator RunWave(WaveData wave)
    {
        float timer = 0f;

        while (timer < wave.waveDuration && waveRunning)
        {
            SpawnEnemy(wave);

            yield return new WaitForSeconds(wave.spawnFrequency);

            timer += wave.spawnFrequency;
            OnWaveProgressChanged?.Invoke(timer / wave.waveDuration);
        }

        currentWaveIndex++;
        waveRunning = false;

        OnWaveProgressChanged?.Invoke(1f);

        ShowStatus("Fin de la Oleada");

        if (MusicManager.Instance != null)
            MusicManager.Instance.PlayConstructionMusic();

        OnWaveEnded?.Invoke();
    }

    // LÍNEA RARA / COMPLEJA: Algoritmo de Selección Estocástica por Acumulación de Pesos Porcentuales ('GetRandomEnemyByPercentage').
    // Genera un valor flotante pseudoaleatorio continuo uniformemente distribuido entre [0f, 100f]. Itera sobre la lista de entradas 
    // acumulando progresivamente sus pesos relativos en 'currentPercentage' (simulando una ruleta estadística segmentada). 
    // En el momento en que el número aleatorio cae dentro del intervalo acumulado actual ($r \le P_i$), el método rompe la iteración 
    // y retorna dicha estructura, asegurando un balance probabilístico estricto y exacto según el diseño de datos del nivel.
    private EnemyWaveEntry GetRandomEnemyByPercentage(WaveData wave)
    {
        float randomValue = UnityEngine.Random.Range(0f, 100f);
        float currentPercentage = 0f;

        foreach (EnemyWaveEntry enemyEntry in wave.enemies)
        {
            currentPercentage += enemyEntry.percentageInWave;

            if (randomValue <= currentPercentage)
                return enemyEntry;
        }

        return wave.enemies[0];
    }

    // Redirecciona los mensajes lógicos de estado al lienzo del HUD y a la terminal estándar del motor de juego
    private void ShowStatus(string message)
    {
        if (StatusMessageUI.Instance != null)
            StatusMessageUI.Instance.ShowMessage(message);

        Debug.Log(message);
    }

    // Retorna el índice ordinal legible basado en la ronda activa del gestor
    public int GetCurrentWaveNumber()
    {
        return currentWaveIndex + 1;
    }

    // Retorna la dimensión o volumen total del contenedor de arreglos de datos de oleadas
    public int GetTotalWaves()
    {
        return waves.Length;
    }

    // Evalúa de forma lineal el listado estático global de estructuras construidas en busca de edificaciones defensivas válidas
    private bool HasAnyDefenseBuilt()
    {
        foreach (BaseConstruction construction in BaseConstruction.AllConstructions)
        {
            if (construction == null || construction.Data == null)
                continue;

            if (construction.Data.type == ConstructionType.Defense ||
                construction.Data.type == ConstructionType.Barracks ||
                construction.Data.type == ConstructionType.Wall)
                return true;
        }

        return false;
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Controlador y Distribuidor Central de Invasiones de Enemigos (WaveSpawner). 
   Es el núcleo lógico encargado de secuenciar las fases del bucle principal de un videojuego Tower Defense o de estrategia. 
   Su responsabilidad abarca la gestión temporal del spawn de monstruos, el cálculo estocástico (probabilístico) para la 
   selección de tipos de enemigos en base a porcentajes de aparición, el control de las transiciones musicales del nivel 
   y la invocación reactiva de eventos que comunican al HUD y a otros sistemas el estado del combate.

   Características clave:
   1. Mecánica de Eventos Basada en Patrón Observador: Expone delegados genéricos de C# (`Action`) para notificar de forma 
      limpia hitos temporales críticos (`OnWaveStarted`, `OnWaveEnded`, `OnWaveProgressChanged`). Esto elimina el acoplamiento 
      rígido, permitiendo que la interfaz gráfica se redibuje únicamente cuando el Spawner emite una señal.
   2. Selección por Ruleta de Probabilidades: Incorpora un algoritmo clásico de distribución acumulada para decidir qué 
      enemigo instanciar. Al sumar linealmente las propiedades de peso de cada entrada, el sistema se vuelve altamente modular, 
      permitiendo balances complejos en los datos de oleadas (`WaveData`) directamente desde el inspector.
   3. Control Contextual de Jefes por Mitología: Detecta el estado evolutivo del núcleo de juego (`GodType` del Templo). 
      Si la oleada se etiqueta como batalla final (`Wave.FinalBattle`), bloquea el inicio si el jugador no ha escogido un 
      patrón divino y, en caso correcto, extrae mecánicamente el Nemesis o Jefe final diseñado específicamente para contrarrestar 
      los poderes elegidos por el usuario.
   ========================================================================================================
*/