using UnityEngine;
using System.Collections;

public class Wall : BaseConstruction
{

    private bool canRepair = false;
    private Coroutine repairCooldown;


    // Initialization of Wall
    // Inicializa la estructura de defensa invocando las subrutinas de registro espacial de la clase base 'BaseConstruction'
    protected override void Awake()
    {
        base.Awake();
    }

    // LÍNEA RARA / COMPLEJA: Desmantelamiento Preventivo de Hilos de Corrutina en el Ciclo de Desactivación Física ('OnDisable').
    // Intercepta la desactivación o destrucción del objeto en la escena y llama a 'base.OnDisable()' para limpiar referencias. Acto seguido, 
    // evalúa si el puntero 'repairCooldown' retiene un hilo de ejecución activo en segundo plano. De ser así, ejecuta de forma síncrona 
    // 'StopCoroutine(repairCooldown)' para abortar el temporizador del reloj. Esto previene fugas de memoria y errores críticos latentes 
    // ("NullReferenceException/MissingReferenceException") si el motor intenta despertar la rutina sobre un objeto oculto en el pool.
    protected override void OnDisable()
    {
        base.OnDisable();

        if (repairCooldown != null)
            StopCoroutine(repairCooldown);
    }

    // Configura los parámetros iniciales de la muralla y desbloquea el interruptor lógico para habilitar la primera reparación
    public override void Initialize(ConstructionData newData)
    {
        base.Initialize(newData);
        timeToBeDestroyed = 2f;

        canRepair = true;

    }


    // Functions

    // LÍNEA RARA / COMPLEJA: Validación de Costes Multidivisa y Propagación Colectiva de Restauración de Resistencia ('RepairGroup').
    // Valida la bandera 'canRepair' e intercepta el componente 'ConstructionController' para acceder al clúster de elementos vinculados de la muralla. 
    // Ejecuta un bucle 'foreach' para deducir de forma secuencial los costes económicos mapeados en 'data.bonusWillToPay' a través del singleton 
    // 'WillManager.Instance.SpendMoney'. Si alguna transacción falla por fondos insuficientes, aborta la operación instantáneamente. Si el pago es exitoso, 
    // recorre de forma polimórfica todos los elementos adyacentes del grupo ('members'), extrae sus componentes 'Wall' de forma segura y restaura 
    // la salud de todo el sector defensivo ejecutando de manera simultánea el método local 'HealToMax()'.
    public void RepairGroup()
    {
        if (!canRepair)
        {
            Debug.Log("Aún no se puede reparar");
            return;
        }
        var controller = GetComponent<ConstructionController>();

        if (controller == null || controller.group == null)
            return;

        foreach (var w in data.bonusWillToPay)
        {
            if (!WillManager.Instance.SpendMoney(w.type, w.amount))
            {
                Debug.Log("No te alcanza para reparar");
                return;
            }
        }
        foreach (var member in controller.group.members)
        {
            var wall = member.GetComponent<Wall>();

            if (wall != null)
                wall.HealToMax();
        }

        if (GameplaySoundPlayer.Instance != null)
            GameplaySoundPlayer.Instance.PlayWallRepair();
        Debug.Log("Muralla completamente reparada");
        canRepair = false;

        if (repairCooldown != null)
            StopCoroutine(repairCooldown);

        repairCooldown = StartCoroutine(RepairCooldown());
    }

    // Sobreescribe el flotante de integridad estructural actualizando el valor con el tope máximo dictado por el contenedor de datos estáticos
    void HealToMax()
    {
        colorOfParticles = new Color(0.4f, 0f, 0.6f);
        FXManager.Instance.PlayFX(FXManager.Instance.particulesEffects, transform, colorOfParticles);

        resistance = data.resistance;
    }




    // Cooroutines
    // Subproceso asíncrono que retiene el estado de bloqueo de la habilidad y restablece la bandera de reparación tras expirar el tiempo de espera
    IEnumerator RepairCooldown()
    {
        Debug.Log("Reparación en cooldown");

        yield return new WaitForSeconds(timeToSpawnAditaments);

        canRepair = true;
        repairCooldown = null;

        Debug.Log("Muralla lista para repararse");
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el **Controlador Lógico Interconectado y Gestor de Reparaciones de la Muralla** (`Wall`). 
   Hereda de la clase especializada `BaseConstruction` y su rol crítico dentro de las mecánicas de juego es el de implementar un sistema de 
   defensa distribuido por sectores. No procesa la curación de forma aislada, sino que interconecta de forma algorítmica las casillas 
   afines a un mismo grupo arquitectónico para curar murallas enteras en bloque (un comportamiento clásico de los juegos tipo RTS o *Castle Defense*).

   Características arquitectónicas clave:
   1. Curación Conectada por Grafos (Group-Linked Propagation): Al consumir el objeto `group` expuesto por el `ConstructionController`, 
      la clase implementa una arquitectura colectiva. Permite que la interacción sobre una sola sección de la muralla irradie el efecto de salud 
      al resto de las almenas colindantes registradas en esa misma línea de defensa, simplificando radicalmente la microgestión al usuario.
   2. Transaccionalidad de Costes Segura (Safe Cost Transaction): El primer bucle condicional actúa como una compuerta transaccional atómica. 
      Al comprobar y descontar recursos de forma secuencial, el script garantiza que el jugador pague el coste exacto antes de aplicar 
      cualquier incremento de resistencia, abortando el hilo de ejecución limpiamente si el jugador carece de los fondos requeridos.
   3. Blindaje de Corrutinas en Redundancia (Anti-Spam Cooldown Architecture): El flujo de recarga utiliza un puntero dedicado de tipo `Coroutine` 
      (`repairCooldown`). Al invocar de forma consecutiva `StopCoroutine` antes de un nuevo `StartCoroutine`, el script resetea de forma segura 
      cualquier temporizador residual, impidiendo que el jugador buclee o encadene múltiples rutinas concurrentes que desborden la memoria del motor.
   ========================================================================================================
*/