using UnityEngine;

// LÍNEA RARA / COMPLEJA (Para principiantes): El uso del modificador de acceso 'protected' y la estructura de clase base.
// Esta clase no está pensada para ser colgada directamente en un objeto, sino para servir de "plantilla madre" (Clase Base).
// Al marcar variables como 'protected' en lugar de 'private', permitimos que cualquier script hijo que herede de este 
// (por ejemplo: 'ShopPanelUI' o 'PauseMenuUI') pueda leer y modificar estas propiedades de forma directa, manteniéndolas ocultas para el resto de sistemas.
public class BasePanelUI : MonoBehaviour
{
    [Header("Base Panel")]
    [SerializeField] protected CanvasGroup canvasGroup;
    [SerializeField] protected float fadeSpeed = 10f;

    protected bool isOpen;

    // LÍNEA RARA / COMPLEJA: Métodos con la palabra clave 'virtual'.
    // Al declarar 'protected virtual void Awake()', le estamos dando permiso a las clases hijas de "sobrescribir" (Override) este método.
    // Esto significa que un panel hijo puede ejecutar su propia lógica personalizada al despertar, pero conservando la opción 
    // de invocar esta lógica base usando el comando 'base.Awake();'.
    protected virtual void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    // Calcula de forma continua el desvanecimiento del panel usando un algoritmo de progresión lineal uniforme
    protected virtual void Update()
    {
        float targetAlpha = isOpen ? 1f : 0f;

        if (canvasGroup != null)
        {
            // LÍNEA RARA / COMPLEJA: Interpolación lineal uniforme mediante 'Mathf.MoveTowards'.
            // A diferencia de 'Mathf.Lerp' (que reduce su velocidad de forma orgánica a medida que se acerca al objetivo generando una curva),
            // 'MoveTowards' avanza a una velocidad matemática constante y lineal. El cambio de opacidad se detiene exactamente 
            // al llegar al valor objetivo (0f o 1f), lo que resulta ideal para interfaces que requieren transiciones predecibles y simétricas.
            canvasGroup.alpha = Mathf.MoveTowards(
                canvasGroup.alpha,
                targetAlpha,
                Time.unscaledDeltaTime * fadeSpeed
            );
        }
    }

    // Activa el panel iniciando la transición visual de apertura y desbloqueando las interacciones
    public virtual void Open()
    {
        isOpen = true;
        SetInteractable(true);
    }

    // Desactiva el panel iniciando la transición visual de cierre y bloqueando de inmediato los clics
    public virtual void Close()
    {
        isOpen = false;
        SetInteractable(false);
    }

    // Fuerza la desaparición absoluta e instantánea del panel sin animaciones intermedias
    public virtual void HideInstant()
    {
        isOpen = false;

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        SetInteractable(false);
    }

    // Fuerza la aparición absoluta e instantánea del panel en pantalla completa sin animaciones intermedias
    protected void ShowInstant()
    {
        isOpen = true;

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;

        SetInteractable(true);
    }

    // Gestiona la interactividad física y lógica del bloque de la interfaz de usuario
    protected void SetInteractable(bool active)
    {
        if (canvasGroup == null)
            return;

        canvasGroup.interactable = active;
        canvasGroup.blocksRaycasts = active;
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como la Clase Base o Plantilla Maestra de Interfaces (BasePanelUI). Es un pilar arquitectónico 
   de Programación Orientada a Objetos (POO) diseñado para centralizar el comportamiento estándar de apertura, cierre, 
   interactividad y desvanecimiento de cualquier ventana de menú dentro del videojuego.

   Características clave:
   1. Reutilización Absoluta de Código: Implementa el principio DRY (Don't Repeat Yourself). Al definir los métodos 
      'Open', 'Close' y 'Fade' en este componente central, se evita tener que picar o duplicar exactamente el mismo 
      código matemático de manipulación de opacidades en los veinte menús diferentes que pueda tener el proyecto.
   2. Polimorfismo Limpio (Arquitectura Extensible): Gracias al uso de métodos 'virtual', la arquitectura es sumamente flexible. 
      Si el panel de la tienda ('ShopPanelUI') necesita reproducir un sonido metálico de monedas al abrirse, solo debe 
      heredar de esta clase, escribir 'public override void Open()' y añadir el sonido, llamando a 'base.Open()' para 
      que el desvanecimiento por código se siga ejecutando de forma transparente en segundo plano.
   3. Control de Rendimiento por CanvasGroup: Toda la gestión visual aprovecha las propiedades avanzadas del 'CanvasGroup'. 
      Esto asegura transiciones fluidas e independientes del estado de pausa del juego mediante 'Time.unscaledDeltaTime', 
      previniendo de forma automática clics accidentales en elementos ocultos de la interfaz sin necesidad de recurrir al 
      costoso apagado físico de GameObjects.
   ========================================================================================================
*/