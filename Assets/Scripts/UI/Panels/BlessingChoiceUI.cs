using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BlessingChoiceUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Button[] blessingButtons;
    [SerializeField] private BlessingCardHighlightUI[] cardHighlights;

    [Header("Sound")]
    [SerializeField] private UISoundPlayer uiSoundPlayer;

    [Header("Animation")]
    [SerializeField] private float fadeSpeed = 8f;

    [Header("Navigation")]
    [SerializeField] private float inputDeadZone = 0.6f;
    [SerializeField] private float confirmDelay = 0.25f;

    private bool isOpen;
    private bool stickInUse;
    private int currentIndex;
    private float openTime;
    private ConstructionController templeController;

    // Inicializa ocultando el panel al arrancar el juego de forma instantánea
    private void Start()
    {
        HideInstant();
    }

    // Controla la transición de desvanecimiento del menú usando interpolación lineal asíncrona
    private void Update()
    {
        if (canvasGroup == null)
            return;

        float targetAlpha = isOpen ? 1f : 0f;

        canvasGroup.alpha = Mathf.Lerp(
            canvasGroup.alpha,
            targetAlpha,
            Time.unscaledDeltaTime * fadeSpeed
        );
    }

    // Configura e inicia el proceso de selección de bendiciones congelando el bucle del juego a un estado de menú
    public void Open(ConstructionController temple)
    {
        templeController = temple;
        isOpen = true;
        currentIndex = 0;
        stickInUse = false;
        openTime = Time.unscaledTime; // Guarda el segundo exacto en el que el menú apareció en la pantalla

        if (canvasGroup != null)
        {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }

        if (uiSoundPlayer != null)
            uiSoundPlayer.PlayOpenPanel();

        if (GameplaySoundPlayer.Instance != null)
            GameplaySoundPlayer.Instance.PlayBlessingOpen();

        // Informa a la máquina de estados central que el jugador se encuentra eligiendo una recompensa
        GameStateController.Instance.SetState(GameState.BlessingSelection);

        SelectCurrentButton();

        if (StatusMessageUI.Instance != null)
            StatusMessageUI.Instance.ShowMessage("Elige una condición de victoria");

        Debug.Log("Blessing panel opened. State: " + GameStateController.Instance.currentState);
    }

    // Desactiva el panel y devuelve el control al mapa interactivo principal del juego
    public void Close()
    {
        isOpen = false;

        if (canvasGroup != null)
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        if (uiSoundPlayer != null)
            uiSoundPlayer.PlayClosePanel();

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(null);

        // Devuelve el flujo del juego al estado de exploración libre
        GameStateController.Instance.SetState(GameState.MapIdle);
    }

    // LÍNEA RARA / COMPLEJA: Algoritmo de control por pasos discreto para Sticks Analógicos ('stickInUse').
    // Los joysticks envían datos continuos en cada frame (ej. si mantienes el stick a la derecha, enviará 1.0f constantemente).
    // Si no usáramos 'stickInUse', el cursor saltaría locamente entre las tarjetas a la velocidad de los FPS del juego.
    // Este algoritmo detecta si el stick pasó la zona muerta ('inputDeadZone'), ejecuta un único salto de índice y se bloquea.
    // No permitirá un nuevo movimiento hasta que el jugador suelte el joystick y este regrese al centro físico (menor que la zona muerta).
    public void Move(Vector2 input)
    {
        if (!isOpen)
            return;

        if (Mathf.Abs(input.x) < inputDeadZone)
        {
            stickInUse = false; // El stick regresó al centro, se libera el candado para permitir un nuevo movimiento
            return;
        }

        if (stickInUse)
            return; // Si el stick sigue inclinado, ignora el procesamiento de este frame

        stickInUse = true; // Activa el candado de pulsación única

        if (input.x > 0)
            currentIndex++;
        else
            currentIndex--;

        currentIndex = Mathf.Clamp(currentIndex, 0, blessingButtons.Length - 1);

        SelectCurrentButton();
    }

    // LÍNEA RARA / COMPLEJA: Candado de Confirmación por Retraso de Tiempo de Seguridad ('confirmDelay').
    // Evita pulsaciones accidentales fatales. Cuando un jugador está combatiendo y presionando frenéticamente los botones, 
    // si el panel de bendición se abre de golpe, podría confirmar y elegir una tarjeta por accidente en el primer frame.
    // Comparar 'Time.unscaledTime' con el tiempo de apertura más el retraso obliga al script a ignorar el botón de confirmación 
    // durante los primeros 0.25 segundos de vida del panel, garantizando que la elección sea 100% voluntaria.
    public void Confirm()
    {
        if (!isOpen)
            return;

        if (Time.unscaledTime < openTime + confirmDelay)
            return;

        if (currentIndex < 0 || currentIndex >= blessingButtons.Length)
            return;

        if (blessingButtons[currentIndex] == null)
            return;

        // Simula de manera programática un clic físico sobre el componente de botón seleccionado de la interfaz de usuario
        blessingButtons[currentIndex].onClick.Invoke();
    }

    // Coordina la sincronización visual entre el EventSystem de Unity y los scripts de resplandor de las tarjetas
    private void SelectCurrentButton()
    {
        if (blessingButtons == null || blessingButtons.Length == 0)
            return;

        currentIndex = Mathf.Clamp(currentIndex, 0, blessingButtons.Length - 1);

        Button selectedButton = blessingButtons[currentIndex];

        if (selectedButton == null)
            return;

        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(selectedButton.gameObject);

        selectedButton.Select();

        // Bucle que recorre todos los resaltadores secundarios de las tarjetas.
        // Si el índice del bucle coincide con la tarjeta apuntada, la enciende; de lo contrario, apaga su halo de luz.
        if (cardHighlights != null)
        {
            for (int i = 0; i < cardHighlights.Length; i++)
            {
                if (cardHighlights[i] != null)
                    cardHighlights[i].SetSelected(i == currentIndex);
            }
        }

        Debug.Log("Blessing selected index: " + currentIndex);
    }

    // Fuerza el apagado lógico e invisible del lienzo de forma fulminante en el arranque del nivel
    private void HideInstant()
    {
        isOpen = false;

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
    }

    // Métodos puente públicos mapeados en los eventos 'onClick' de los botones del Inspector para cada deidad
    public void ChooseAphrodite()
    {
        ChooseGod(GodType.Aphrodite);
    }

    public void ChooseAres()
    {
        ChooseGod(GodType.Ares);
    }

    public void ChooseHephaestus()
    {
        ChooseGod(GodType.Hephaestus);
    }

    // LÍNEA RARA / COMPLEJA: Inyección y mutación de datos de estructuras complejas a través de controladores de construcción.
    // Tras pasar los filtros de seguridad temporales, notifica al script del templo ('ConstructionController') qué Dios fue elegido.
    // Esto desencadena un cambio interno en el modelo de datos de la estructura física del mapa ('BaseConstruction.Data'), 
    // alterando dinámicamente sus estadísticas, su tipo lógico y su nivel para transformarlo en un templo dedicado.
    private void ChooseGod(GodType god)
    {
        if (!isOpen)
            return;

        if (Time.unscaledTime < openTime + confirmDelay)
            return;

        if (templeController == null)
            return;

        Debug.Log("Choosing blessing: " + god);

        templeController.SetSelectedGod(god);
        templeController.MarkBlessingChosen();
        templeController.Upgrade(); // Dispara la evolución o mejora de la estructura en el escenario 3D

        BaseConstruction baseConstruction = templeController.GetComponent<BaseConstruction>();

        if (baseConstruction != null && baseConstruction.Data != null)
        {
            Debug.Log("Temple after upgrade: "
                + baseConstruction.Data.type + " / "
                + baseConstruction.Data.god + " / Lv "
                + baseConstruction.Data.level);
        }

        if (GameplaySoundPlayer.Instance != null)
            GameplaySoundPlayer.Instance.PlayBlessing(god);

        if (StatusMessageUI.Instance != null)
            StatusMessageUI.Instance.ShowMessage("Condición elegida: " + god);

        Close(); // Cierra el panel y limpia la interfaz de usuario de la pantalla
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Menú de Selección de Bendiciones Divinas (BlessingChoiceUI). Es un componente dinámico 
   y crítico del HUD encargado de desplegar en pantalla una terna de opciones sagradas (Afrodita, Ares o Hefesto) 
   cuando el jugador interactúa con un templo o alcanza un hito de juego que define su estrategia de combate o victoria.

   Características clave:
   1. Control de Navegación Analógica Profesional: Resuelve mediante software el desfase de entradas continuas de 
      los mandos tradicionales. El uso del booleano de control 'stickInUse' transforma el flujo bruto del joystick en un 
      sistema de pulsación limpia, imitando de forma perfecta el comportamiento de los menús nativos de consolas.
   2. Seguridad Gráfica y Mecánica Integrada: Incorpora una ventana de tiempo de amortiguación ('confirmDelay') 
      que inmuniza la interfaz contra clics fantasmas o confirmaciones accidentales derivadas del estrés del combate en tiempo real.
   3. Conector de Lógica de Negocio (UI a Gameplay): No es un menú meramente cosmético. Se acopla de manera profunda 
      con los datos internos de las mecánicas de construcción del juego. Al procesar una selección, altera la metadata 
      del script del edificio del mapa, detonando sistemas de partículas, actualizaciones de nivel ('Upgrade') e impactando 
      directamente las mecánicas estratégicas de la partida.
   ========================================================================================================
*/