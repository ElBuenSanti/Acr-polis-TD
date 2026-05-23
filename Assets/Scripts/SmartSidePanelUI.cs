using UnityEngine;

public class SmartSidePanelUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Opacity")]
    [SerializeField] private float compactAlpha = 0.25f;
    [SerializeField] private float expandedAlpha = 0.9f;

    [Header("Animation")]
    [SerializeField] private float animationSpeed = 10f;

    private bool expanded;

    // Cachea de forma preventiva el contenedor de desvanecimiento global de UI en caso de omisión en el Inspector
    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    // Fuerza la inicialización asertiva del panel fijando el estado de contracción en el primer frame
    private void Start()
    {
        SetCompact();
    }

    // LÍNEA RARA / COMPLEJA: Transición Lineal Atómica de Transparencia Independiente del Factor de Pausa ('Update').
    // Evalúa preventivamente el estado del puntero 'canvasGroup' para evitar fugas de ejecución. Resuelve el valor objetivo de opacidad 
    // mediante un operador ternario basado en la bandera 'expanded'. Acto seguido, calcula el desplazamiento alfa del frame utilizando 
    // la función matemática 'Mathf.MoveTowards'. A diferencia de una interpolación asintótica ('Lerp'), esta instrucción calcula un cambio 
    // lineal uniforme absoluto que no se ralentiza al acercarse al destino. Multiplica la velocidad por 'Time.unscaledDeltaTime' para 
    // garantizar que el desvanecimiento transcurra a ritmo constante, incluso si el juego sufre caídas de frames o está completamente pausado.
    private void Update()
    {
        if (canvasGroup == null)
            return;

        float targetAlpha = expanded ? expandedAlpha : compactAlpha;

        canvasGroup.alpha = Mathf.MoveTowards(
             canvasGroup.alpha,
             targetAlpha,
             Time.unscaledDeltaTime * animationSpeed
        );
    }

    // Conmuta el estado lógico para inducir mecánicamente el desvanecimiento hacia el umbral mínimo de opacidad compacta
    public void SetCompact()
    {
        expanded = false;
    }

    // Conmuta el estado lógico para inducir mecánicamente el desvanecimiento hacia el umbral máximo de opacidad expandida
    public void SetExpanded()
    {
        expanded = true;
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el **Controlador Reactivo de Opacidad para Paneles Inteligentes de Interfaz** (`SmartSidePanelUI`). 
   Su rol exclusivo dentro de la capa visual de la arquitectura es gestionar la transición dinámica de visibilidad de un panel lateral de UI. 
   El código antiguo (comentado) alteraba simultáneamente la escala bidimensional física (`sizeDelta`) y la transparencia. La versión activa 
   simplifica el comportamiento, delegando toda la carga estética a la manipulación atómica de la opacidad del `CanvasGroup` para optimizar 
   el rendimiento de renderizado en pantallas con interfaces densas.

   Características arquitectónicas clave:
   1. Transición de Opacidad Uniforme (Linear MoveTowards vs Exponential Lerp): El cambio principal con respecto al código previo 
      es el uso de `Mathf.MoveTowards`. Mientras que un `Lerp` produce un frenado suavizado que puede tardar varios frames en alcanzar 
      el valor exacto, `MoveTowards` avanza en línea recta a una velocidad matemática constante, garantizando un cierre nítido e inmediato 
      en cuanto el alfa toca los límites de 0.25f o 0.9f.
   2. Inmunidad a Escalas de Tiempo de Juego (Unscaled Delta Time Alignment): Al calcular los deltas de animación multiplicando por 
      `Time.unscaledDeltaTime`, el panel se desacopla del reloj interno del motor físico (`Time.timeScale`). Si el jugador abre el panel 
      estando el juego en pausa (donde el tiempo real del juego se congela para congelar enemigos y proyectiles), la UI se desvanece de 
      forma suave y fluida a la misma velocidad.
   3. Interfaz de Mutación del Estado Limpia (State-Driven UI Mutation): Expone métodos públicos descriptivos y autoexplicativos 
      (`SetCompact` y `SetExpanded`). Esto permite que otros managers de la escena, sistemas de eventos del ratón (como punteros flotantes 
      Hover de UI) o botones del Canvas alteren el comportamiento visual de la pantalla sin necesidad de conocer la matemática interna 
      de cálculo de desvanecimiento que procesa el Update.
   ========================================================================================================
*/