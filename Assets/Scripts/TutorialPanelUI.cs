using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TutorialPanelUI : MonoBehaviour
{
    public static TutorialPanelUI Instance;

    [Header("UI")]
    [SerializeField] private CanvasGroup tutorialGroup;
    [SerializeField] private CanvasGroup titleGroup;
    [SerializeField] private Button continueButton;
    [SerializeField] private CanvasGroup continueButtonGroup;
    [SerializeField] private float continueAppearDelay = 1.2f;

    [Header("Scrolling Text")]
    [SerializeField] private RectTransform scrollingText;
    [SerializeField] private float startY = -420f;
    [SerializeField] private float endY = 520f;
    [SerializeField] private float scrollSpeed = 35f;
    [SerializeField] private float titleFadeStartY = -200f;

    [Header("Animation")]
    [SerializeField] private float fadeSpeed = 8f;

    [Header("Sound")]
    [SerializeField] private UISoundPlayer uiSoundPlayer;

    private float textFinishedTime;
    private bool isOpen;
    private bool tutorialCompleted;
    private bool textFinished;
    private bool continueReady;
    private bool continueSelected;

    // Inicializa la variable de instancia estática global Singleton y autocompleta referencias faltantes en los componentes de interfaz
    private void Awake()
    {
        Instance = this;

        if (tutorialGroup == null)
            tutorialGroup = GetComponent<CanvasGroup>();

        if (continueButtonGroup == null && continueButton != null)
            continueButtonGroup = continueButton.GetComponent<CanvasGroup>();

        if (uiSoundPlayer == null)
            uiSoundPlayer = FindAnyObjectByType<UISoundPlayer>();
    }

    private void Start()
    {
    }

    // LÍNEA RARA / COMPLEJA: Interpolación de Opacidad Inmune a la Congelación del Tiempo de Juego ('Update').
    // Utiliza el método matemático 'Mathf.MoveTowards' para transicionar linealmente el valor alfa de los paneles. 
    // Al multiplicar el ratio de desvanecimiento ('fadeSpeed') por la variable global 'Time.unscaledDeltaTime', la animación 
    // mantiene un movimiento fluido y constante en la pantalla, ignorando por completo que el flujo físico del juego 
    // se encuentre pausado mediante una escala de tiempo de cero absoluto ('Time.timeScale = 0f').
    private void Update()
    {
        if (tutorialGroup == null)
            return;

        tutorialGroup.alpha = Mathf.MoveTowards(
            tutorialGroup.alpha,
            isOpen ? 1f : 0f,
            Time.unscaledDeltaTime * fadeSpeed
        );

        if (isOpen && scrollingText != null)
        {
            Vector2 pos = scrollingText.anchoredPosition;

            if (!textFinished)
            {
                pos.y += scrollSpeed * Time.unscaledDeltaTime;

                if (pos.y >= endY)
                {
                    pos.y = endY;
                    textFinished = true;
                    textFinishedTime = Time.unscaledTime;

                    if (scrollingText != null)
                        scrollingText.gameObject.SetActive(false);
                }

                scrollingText.anchoredPosition = pos;
            }

            if (titleGroup != null && pos.y >= titleFadeStartY)
            {
                titleGroup.alpha = Mathf.MoveTowards(
                    titleGroup.alpha,
                    0f,
                    Time.unscaledDeltaTime * fadeSpeed
                );
            }
        }

        // LÍNEA RARA / COMPLEJA: Inyección Forzada de Foco de Navegación en el Sistema de Eventos de Entrada ('Update' - Continuación).
        // Tras verificar la conclusión del texto y superar el tiempo de gracia diferido por 'continueAppearDelay', desplaza la opacidad 
        // del botón de confirmación. Una vez consolidada la malla visual, activa los conmutadores lógicos 'interactable' y 'blocksRaycasts'. 
        // Acto seguido, accede al puntero global del motor de entradas de Unity ('EventSystem.current') para inyectar de forma directa 
        // el foco de selección en el objeto físico del botón, permitiendo que el usuario pueda interactuar mediante teclados o mandos inmediatamente.
        if (textFinished && continueButtonGroup != null)
        {
            if (Time.unscaledTime < textFinishedTime + continueAppearDelay)
                return;

            continueButtonGroup.alpha = Mathf.MoveTowards(
                continueButtonGroup.alpha,
                1f,
                Time.unscaledDeltaTime * fadeSpeed
            );

            if (continueButtonGroup.alpha >= 0.95f)
            {
                continueReady = true;
                continueButtonGroup.interactable = true;
                continueButtonGroup.blocksRaycasts = true;

                if (!continueSelected && continueButton != null && EventSystem.current != null)
                {
                    EventSystem.current.SetSelectedGameObject(continueButton.gameObject);
                    continueSelected = true;
                }
            }
        }
    }

    // Devuelve el estado booleano inverso del progreso para controlar la apertura inicial automática
    public bool ShouldShowTutorial()
    {
        return !tutorialCompleted;
    }

    // Intercepta los eventos de pulsación física del jugador deteniendo el flujo si el botón de salida no ha completado su desvanecimiento alpha
    public void TryCloseFromInput()
    {
        if (!continueReady)
            return;

        Close();
    }

    // LÍNEA RARA / COMPLEJA: Suspensión de Hilos de Simulación Dinámica y Despliegue de Lienzos del Tutorial ('Open').
    // Fuerza la congelación total del reloj interno del motor de juego asignando un valor nulo a la escala temporal ('Time.timeScale = 0f'), 
    // lo que paraliza los cálculos de físicas, traslaciones estándar e inteligencia artificial en segundo plano. Para evitar silenciar la escena, 
    // asegura la continuidad del canal de audio ('AudioListener.pause = false'). Reposiciona el contenedor de texto en su origen vertical ('startY'), 
    // redefine las banderas lógicas del script y actualiza el contenedor de estados globales del software a 'GameState.Tutorial'.
    public void Open()
    {
        isOpen = true;
        textFinished = false;
        continueReady = false;
        continueSelected = false;
        textFinishedTime = 0f;

        Time.timeScale = 0f;
        AudioListener.pause = false;

        if (scrollingText != null)
        {
            scrollingText.gameObject.SetActive(true);
            scrollingText.anchoredPosition = new Vector2(scrollingText.anchoredPosition.x, startY);
        }

        if (titleGroup != null)
            titleGroup.alpha = 1f;

        if (tutorialGroup != null)
        {
            tutorialGroup.interactable = true;
            tutorialGroup.blocksRaycasts = true;
        }

        if (continueButtonGroup != null)
        {
            continueButtonGroup.alpha = 0f;
            continueButtonGroup.interactable = false;
            continueButtonGroup.blocksRaycasts = false;
        }


        if (uiSoundPlayer != null)
            uiSoundPlayer.PlayOpenPanel();

        GameStateController.Instance.SetState(GameState.Tutorial);
    }

    // Restaura la velocidad del tiempo normal de juego, deshabilita las interacciones del panel gráfico y purga el foco de selección del periférico
    public void Close()
    {
        if (!isOpen)
            return;

        isOpen = false;
        tutorialCompleted = true;
        continueReady = false;
        continueSelected = false;

        Time.timeScale = 1f;
        AudioListener.pause = false;

        if (tutorialGroup != null)
        {
            tutorialGroup.interactable = false;
            tutorialGroup.blocksRaycasts = false;
        }

        if (continueButtonGroup != null)
        {
            continueButtonGroup.interactable = false;
            continueButtonGroup.blocksRaycasts = false;
        }

        if (uiSoundPlayer != null)
            uiSoundPlayer.PlayClosePanel();

        GameStateController.Instance.SetState(GameState.MapIdle);

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);
    }

    // Pone a cero de forma fulminante los canales alfa e interactividad de todos los elementos visuales del script sin interpolación lineal
    private void HideInstant()
    {
        isOpen = false;
        textFinished = false;
        continueReady = false;
        continueSelected = false;

        if (tutorialGroup != null)
        {
            tutorialGroup.alpha = 0f;
            tutorialGroup.interactable = false;
            tutorialGroup.blocksRaycasts = false;
        }

        if (titleGroup != null)
            titleGroup.alpha = 0f;

        if (continueButtonGroup != null)
        {
            continueButtonGroup.alpha = 0f;
            continueButtonGroup.interactable = false;
            continueButtonGroup.blocksRaycasts = false;
        }
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Controlador de Interfaz Gráfica del Panel de Tutorial y Texto Desplazable (TutorialPanelUI). 
   Su responsabilidad principal es desplegar una pantalla introductoria o narrativa en el juego, deteniendo por completo 
   el transcurso del tiempo del combate físico (`Time.timeScale = 0f`) mientras se animan e interpolan los textos explicativos, 
   asegurando que el usuario pueda leer las mecánicas de juego en un entorno controlado y sin penalizaciones tácticas.

   Características clave:
   1. Animación Inmune a la Pausa Temporal: Al basar sus transiciones y desplazamientos de posición (`RectTransform`) en la propiedad 
      `Time.unscaledDeltaTime`, el script garantiza que los desvanecimientos gráficos y el avance vertical del texto de estilo "créditos de cine" 
      funcionen de forma fluida e ininterrumpida aunque el motor del juego esté completamente detenido en el fondo.
   2. Control de Flujo Condicional de Interacción: Impide que el jugador se salte el tutorial de manera accidental al bloquear la 
      capacidad de cierre hasta que el texto haya alcanzado su coordenada de destino final (`endY`) y se haya completado un intervalo 
      de retardo de seguridad (`continueAppearDelay`). Esto asegura la lectura obligatoria de las directrices esenciales de diseño.
   3. Sincronización del Foco para Periféricos de Entrada: Utiliza clases nativas del Canvas de Unity (`CanvasGroup`) para alterar 
      la opacidad, interactividad física y bloqueo de rayos ópticos de los botones de forma unificada. Inyecta el foco de selección en el 
      `EventSystem` de manera automatizada al habilitar el botón, garantizando una accesibilidad nativa y total mediante mandos o teclados.
   ========================================================================================================
*/