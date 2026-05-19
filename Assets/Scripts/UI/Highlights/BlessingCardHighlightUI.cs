using UnityEngine;

public class BlessingCardHighlightUI : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private float selectedScale = 1.08f;
    [SerializeField] private float speed = 12f;

    private bool selected;
    private Vector3 normalScale;

    // Inicializa las referencias de la interfaz de usuario y almacena la escala base del componente
    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        // LÍNEA RARA / COMPLEJA: Accede de forma jerárquica al componente RectTransform del contenedor padre ('transform.parent').
        // Esto se realiza porque el efecto visual de agrandar (resaltar) la tarjeta divina usualmente se aplica sobre 
        // todo el contenedor base de la tarjeta, permitiendo que este script actúe de forma aislada en un objeto hijo.
        if (rectTransform == null)
            rectTransform = transform.parent.GetComponent<RectTransform>();

        normalScale = rectTransform.localScale;
    }

    // Ejecuta de forma continua la suavización de los efectos visuales y de escala de la tarjeta
    private void Update()
    {
        // Operador Ternario: Define de manera compacta los valores objetivos de opacidad y tamaño según el estado de selección
        float targetAlpha = selected ? 1f : 0f;
        Vector3 targetScale = selected ? normalScale * selectedScale : normalScale;

        // LÍNEA RARA / COMPLEJA: Interpolación Lineal (Mathf.Lerp / Vector3.Lerp) con 'Time.unscaledDeltaTime'.
        // 'Lerp' calcula una transición matemática fluida y orgánica entre el valor actual y el valor objetivo.
        // REGLA DE ORO: Utiliza 'Time.unscaledDeltaTime' en lugar del 'deltaTime' tradicional. Como vimos en scripts anteriores, 
        // cuando el juego se pausa (Time.timeScale = 0), todas las animaciones basadas en deltaTime se congelan. 
        // El uso de 'unscaledDeltaTime' garantiza que los menús e interfaces sigan respondiendo de forma fluida y animada 
        // incluso si el juego principal se encuentra completamente pausado de fondo.
        if (canvasGroup != null)
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.unscaledDeltaTime * speed);

        if (rectTransform != null)
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, Time.unscaledDeltaTime * speed);
    }

    // Permite que componentes externos (como el selector del control) modifiquen el estado de enfoque de la tarjeta
    public void SetSelected(bool value)
    {
        selected = value;
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Realzador Dinámico de Tarjetas de Interfaz (BlessingCardHighlightUI). Su función 
   principal es gestionar la respuesta cosmética y estética de una tarjeta de selección (como los menús de bendiciones 
   divinas) cuando el usuario se posiciona sobre ella con el control de mandos.

   Características clave:
   1. Animaciones Orgánicas por Código: En lugar de utilizar el pesado sistema de animaciones tradicionales de Unity 
      ('Animator'), que consume memoria de procesamiento en hilos del procesador, calcula de forma matemática mediante 
      interpolaciones lineales (Lerp) un efecto de escalado (crecimiento) y desvanecimiento (opacidad) ultra-optimizado.
   2. Compatibilidad con Menús de Pausa e Interrupción: Gracias al uso de la propiedad temporal independiente 
      'unscaledDeltaTime', el script ignora las congelaciones temporales físicas del motor de videojuegos, asegurando 
      un comportamiento y transiciones responsivas en menús de elección que pausan el mundo del juego.
   3. Estructuración Modular: Funciona de forma aislada a través del método público 'SetSelected', permitiendo que 
      el controlador general de inputs active o desactive los efectos visuales de la tarjeta de forma remota sin 
      necesidad de que la tarjeta conozca qué botón o control la está apuntando.
   ========================================================================================================
*/