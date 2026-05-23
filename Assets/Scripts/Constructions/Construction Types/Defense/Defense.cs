using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Defense : BaseConstruction
{
    public GameObject arrowPrefab;
    private TargetFinder targetFinder;
    private Transform currentTarget;

    private float range;


    // Defense Initialization
    // Invocación polimórfica del constructor base para heredar componentes y localización del buscador de amenazas perimetrales
    protected override void Awake()
    {
        base.Awake();
        targetFinder = FindAnyObjectByType<TargetFinder>();
    }

    // Configura las variables temporales de colapso físico y activa el bucle modular de spawn condicionado al estado de la oleada
    public override void Initialize(ConstructionData newData)
    {
        base.Initialize(newData);
        timeToBeDestroyed = 2f;

        StartWaveDependentSpawn(SpawnArrows);
    }


    // Functions
    // LÍNEA RARA / COMPLEJA: Sobrescritura Sincrónica Extensible de Perfiles de Atributos Tácticos ('ApplyStats').
    // Utiliza la palabra clave 'base.ApplyStats()' para delegar la sincronización de variables críticas universales (como la 
    // resistencia e interfaz de salud HUD) en la clase abstracta padre. Inmediatamente después, extiende el método asignando de forma 
    // específica la propiedad flotante 'range' extraída directamente del ScriptableObject 'data', acoplando los parámetros de balance 
    // numérico con la lógica física de este subtipo de torre.
    protected override void ApplyStats()
    {
        base.ApplyStats();
        range = data.range;
    }

    // Arrow Generation
    // LÍNEA RARA / COMPLEJA: Factoría de Proyectiles Balísticos por Pooling con Desfase de Vector Altura ('SpawnArrows').
    // Ejecuta un escaneo radial invocando 'targetFinder.FindTarget<Enemy>' delimitado por el rango dinámico de la estructura. Si localiza 
    // una amenaza, calcula un vector de origen sobreelevado ('Vector3.up * 1.5f') para simular la corona o almena de la torre. Solicita 
    // una entidad al gestor de reciclaje 'pooling.CreateObject', actualiza su posición espacial e intercepta su interfaz interna 
    // mediante 'TryGetComponent<Arrow>'. Al convalidar el componente, inyecta atómicamente los metadatos balísticos para iniciar el trayecto.
    void SpawnArrows()
    {
        currentTarget = targetFinder.FindTarget<Enemy>(transform, range);

        if (currentTarget == null)
        {
            return;
        }

        Vector3 spawnPos = transform.position + Vector3.up * 1.5f;

        GameObject newArrow = pooling.CreateObject(arrowPrefab, transform);

        newArrow.transform.position = spawnPos;

        if (newArrow.TryGetComponent<Arrow>(out var arrow))
        {
            arrow.Initialize(data, spawnPos, currentTarget.position, this);
        }
    }

}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el **Controlador Lógico de Estructuras Defensivas a Distancia** (`Defense`). Hereda directamente 
   de la clase abstracta `BaseConstruction`, lo que le otorga de forma nativa la gestión de barra de vida (`IDamageable`), 
   su anclaje en el mapa de casillas (`Tile`), y la capacidad de obstruir dinámicamente las mallas de navegación enemigas. 
   Su rol principal dentro de la simulación es actuar como una torre de arqueros automatizada que detecta amenazas y 
   dispara proyectiles con frecuencias controladas.

   Características arquitectónicas clave:
   1. Escaneo Genérico y Aislado de Amenazas: En lugar de procesar costosos cálculos físicos de detección (`Physics.OverlapSphere`) 
      en cada frame, la torre delega el escaneo de proximidad al componente especializado `TargetFinder`, solicitando únicamente 
      entidades que cumplan con el contrato de tipo `<Enemy>`.
   2. Integración con el Sistema de Pooling: Para evitar la degradación de rendimiento provocada por la instanciación y destrucción 
      constante de objetos de Unity en memoria (`Instantiate` / `Destroy`), la torre solicita y devuelve las flechas a través de la 
      clase `Pooling`, optimizando el consumo de CPU durante picos de alta densidad de combate.
   3. Inicialización Inyectada y Segura: Al acoplarse directamente con el script `Arrow` mediante `TryGetComponent`, la torre 
      transfiere los datos de configuración física (daño, velocidad del proyectil) en el mismo frame de su activación, garantizando 
      que el proyectil viaje con los valores correctos de nivel y deidad correspondientes a la estructura emisora.
   ========================================================================================================
*/