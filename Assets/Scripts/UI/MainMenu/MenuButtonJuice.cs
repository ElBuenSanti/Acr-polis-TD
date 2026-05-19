using UnityEngine;
using UnityEngine.EventSystems;

// LÍNEA RARA / COMPLEJA: Implementación de interfaces del sistema de eventos de Unity para UI ('ISelectHandler', 'IDeselectHandler').
// Permite que este script reaccione de forma reactiva y automática cuando el EventSystem detecta que un control (como un Gamepad 
// o las flechas del teclado) selecciona o abandona este elemento de la interfaz de usuario, sin necesidad de usar un puntero físico.
public class MenuButtonJuice : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private RectTransform target;
    [SerializeField] private CanvasGroup glowGroup;
    [SerializeField] private float selectedScale = 1.08f;
    [SerializeField] private float speed = 12f;
    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private AudioClip confirmClip;

    private bool selected;
    private Vector3 normalScale;

    // Configura las referencias espaciales iniciales y oculta el brillo cosmético en el primer frame
    private void Awake()
    {
        if (target == null)
            target = GetComponent<RectTransform>();

        normalScale = target.localScale;

        if (glowGroup != null)
            glowGroup.alpha = 0f;
    }

    // LÍNEA RARA / COMPLEJA: Suavizado asíncrono con 'Time.unscaledDeltaTime'.
    // Realiza un cálculo matemático continuo mediante 'Vector3.Lerp' y 'Mathf.Lerp' para suavizar la transición visual del botón.
    // REGLA DE ORO: Al utilizar 'Time.unscaledDeltaTime', la animación ignorará por completo si el juego principal está pausado 
    // (es decir, si 'Time.timeScale' vale cero). El botón se estirará y su resplandor aparecerá de manera fluida y responsiva 
    // bajo cualquier circunstancia o estado lógico del motor de videojuegos.
    private void Update()
    {
        Vector3 targetScale = selected ? normalScale * selectedScale : normalScale;
        target.localScale = Vector3.Lerp(target.localScale, targetScale, Time.unscaledDeltaTime * speed);

        if (glowGroup != null)
            glowGroup.alpha = Mathf.Lerp(glowGroup.alpha, selected ? 1f : 0f, Time.unscaledDeltaTime * speed);
    }

    // Método disparado automáticamente por el EventSystem al tomar el foco de navegación
    public void OnSelect(BaseEventData eventData)
    {
        selected = true;
    }

    // Método disparado automáticamente por el EventSystem al perder el foco de navegación
    public void OnDeselect(BaseEventData eventData)
    {
        selected = false;
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Optimizador de Retroalimentación o Jugosidad del Menú (MenuButtonJuice). En el diseño de 
   videojuegos, el término "Juice" (jugosidad) se refiere a la adición de efectos visuales, sonoros y de movimiento 
   exagerados y satisfactorios ante las interacciones del usuario para que la interfaz se sienta viva, responsiva y orgánica.

   Características clave:
   1. Multi-Efecto Simultáneo: Va más allá del simple redimensionamiento de un botón. Al coordinar de forma paralela el 
      crecimiento tridimensional de la malla del botón ('target.localScale') junto con la aparición progresiva de un halo 
      de luz decorativo ('glowGroup.alpha'), genera un impacto estético de alta calidad visual.
   2. Animación Matemática Líquida (Lerp): Evita las transiciones rígidas e instantáneas. El algoritmo de interpolación 
      lineal asegura que los cambios visuales se suavicen en una curva de desaceleración orgánica, dando una sensación táctil 
      de amortiguación muy agradable para el jugador.
   3. Preparado para Audio del Menú: Declara variables serializadas para almacenar los clips de sonido de navegación 
      ('hoverClip') y de confirmación ('confirmClip'). Esto permite que, en fases de pulido técnico posteriores, se puedan 
      detonar estos audios mediante el sistema de sonido del juego al activar los métodos de selección.
   ========================================================================================================
*/