using UnityEngine;

public class HealthBarHolder : MonoBehaviour
{
    [Header("Health Bar")]
    [SerializeField] private WorldHealthBarUI healthBarPrefab;
    [SerializeField] private Vector3 offset = new Vector3(0f, 2.2f, 0f);

    private WorldHealthBarUI healthBar;

    // LÍNEA RARA / COMPLEJA: Desactivación Sincrónica Protectora del Ciclo de Vida de la Interfaz UI ('OnDisable').
    // Evento nativo disparado cuando la entidad portadora (ya sea un soldado o un enemigo) es destruida o devuelta al Pool de objetos. 
    // Ejecuta una compuerta defensiva ('if (healthBar != null)'); si la barra flotante existe en el espacio de mundo, apaga de forma 
    // atómica su 'GameObject' ('SetActive(false)'). Esto evita que la barra de salud quede flotando de manera huérfana o "fantasma" 
    // en el escenario de juego mientras la entidad que la originó ya no se encuentra físicamente activa.
    private void OnDisable()
    {
        if (healthBar != null)
            healthBar.gameObject.SetActive(false);
    }

    // LÍNEA RARA / COMPLEJA: Actualización Dinámica con Inicialización Demorada Bajo Demanda ('UpdateHealth').
    // Actúa como el punto de entrada para sincronizar la vitalidad. Implementa un patrón de diseño 'Lazy Initialization' (Inicialización Perezosa): 
    // en lugar de malgastar recursos instanciando barras de vida para todas las unidades al arrancar el mapa, evalúa si 'healthBar == null'. 
    // Si la aserción es verdadera (porque es la primera vez que la entidad recibe daño), invoca la factoría 'CreateHealthBar()'. Superada la 
    // validación, transfiere de forma segura los flotantes 'current' y 'max' al mutador interno de la barra para refrescar el rellenado visual.
    public void UpdateHealth(float current, float max)
    {
        if (healthBar == null)
            CreateHealthBar();

        if (healthBar != null)
            healthBar.SetHealth(current, max);
    }

    // LÍNEA RARA / COMPLEJA: Factoría de Inyección Espacial de Elementos Flotantes de Escenario ('CreateHealthBar').
    // Realiza un filtrado preventivo confirmando la asignación del recurso de interfaz en el Inspector. Acto seguido, clona procedimentalmente 
    // en la escena el prefabricado UI mediante la instrucción 'Instantiate(healthBarPrefab)'. Por último, establece el acoplamiento lógico 
    // invocando el método de inicialización de la barra ('healthBar.Initialize'), pasándole el 'transform' de esta entidad como pivote de 
    // seguimiento de cámara y el vector 'offset' para sobreelevar la barra de vida $2.2\text{ unidades}$ en el eje Y por encima del modelo tridimensional.
    private void CreateHealthBar()
    {
        if (healthBarPrefab == null)
            return;

        healthBar = Instantiate(healthBarPrefab);
        healthBar.Initialize(transform, offset);
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el **Anclaje e Instanciador Dinámico de Barras de Vida en Espacio de Mundo** (`HealthBarHolder`). 
   Su responsabilidad principal en la arquitectura del videojuego es servir como un puente de comunicación intermedio 
   entre los scripts de lógica de combate (como `Enemy` o `HandToHandSoldier` que controlan la vida numérica) y la representación 
   gráfica y visual flotante de dicha salud (`WorldHealthBarUI`) sobre las cabezas de los personajes.

   Características arquitectónicas clave:
   1. Optimización por Inicialización Perezosa (Lazy Loading): El script no sobrecarga el juego instanciando barras de vida al 
      arrancar la escena. El objeto visual de la barra de salud solo se crea en memoria en el frame exacto en que la unidad recibe 
      su primer impacto de daño, lo que ahorra valiosos recursos de rendimiento en oleadas con alta densidad de combatientes.
   2. Desacoplamiento de Coordenadas de Interfaz: Al transferir el transformador (`transform`) y un vector de desfase (`offset`), 
      el titular delega por completo la matemática del posicionamiento a la barra de vida. Esta última se encargará de realizar de forma 
      autónoma los cálculos de rotación frente a la cámara (efecto Billboard) para que la UI siempre mire al jugador de manera correcta.
   3. Sincronización Limpia del Ciclo de Vida del Objeto: Gracias a la rutina de limpieza en `OnDisable`, este componente garantiza 
      la integridad visual del mapa. Asegura que el borrado, reciclaje o desactivación de una unidad física arrastre consigo de forma 
      inmediata a su interfaz asociada, manteniendo el espacio de renderizado limpio de basura espacial o errores de renderizado.
   ========================================================================================================
*/