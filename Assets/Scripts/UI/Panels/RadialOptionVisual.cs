using UnityEngine;
using UnityEngine.UI;

public class RadialOptionVisual : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform targetTransform;

    [Header("Settings")]
    [SerializeField] private float activeScale = 1.08f;
    [SerializeField] private float inactiveScale = 1f;
    [SerializeField] private float activeAlpha = 1f;
    [SerializeField] private float inactiveAlpha = 0f;
    [SerializeField] private float animationSpeed = 12f;

    private bool isActive;

    // Inicializa por código las referencias a los componentes si el desarrollador no los vinculó en el inspector
    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();

        if (targetTransform == null)
            targetTransform = GetComponent<RectTransform>();
    }

    // Gestiona la transición animada del tamaño y la opacidad del elemento visual según su estado de selección
    private void Update()
    {
        float targetAlpha = isActive ? activeAlpha : inactiveAlpha;
        float targetScale = isActive ? activeScale : inactiveScale;

        // LÍNEA RARA / COMPLEJA: Interpolación Lineal Asíncrona de Opacidad ('Mathf.Lerp').
        // Modifica de forma matemática y progresiva el valor alfa de la UI basándose en 'Time.deltaTime'. 
        // Al multiplicar el delta por 'animationSpeed', la transición se suaviza simulando una amortiguación física (deceleración),
        // lo que suaviza la aparición o desaparición del resalte visual del botón sin requerir una máquina de animación pesada (Animator).
        if (canvasGroup != null)
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, targetAlpha, Time.deltaTime * animationSpeed);

        // LÍNEA RARA / COMPLEJA: Escalamiento Vectorial Tridimensional Suavizado ('Vector3.Lerp').
        // Multiplica el escalar de destino ('targetScale') por 'Vector3.one' para generar un vector homogéneo uniforme (X, Y, Z idénticos).
        // Interpola el tamaño actual del 'RectTransform' hacia este nuevo volumen. Al ejecutarse frame a frame, el botón parece "inflarse" 
        // de forma orgánica al posicionar el cursor sobre él y recuperar su tamaño natural de reposo al deseleccionarse.
        if (targetTransform != null)
            targetTransform.localScale = Vector3.Lerp(
                targetTransform.localScale,
                Vector3.one * targetScale,
                Time.deltaTime * animationSpeed
            );
    }

    // Registra externamente el estado de activación lumínica y de tamaño para que el bucle 'Update' procese el cambio gráfico
    public void SetActiveVisual(bool active)
    {
        isActive = active;
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Controlador de Feedback Visual para las Opciones del Menú Radial (RadialOptionVisual). 
   Es el componente esclavo encargado de dar vida gráfica a cada gajo o botón individual del menú radial. Cuando 
   el script maestro ('RadialMenuUI') detecta que el joystick apunta en su dirección, le ordena a esta clase activarse, 
   generando una animación fluida de resaltado mediante código.

   Características clave:
   1. Animación por Software Altamente Eficiente: Evita el uso del sistema tradicional 'Animator' de Unity o clips 
      de animación basados en keyframes. Al calcular el Lerp directamente en el método 'Update', reduce el uso de hilos 
      y evita la sobrecarga de CPU que generan los árboles de animación en elementos de la UI.
   2. Manipulación Limpia a través de CanvasGroup: Controla la visibilidad alterando el 'alpha' de un 'CanvasGroup'. 
      Esto es óptimo para interfaces de usuario, ya que permite ocultar por completo el resalte (haciéndolo invisible e 
      incapaz de bloquear clics extraños de ratón) sin necesidad de recurrir a la desactivación del GameObject completo.
   3. Consistencia de Escala Basada en DeltaTime: Al incluir 'Time.deltaTime' dentro del factor de interpolación lineal, 
      el script garantiza que la velocidad de "inflado" y desvanecimiento de la UI sea exactamente idéntica, sin importar 
      si el juego corre en un teléfono inteligente a 30 FPS o en un monitor de computadora de alta gama a 144 FPS o superior.
   ========================================================================================================
*/