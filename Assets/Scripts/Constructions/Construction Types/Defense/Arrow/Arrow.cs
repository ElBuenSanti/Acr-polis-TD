using UnityEngine;
using System.Collections;

public class Arrow : Projectile
{
    public Defense defense;
    public ConstructionData defenseData;


    // Extracting Arrow values
    // Inyecta las dependencias de la torre emisora, extrae las estadísticas de daño/velocidad y activa el feedback sonoro de disparo
    public void Initialize(ConstructionData data, Vector3 start, Vector3 target, Defense defense)
    {
        this.defense = defense;

        defenseData = data;

        Setup(start, target, defenseData.attackDamage, defenseData.proyectileVelocity); // ANTES data.attack... en lugar de defenseData
        SetTeam(Team.Ally);

        if (GameplaySoundPlayer.Instance != null)
            GameplaySoundPlayer.Instance.PlayArrowShot();

        Debug.Log($"Flecha inicializada: attackDamage: {attackDamage} velocity: ({proyectileVelocity})");
    }

    // LÍNEA RARA / COMPLEJA: Invocación del Constructor de Activación de la Clase Base Heredada ('OnEnable').
    // Utiliza la palabra clave 'base.OnEnable()' para forzar la ejecución de las subrutinas de inicialización del script 
    // padre 'Projectile'. Esto garantiza que al reactivarse la flecha desde el sistema de Pooling, se ejecuten de forma 
    // transparente los cálculos de trayectoria, limpieza de flags antiguos o temporizadores de autodestrucción por límite de rango.
    protected override void OnEnable()
    {
        base.OnEnable();
    }

    // Functions

    // LÍNEA RARA / COMPLEJA: Intercepción Discriminatoria por Solapamiento Volumétrico y Doble Validación de Interfaz ('OnTriggerEnter').
    // Callback nativo de físicas disparado cuando un colisionador ingresa en el volumen de detección de la flecha. Evalúa 'hasHit' de forma 
    // preventiva para evitar procesamientos dobles. Implementa una arquitectura robusta mediante 'TryGetComponent': primero confirma que 
    // la entidad impactada porta la lógica de comportamiento 'Enemy', y de forma anidada extrae su interfaz reactiva 'IDamageable'. 
    // Al verificar con éxito el contrato, transfiere el daño atómicamente, genera divisas y rompe el ciclo físico invocando el método 'Hit()'.
    void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        if (other.TryGetComponent<Enemy>(out var enemy))
        {
            if (other.TryGetComponent<IDamageable>(out var target))
            {
                hasHit = true;
                target.ReceiveDamage(attackDamage);
                SpawnWill();
                Debug.Log("Atacando a enemigo");
            }

            Hit();
        }
    }

    // LÍNEA RARA / COMPLEJA: Inyección y Transmisión de Recompensas de Divisas en Bucle Iterativo ('SpawnWill').
    // Itera sobre la colección de estructuras de recompensa financieras de tipo 'WillData' almacenadas en los datos estáticos de la torre 
    // ('defenseData.willObtaied'). En cada ciclo, accede directamente al gestor económico centralizado a través de su patrón Singleton 
    // ('WillManager.Instance') y ejecuta el método mutador 'AddMoney', acreditando de forma limpia los diferentes tipos de energía o recursos 
    // recolectados por haber acertado con éxito el proyectil.
    void SpawnWill()
    {
        foreach (var w in defenseData.willObtaied)
        {
            WillManager.Instance.AddMoney(w.type, w.amount);
        }
    }

    // Sobrescribe el método de impacto del proyectil base para detonar los efectos auditivos y preparar la destrucción visual de la entidad
    protected override void OnHit()
    {
        if (GameplaySoundPlayer.Instance != null)
            GameplaySoundPlayer.Instance.PlayArrowHit();

        Debug.Log("Golpeo flecha y se reproduce animación de destrucción de flecha");
        // AQUÍ ANIMACIÓN DE FLECHA COLISIONANDO
    }

}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Controlador Físico y de Impacto del Proyectil Flecha (Arrow). Hereda de la clase abstracta 
   o general `Projectile`, especializándose en el comportamiento balístico estándar para las estructuras de defensa 
   aliadas del tipo torre de arqueros (`Defense`). Su objetivo principal es resolver la traslación espacial hacia un 
   objetivo enemigo, aplicar el daño correspondiente de manera atómica al impactar y transferir las recompensas 
   económicas al inventario global del jugador.

   Características clave:
   1. Validación Arquitectónica de Impactos (TryGetComponent): En lugar de utilizar etiquetas de texto rígidas (`other.tag == "Enemy"`), 
      el script emplea la búsqueda segura de componentes por tipo. Esto previene excepciones en tiempo de ejecución, permitiendo 
      que cualquier entidad que posea el script de lógica enemiga e implemente el contrato de daños `IDamageable` sea afectada.
   2. Generación Económica Desacoplada: El proyectil no conoce de dónde surge el dinero ni cómo se almacena; simplemente lee 
      el contenedor de datos inyectado (`ConstructionData`) en su inicialización y, al cumplir la condición de colisión, 
      notifica al `WillManager` de forma directa el cobro del dividendo, logrando un flujo de código limpio y modular.
   3. Extensibilidad para Animaciones y Efectos Visuales: Cuenta con métodos de sobreescritura de ciclo de vida (`OnHit`), lo 
      que le permite aislar la lógica matemática de las físicas de Unity de la lógica cosmética, dejando un espacio seguro para 
      instanciar sistemas de partículas de astillas o animaciones de destrucción de la flecha sin alterar la simulación.
   ========================================================================================================
*/