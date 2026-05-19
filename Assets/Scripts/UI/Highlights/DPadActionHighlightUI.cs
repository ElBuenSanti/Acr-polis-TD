using UnityEngine;

public class DPadActionHighlightUI : MonoBehaviour
{
    [Header("Animation")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private float activeScale = 1.15f;
    [SerializeField] private float animationSpeed = 12f;

    private bool isActive;
    private Vector3 originalScale;

    // Configura e inicializa los componentes de la interfaz de usuario en el arranque
    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        originalScale = rectTransform.localScale;

        // Desactiva el resaltado de forma inmediata al iniciar para que no aparezca encendido por error en el HUD
        SetActive(false);
    }

    // LÍNEA RARA / COMPLEJA: Suavizado por Interpolación (Mathf.Lerp / Vector3.Lerp) con 'Time.deltaTime'.
    // Modifica de manera continua la opacidad del canvas y el tamaño del icono de la cruceta (D-Pad).
    // A diferencia de los menús de pausa que analizamos antes (que usan 'unscaledDeltaTime'), este elemento del HUD 
    // utiliza 'Time.deltaTime'. Esto significa que si el juego se ralentiza debido a un efecto de juego o un poder, 
    // la animación visual de parpadeo del D-Pad se ralentizará en perfecta sincronía con el tiempo del mundo físico.
    private void Update()
    {
        float targetAlpha = isActive ? 1f : 0f;
        Vector3 targetScale = isActive ? originalScale * activeScale : originalScale;

        if (canvasGroup != null)
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * animationSpeed);

        if (rectTransform != null)
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, Time.deltaTime * animationSpeed);
    }

    // Permite que el script del mando principal ('GamepadInputController') encienda o apague este indicador visual
    public void SetActive(bool active)
    {
        isActive = active;
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Realzador Visual de Acciones en la Cruceta (DPadActionHighlightUI). Su función principal 
   es animar mediante código los elementos del HUD en pantalla que representan las flechas del mando (D-Pad), dándole 
   una respuesta visual dinámica al jugador en el instante en que ejecuta un atajo rápido.

   Características clave:
   1. Feedback de Ejecución de Comandos: Trabaja en conjunto con el gestor de inputs del control. Cuando el jugador pulsa, 
      por ejemplo, la flecha hacia abajo para construir un muro, este script hace que el sub-elemento correspondiente en la 
      pantalla crezca de tamaño (un 115%) y gane opacidad completa (alpha = 1f) de forma fluida.
   2. Interpolación Sincronizada con el Gameplay: Al estar supeditado al multiplicador de tiempo 'Time.deltaTime', el 
      comportamiento de desvanecimiento y escalado respeta el flujo de fotogramas e hilos de renderizado normales del mapa de juego, 
      manteniendo la consistencia visual de los indicadores del juego.
   3. Interfaz de Control Limpia: Centraliza la lógica cosmética en su propio componente, permitiendo que el controlador de mandos 
      se desentienda por completo de la animación interna; este solo debe llamar a 'SetActive(true)' o 'SetActive(false)' 
      para controlar la interfaz.
   ========================================================================================================
*/