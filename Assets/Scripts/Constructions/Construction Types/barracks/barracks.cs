using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barracks : BaseConstruction
{
    public GameObject soldierPrefab;
    private Coroutine spawnSequence;
    private TargetFinder targetFinder;

    private float timeToSpwanInBetweenSoldiers = 1.5f;


    // Barrack Creation
    // Invocación polimórfica de la inicialización base y localización del resolvedor genérico de proximidad de amenazas
    protected override void Awake()
    {
        base.Awake();
        targetFinder = FindAnyObjectByType<TargetFinder>();
    }

    // Libera de forma segura las colecciones estáticas y detiene las subrutinas heredadas a través de la llamada base
    protected override void OnDisable()
    {
        base.OnDisable();
    }

    // Configura los parámetros de muerte y arranca el bucle reactivo subordinado al estado de la oleada global
    public override void Initialize(ConstructionData newData)
    {
        base.Initialize(newData);
        timeToBeDestroyed = 2f;

        StartWaveDependentSpawn(SpawnSoldier);

    }

    // Functions

    // Aborta de forma fulminante los sub-hilos de spawn activos limpiando los delegados mediante el contrato base
    public override void ResetConstruction()
    {
        base.ResetConstruction();
    }

    // Soldier Generation
    // Evalúa la concurrencia del sub-hilo, localiza el objetivo óptimo e intercepta la llamada si existen muros en estado íntegro
    void SpawnSoldier()
    {
        if (spawnSequence != null) return;


        Transform nearestEnemy = FindNearestEnemy();

        if (nearestEnemy == null)
            return;

        if (WallBetweenBarracksAndEnemy(nearestEnemy))
            return;

        spawnSequence = StartCoroutine(SpawnSoldiersWithDelay());
    }

    // LÍNEA RARA / COMPLEJA: Invocación del Sistema de Escaneo Espacial Genérico por Contrato de Tipo ('FindNearestEnemy').
    // Utiliza el resolvedor 'targetFinder.FindTarget' especificando el parámetro genérico de clase '<Enemy>'. Esta directiva 
    // realiza un muestreo de proximidad trigonométrica en base al origen del transformador de este cuartel, barriendo un 
    // radio esférico máximo de 100 unidades del motor, aislando y devolviendo el 'Transform' de la entidad hostil más cercana.
    Transform FindNearestEnemy()
    {
        return targetFinder.FindTarget<Enemy>(transform, 100f);
    }



    // LÍNEA RARA / COMPLEJA: Algoritmo de Triangulación y Análisis de Umbral de Integridad Estructural Perimetral ('WallBetweenBarracksAndEnemy').
    // Calcula la distancia escalar lineal hacia la amenaza detectada. Itera de forma lineal sobre la lista estática global 
    // 'BaseConstruction.AllConstructions' filtrando únicamente los nodos clasificados como 'ConstructionType.Wall'. Si una muralla se halla 
    // espacialmente más cerca del cuartel que el enemigo ('wallDistance < enemyDistance'), evalúa la distancia de dicho muro respecto a la amenaza. 
    // Al consolidar el objeto más próximo, ejecuta una ecuación de control de salud: si la resistencia actual de la muralla es menor o igual 
    // a dos tercios de su vitalidad máxima, el método interpreta que el perímetro está comprometido y retorna 'false' (permitiendo el spawn). 
    // De lo contrario, si el muro está intacto, bloquea el despliegue devolviendo 'true' para conservar tropas en la reserva interna.
    bool WallBetweenBarracksAndEnemy(Transform enemy)
    {
        float enemyDistance = Vector3.Distance(transform.position, enemy.position);

        BaseConstruction closestWall = null;

        float closestWallDistance = Mathf.Infinity;

        foreach (BaseConstruction construction in BaseConstruction.AllConstructions)
        {
            if (construction == null)
                continue;

            if (construction.Data.type != ConstructionType.Wall)
                continue;

            float wallDistance = Vector3.Distance(transform.position, construction.transform.position);


            if (wallDistance < enemyDistance)
            {
                float distanceToEnemy = Vector3.Distance(enemy.position, construction.transform.position);

                if (distanceToEnemy < closestWallDistance)
                {
                    closestWallDistance = distanceToEnemy;
                    closestWall = construction;
                }
            }
        }

        if (closestWall == null)
        {
            Debug.Log("No hay muralla");
            return false;
        }

        Debug.Log("Muralla más cercana: " + closestWall);
        Debug.Log($"Resistencia muralla: {closestWall.GetResistance()}. Resistencia Max: {closestWall.GetMaxHealth()}");

        if (closestWall.GetResistance() <= (closestWall.GetMaxHealth() / 1.5))
        {
            return false;
        }
        else
        {
            return true;
        }

    }



    // Cooroutines
    // LÍNEA RARA / COMPLEJA: Factoría Asíncrona Iterativa de Unidades de Combate con Desplazamiento Vectorial Coordenado ('SpawnSoldiersWithDelay').
    // Bucle indexado que limita rígidamente el despliegue a una ráfaga atómica de 3 soldados por ciclo de activación. En cada iteración, realiza 
    // una auditoría reactiva sobre 'WaveSpawner.Instance.IsWaveRunning()'; si la batalla concluye repentinamente, rompe el bucle de inmediato. 
    // Recupera la entidad desde el pool dinámico, calcula un desfase lateral usando el vector unitario derecho ('transform.right * 2f') para 
    // evitar solapamientos físicos en el punto de origen e inyecta los datos de configuración al componente 'HandToHandSoldier' antes de suspender 
    // el hilo durante los segundos definidos en el intervalo. Al finalizar la ráfaga, limpia el puntero de control.
    IEnumerator SpawnSoldiersWithDelay()
    {
        for (int i = 0; i < 3; i++)
        {
            if (!WaveSpawner.Instance.IsWaveRunning())
                break;

            GameObject newSoldier = pooling.CreateObject(soldierPrefab, transform);

            if (GameplaySoundPlayer.Instance != null)
                GameplaySoundPlayer.Instance.PlaySoldierSpawn();

            Vector3 spawnPos = transform.position + transform.right * 2f;
            newSoldier.transform.position = spawnPos;

            if (newSoldier.TryGetComponent<HandToHandSoldier>(out var soldier))
            {
                soldier.Initialize(data, this);
            }

            yield return new WaitForSeconds(timeToSpwanInBetweenSoldiers);
        }



        spawnSequence = null;
    }


}







/*
bool WallBetweenBarracksAndEnemy(Transform enemy)
{
    float enemyDistance = Vector3.Distance(transform.position, enemy.position);

    foreach (BaseConstruction construction in BaseConstruction.AllConstructions)
    {
        if (construction == null)
            continue;

        if (construction.Data.type != ConstructionType.Wall)
            continue;

        if (construction.GetResistance() < construction.GetMaxHealth() / 2)
            continue;

        float wallDistance = Vector3.Distance(transform.position, construction.transform.position);

        if (wallDistance < enemyDistance)
        {
            return true;
        }
    }

    return false;
}
*/