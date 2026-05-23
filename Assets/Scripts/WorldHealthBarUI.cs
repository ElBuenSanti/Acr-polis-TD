using UnityEngine;
using UnityEngine.UI;

public class WorldHealthBarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    private Transform target;
    private Vector3 offset;
    private Camera mainCamera;

    // Configura el objetivo de seguimiento espacial en el plano tridimensional e inyecta la referencia de la cámara principal
    public void Initialize(Transform newTarget, Vector3 newOffset)
    {
        target = newTarget;
        offset = newOffset;
        mainCamera = Camera.main;
    }

    // LÍNEA RARA / COMPLEJA: Proyección de Coordenadas de Seguimiento y Alineación Homogénea de Cartelera ('LateUpdate').
    // Se ejecuta al final del ciclo de procesamiento del frame para garantizar que los transformadores físicos ya se hayan desplazado. 
    // Si el objetivo 'target' es destruido o liberado, apaga el GameObject de inmediato para mitigar llamadas huérfanas. 
    // Traslada la posición de la barra sumando el desfase cartesiano de la UI ('target.position + offset'). Inmediatamente después, 
    // iguala el vector de dirección frontal de la barra con el de la lente de renderizado ('transform.forward = mainCamera.transform.forward'). 
    // Esta técnica de cartelera pura ("Billboard Effect") evita que el Canvas se distorsione o incline de forma ortogonal, asegurando que la 
    // barra de salud encare de forma plana y perpendicular al jugador sin importar las rotaciones tridimensionales de la cámara en el mundo.
    private void LateUpdate()
    {
        if (target == null)
        {
            gameObject.SetActive(false);
            return;
        }

        transform.position = target.position + offset;

        if (mainCamera != null)
            transform.forward = mainCamera.transform.forward;
    }

    // LÍNEA RARA / COMPLEJA: Normalización Aritmética de Ratio de Impacto y Conmutación Predictiva de Estado de UI ('SetHealth').
    // Intercepta el llenado gráfico del componente 'Image'. Primero evalúa la salud máxima mediante un operador ternario para interceptar la 
    // división por cero (evitando un desbordamiento matemático "NaN"). Divide los enteros y acota el resultado entre 0 y 1 empleando 'Mathf.Clamp01', 
    // asignándolo directamente al factor de llenado radial o lineal 'fillAmount'. Finalmente, aplica un criterio de optimización reactiva: 
    // conmuta el bit de activación del objeto con 'gameObject.SetActive(value > 0f && value < 1f)'. Esto oculta de forma automatizada la interfaz 
    // si la unidad tiene la vida completa (limpiando el ruido visual de la pantalla) o si está muerta, despertándola solo cuando sufre daño intermedio.
    public void SetHealth(float current, float max)
    {
        if (fillImage == null)
            return;

        float value = max <= 0f ? 0f : Mathf.Clamp01(current / max);
        fillImage.fillAmount = value;

        gameObject.SetActive(value > 0f && value < 1f);
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el **Controlador de Cartelera Espacial para Barras de Salud en el Mundo Flotante** (`WorldHealthBarUI`). 
   Su responsabilidad exclusiva dentro de la arquitectura de la interfaz de usuario es proyectar dinámicamente indicadores visuales 
   (Canvas en modo *World Space*) sobre entidades físicas del mapa (como enemigos, aliados o torretas), resolviendo el posicionamiento 
   y la orientación en función del punto de vista del usuario.

   Características arquitectónicas clave:
   1. Sincronización de Fase de Movimiento (LateUpdate Vector Tracking): Al procesar la traslación tridimensional dentro del método 
      nativo `LateUpdate`, el script elimina el desfase de parpadeo visual (*Jittering*). Garantiza que la interfaz se mueva *después* de que la física del motor (`FixedUpdate`) o las animaciones hayan consolidado la posición real de la entidad en el frame actual.
   2. Técnica de Cartelera Pura (Agnostic Billboard Alignment): Forzar `transform.forward` en lugar de utilizar funciones costosas como 
      `LookAt()` evita rotaciones no deseadas en los ejes locales Y o Z. La barra copia fielmente el vector de proyección de la lente, 
      haciendo que los elementos floten de forma bidimensional uniforme y paralela a la pantalla, ideal para perspectivas isométricas o en tercera persona.
   3. Optimización Activa del Búfer de Renderizado (Culling Optimization): El script actúa como su propio gestor de optimización de dibujado. 
      Al apagar automáticamente su propio `gameObject` cuando la salud está llena o llega a cero, remueve la barra del lote de renderizado 
      de la GPU (*Canvas Batching*), previniendo que decenas de barras de salud invisibles sigan consumiendo ciclos de cómputo por *Overdraw*.
   ========================================================================================================
*/