using System.Collections;
using UnityEngine;
using TMPro;

public class StatusMessageUI : MonoBehaviour
{
    public static StatusMessageUI Instance;

    [Header("UI")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform panelTransform;
    [SerializeField] private TextMeshProUGUI statusText;

    [Header("Animation")]
    [SerializeField] private float visibleTime = 2.2f;
    [SerializeField] private float fadeTime = 0.18f;
    [SerializeField] private float slideDistance = 18f;

    private Coroutine messageRoutine;
    private Vector2 startPosition;

    // Asigna el patrón Singleton y cachea de forma nativa la posición inicial de anclaje de la interfaz
    private void Awake()
    {
        Instance = this;

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (panelTransform == null)
            panelTransform = GetComponent<RectTransform>();

        startPosition = panelTransform.anchoredPosition;
    }

    // Inicializa el sistema ocultando inmediatamente el panel para evitar parpadeos visuales al arrancar la escena
    private void Start()
    {
        HideInstant();
    }

    // LÍNEA RARA / COMPLEJA: Interrupción Segura y Reinicio de Corrutinas Solapadas ('ShowMessage').
    // Si un mensaje de error se solicita mientras otro ya está en pantalla, 'messageRoutine != null' evalúa positivo. 
    // Al invocar inmediatamente 'StopCoroutine(messageRoutine)', el script mata de forma fulminante la animación previa en ejecución. 
    // Esto evita un bug de sobreposición y parpadeo donde múltiples hilos asíncronos intentarían modificar el mismo 'alpha' 
    // y 'anchoredPosition' al mismo tiempo, garantizando que el nuevo mensaje se despliegue limpiamente desde el principio.
    public void ShowMessage(string message)
    {
        if (statusText != null)
            statusText.text = message;

        if (messageRoutine != null)
            StopCoroutine(messageRoutine);

        messageRoutine = StartCoroutine(ShowRoutine());
    }

    // LÍNEA RARA / COMPLEJA: Bucle Asíncrono Distribuidor de Ticks e Interpolación de Coordenadas de Interfaz ('ShowRoutine').
    // Esta máquina de estados asíncrona divide el flujo en tres fases mediante instrucciones 'yield return'. En la primera fase (Intro), 
    // ejecuta un bucle 'while' que calcula un factor de progresión normalizado ($t = \text{timer} / \text{fadeTime}$). Utiliza 'Vector2.Lerp' 
    // para desplazar el panel de forma descendente desde su 'hiddenPosition' (desfasada en el eje Y por 'slideDistance') hacia su 'startPosition' 
    // de diseño. Al retornar 'null', suspende temporalmente su ejecución devolviendo el control al hilo principal de Unity hasta el siguiente frame.
    private IEnumerator ShowRoutine()
    {
        //canvasGroup.gameObject.SetActive(true); 

        float timer = 0f;
        Vector2 hiddenPosition = startPosition + Vector2.up * slideDistance;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            float t = timer / fadeTime;

            canvasGroup.alpha = t;
            panelTransform.anchoredPosition = Vector2.Lerp(hiddenPosition, startPosition, t);

            yield return null;
        }

        canvasGroup.alpha = 1f;
        panelTransform.anchoredPosition = startPosition;

        // Fase de mantenimiento: Detiene el progreso del código de forma exacta en base al reloj de juego sin bloquear la UI
        yield return new WaitForSeconds(visibleTime);

        timer = 0f;

        // Fase de Outro: Desvanece la opacidad de manera inversa ($1f - t$) y eleva suavemente el panel hacia la posición oculta superior
        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            float t = timer / fadeTime;

            canvasGroup.alpha = 1f - t;
            panelTransform.anchoredPosition = Vector2.Lerp(startPosition, hiddenPosition, t);

            yield return null;
        }

        HideInstant();
    }

    // Restablece los valores alfa a cero absoluto y reubica el RectTransform en sus coordenadas de reposo originales
    private void HideInstant()
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 0f;

        if (panelTransform != null)
            panelTransform.anchoredPosition = startPosition;
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Sistema centralizado de Mensajes de Estado y Notificaciones del HUD (StatusMessageUI). 
   Es un componente global accesible a través de un patrón de diseño Singleton (`Instance`), cuya función es desplegar 
   alertas temporales no intrusivas en la pantalla del jugador (como advertencias de "No puedes abrir la tienda durante 
   la oleada" o "Recursos insuficientes"), animándolas sutilmente mediante transiciones de desvanecimiento y desplazamiento.

   Características clave:
   1. Patrón Singleton de Acceso Directo: Permite que cualquier subsistema del juego (tienda, gestor de oleadas, combate) 
      pueda mandar una notificación en texto crudo a la pantalla del jugador mediante una única línea estática global: 
      `StatusMessageUI.Instance.ShowMessage("Texto");`.
   2. Control de Animación Basado en Corrutinas: Evita depender del motor de físicas o del componente Animator de Unity. 
      Utiliza subprocesos ligeros coordinados de C# (`IEnumerator`), calculando de manera aritmética el desvanecimiento 
      del alfa y la traslación espacial para lograr la máxima optimización de rendimiento posible.
   3. Manejo de Coordenadas de Interfaz Modernas (RectTransform): En lugar de alterar la propiedad `.position` global 
      (que rompería la escala según la resolución del monitor), opera manipulando directamente `.anchoredPosition`. Esto 
      garantiza que la distancia de desplazamiento en píxeles (`slideDistance`) se aplique de forma relativa y perfecta 
      con respecto al pivote de anclaje de la UI configurado en el lienzo de Unity.
   ========================================================================================================
*/