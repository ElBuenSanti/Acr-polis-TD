using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] private CanvasGroup shopCanvasGroup;

    [Header("Construction Data")]
    [SerializeField] private ConstructionData[] shopConstructions;

    [Header("Cards")]
    [SerializeField] private RectTransform[] cardTransforms;
    [SerializeField] private Image[] cardHighlights;
    [SerializeField] private TextMeshProUGUI[] cardNames;
    [SerializeField] private TextMeshProUGUI[] cardCosts;

    [Header("Resources")]
    [SerializeField] private TextMeshProUGUI agapeText;
    [SerializeField] private TextMeshProUGUI iraText;
    [SerializeField] private TextMeshProUGUI merakiText;

    [Header("Settings")]
    [SerializeField] private int currentIndex;
    [SerializeField] private int maxIndex = 2;
    [SerializeField] private float selectedScale = 1.08f;
    [SerializeField] private float normalScale = 1f;
    [SerializeField] private float scaleSpeed = 10f;

    [Header("Detail")]
    [SerializeField] private GameObject cardsArea;
    [SerializeField] private GameObject detailsArea;

    [Header("Confirm Icons")]
    [SerializeField] private GameObject[] confirmIcons;

    // Inicializa por código el componente de opacidad si no se asignó en el editor
    private void Awake()
    {
        if (shopCanvasGroup == null)
            shopCanvasGroup = GetComponent<CanvasGroup>();
    }

    // Ejecuta la sincronización visual y de datos en el primer frame de ciclo de vida
    private void Start()
    {
        RefreshCards();
        RefreshResources();
        UpdateFocusVisual();
    }

    // Mantiene actualizados los medidores económicos de los dioses y procesa las interpolaciones mecánicas de las tarjetas
    private void Update()
    {
        RefreshResources();
        UpdateFocusVisual();
        UpdateCardAnimation();
    }

    // LÍNEA RARA / COMPLEJA: Interrupción por Estado de Oleada Activa e Inyección de Estado Global ('Open').
    // Interroga de forma restrictiva al planificador de hordas enemigos ('WaveSpawner.Instance.IsWaveRunning()'). 
    // Si la simulación física de combate está activa, bloquea el despliegue de la tienda enviando una señal de alerta visual 
    // al HUD flotante táctico. En caso contrario, muta de forma inmediata la máquina de estados central hacia 'GameState.ShopOpen' 
    // para congelar ciertas interacciones del entorno y aislar los controles del cursor en el carrusel de compras.
    public void Open()
    {

        if (WaveSpawner.Instance.IsWaveRunning())
        {
            if (StatusMessageUI.Instance != null)
                StatusMessageUI.Instance.ShowMessage("No puedes abrir la tienda durante la oleada");

            return;
        }
        GameStateController.Instance.SetState(GameState.ShopOpen);
        RefreshCards();
    }

    // Devuelve el flujo lógico del software al estado de exploración libre y redibuja los contenedores gráficos
    public void Close()
    {
        GameStateController.Instance.SetState(GameState.MapIdle);
        RefreshCards();
    }

    // Conmuta de forma segura la visibilidad de la tienda basándose en la configuración de la máquina de estados actual
    public void Toggle()
    {
        if (GameStateController.Instance.currentState == GameState.ShopOpen)
            Close();
        else if (GameStateController.Instance.currentState == GameState.MapIdle)
            Open();
    }

    // Decrementa el índice del carrusel aplicando un algoritmo de comportamiento cíclico infinito hacia la izquierda
    public void MoveLeft()
    {
        if (GameStateController.Instance.currentState != GameState.ShopOpen)
            return;

        currentIndex--;

        if (currentIndex < 0)
            currentIndex = maxIndex;

        RefreshCards();
    }

    // Incrementa el índice del carrusel aplicando un algoritmo de comportamiento cíclico infinito hacia la derecha
    public void MoveRight()
    {
        if (GameStateController.Instance.currentState != GameState.ShopOpen)
            return;

        currentIndex++;

        if (currentIndex > maxIndex)
            currentIndex = 0;

        RefreshCards();
    }

    // LÍNEA RARA / COMPLEJA: Desencadenamiento Transaccional de Construcción en Estado de Colocación ('ConfirmSelection').
    // Valida la escala de tiempo y las oleadas. Si la verificación es exitosa, transfiere el índice seleccionado del carrusel de la tienda 
    // al subsistema físico 'BuildingManager.Instance.SetConstructionIndex(currentIndex)'. Acto seguido, eleva el estado general del juego 
    // a 'GameState.PlacingTower', cerrando la interactividad del menú para habilitar la proyección del "ghost" o previsualización holográfica 
    // tridimensional de la estructura sobre la cuadrícula o terreno del plano táctico.
    public void ConfirmSelection()
    {

        if (WaveSpawner.Instance.IsWaveRunning())
        {
            if (StatusMessageUI.Instance != null)
                StatusMessageUI.Instance.ShowMessage("No puedes construir durante la oleada");

            return;
        }

        if (GameStateController.Instance.currentState != GameState.ShopOpen)
            return;

        BuildingManager.Instance.SetConstructionIndex(currentIndex);
        GameStateController.Instance.SetState(GameState.PlacingTower);
        RefreshCards();
    }

    // LÍNEA RARA / COMPLEJA: Iteración y Sincronización Masiva Heterogénea de Tarjetas Visuales ('RefreshCards').
    // Recorre de forma matricial los elementos del carrusel de la tienda. Ejecuta una validación lógica booleana cruzada 
    // que evalúa si el índice de la iteración coincide con el foco del jugador y si la tienda está abierta. Basándose en este resultado, 
    // enciende o apaga de forma síncrona las imágenes de resalte y los iconos de confirmación asociados, inyectando simultáneamente las cadenas 
    // de texto formateadas extraídas de los objetos de datos estructurados ('ConstructionData') de la base de datos de construcción.
    private void RefreshCards()
    {
        for (int i = 0; i < cardHighlights.Length; i++)
        {
            bool isSelected = i == currentIndex &&
                              GameStateController.Instance.currentState == GameState.ShopOpen;

            if (cardHighlights[i] != null)
                cardHighlights[i].enabled = isSelected;

            if (i < confirmIcons.Length && confirmIcons[i] != null)
                confirmIcons[i].SetActive(isSelected);

            if (i < shopConstructions.Length && shopConstructions[i] != null)
            {
                if (cardNames[i] != null)
                    cardNames[i].text = shopConstructions[i].type.ToString();

                if (cardCosts[i] != null)
                    cardCosts[i].text = GetFirstCost(shopConstructions[i]).ToString("0");
            }
        }
    }

    // Extrae de forma segura el valor de la primera divisa necesaria dentro del arreglo estructurado de pagos de la torre
    private float GetFirstCost(ConstructionData data)
    {
        if (data.willToPay == null || data.willToPay.Count == 0)
            return 0;

        return data.willToPay[0].amount;
    }

    // LÍNEA RARA / COMPLEJA: Deserialización y Sincronización Tripartita de Recursos Mitológicos ('RefreshResources').
    // Interroga al gestor financiero central ('WillManager.Instance') de forma síncrona frame a frame. Extrae y formatea en variables de texto 
    // de rendimiento nativo ('TextMeshProUGUI') los saldos exactos de los tres tipos de monedas divinas del juego: Ágape (Amor/Devoción), 
    // Ira (Furia Táctica/Guerra) y Meraki (Esencia/Artesanía), garantizando que el marcador de recursos del HUD refleje el capital exacto del jugador.
    private void RefreshResources()
    {
        if (WillManager.Instance == null)
            return;

        if (agapeText != null)
            agapeText.text = WillManager.Instance.GetMoney(Will.Agape).ToString("0");

        if (iraText != null)
            iraText.text = WillManager.Instance.GetMoney(Will.Ira).ToString("0");

        if (merakiText != null)
            merakiText.text = WillManager.Instance.GetMoney(Will.Meraki).ToString("0");
    }

    // Alterna la opacidad del contenedor completo del menú para simular un efecto estético de desvanecimiento por pérdida de foco
    private void UpdateFocusVisual()
    {
        if (shopCanvasGroup == null)
            return;

        shopCanvasGroup.alpha = GameStateController.Instance.currentState == GameState.ShopOpen ? 1f : 0.65f;
    }

    // LÍNEA RARA / COMPLEJA: Interpolación Vectorial de Escala por Evaluación Discriminante ('UpdateCardAnimation').
    // Ejecuta una evaluación por hardware frame a frame para determinar el tamaño ideal de cada tarjeta contenedora. 
    // Si la tarjeta inspeccionada posee el foco y la tienda está abierta, calcula un destino amplificado ('selectedScale'), 
    // en caso contrario apunta al tamaño base ('normalScale'). Transforma este valor flotante a un vector uniforme 'Vector3.one' 
    // y lo aplica al 'localScale' usando 'Vector3.Lerp' multiplicado por 'Time.deltaTime', brindando una suave respuesta elástica al navegar por el menú.
    private void UpdateCardAnimation()
    {
        for (int i = 0; i < cardTransforms.Length; i++)
        {
            if (cardTransforms[i] == null)
                continue;

            float targetScale = i == currentIndex &&
                                GameStateController.Instance.currentState == GameState.ShopOpen
                ? selectedScale
                : normalScale;

            Vector3 target = Vector3.one * targetScale;

            cardTransforms[i].localScale = Vector3.Lerp(
                cardTransforms[i].localScale,
                target,
                Time.deltaTime * scaleSpeed
            );
        }
    }


    // Desactiva los datos extendidos del menú para restablecer la vista limpia del carrusel de cartas de compra
    public void ShowCardsMode()
    {
        if (cardsArea != null) cardsArea.SetActive(true);
        if (detailsArea != null) detailsArea.SetActive(false);
    }

    // Oculta el carrusel de cartas estándar del HUD para dar visibilidad al lienzo técnico de detalles de estructura
    public void ShowDetailsMode()
    {
        if (cardsArea != null) cardsArea.SetActive(false);
        if (detailsArea != null) detailsArea.SetActive(true);
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como la Interfaz del Panel de Comercio e Invocaciones del HUD (ShopUI). Su responsabilidad 
   primordial es orquestar de manera visual el flujo económico y logístico del juego, sirviendo de interfaz interactiva 
   para que el jugador gaste sus recursos divinos y adquiera nuevas edificaciones defensivas durante los tiempos de tregua.

   Características clave:
   1. Blindaje Estratégico durante el Combate: Valida rigurosamente la línea temporal del juego mediante 'WaveSpawner'. 
      Esto impide que el jugador active la tienda o intente colocar estructuras en medio del caos de una invasión enemiga, 
      protegiendo el bucle principal de diseño táctico (fase de construcción frente a fase de defensa).
   2. Animaciones Elásticas de Tarjetas por Software: Gestiona las respuestas de escalado dinámico de las tarjetas de compra 
      mediante interpolaciones lineales vectoriales en lugar de instanciar pesados controladores de animación de Unity. Esto 
      aporta un feedback limpio y fluido al desplazarse horizontalmente a través de los índices de la tienda.
   3. Sincronización Económica Tripartita: Se acopla de manera eficiente a los managers de juego para leer en tiempo real 
      las tres energías sagradas (Ágape, Ira, Meraki). Al centralizar la actualización del texto en su bucle 'Update', 
      garantiza que cualquier alteración en los fondos del jugador se refleje instantáneamente en la interfaz de usuario.
   ========================================================================================================
*/