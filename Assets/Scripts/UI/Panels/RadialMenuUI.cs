using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RadialMenuUI : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private Canvas radialCanvas;
    [SerializeField] private RectTransform radialPanel;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Vector2 screenOffset = new Vector2(0f, 120f);

    [Header("Left Panel")]
    [SerializeField] private ShopUI shopUI;
    [SerializeField] private ConstructionDetailsUI detailsUI;

    [Header("Database")]
    [SerializeField] private ConstructionDatabase database;

    [Header("Highlights")]
    [SerializeField] private RadialOptionVisual aphroditeHighlight;
    [SerializeField] private RadialOptionVisual aresHighlight;
    [SerializeField] private RadialOptionVisual hephaestusHighlight;
    [SerializeField] private RadialOptionVisual sellHighlight;

    [Header("Cost Texts")]
    [SerializeField] private TextMeshProUGUI aphroditeCostText;
    [SerializeField] private TextMeshProUGUI aresCostText;
    [SerializeField] private TextMeshProUGUI hephaestusCostText;
    [SerializeField] private TextMeshProUGUI sellCostText;

    [Header("Level Texts")]
    [SerializeField] private TextMeshProUGUI aphroditeLevelText;
    [SerializeField] private TextMeshProUGUI aresLevelText;
    [SerializeField] private TextMeshProUGUI hephaestusLevelText;
    [SerializeField] private TextMeshProUGUI sellLevelText;

    [Header("Sell Hold")]
    [SerializeField] private float sellHoldTime = 3f;
    [SerializeField] private float refundPercent = 0.75f;
    [SerializeField] private GameObject sellHoldFillObject;
    [SerializeField] private Image sellHoldFillImage;

    [Header("Selection")]
    [SerializeField] private RadialOption selectedOption = RadialOption.None;

    [Header("Radial Animation")]
    [SerializeField] private CanvasGroup radialCanvasGroup;
    [SerializeField] private float openAnimationTime = 0.15f;
    [SerializeField] private float startScale = 0.85f;
    [SerializeField] private float endScale = 1f;

    //[SerializeField] private float invalidSoundCooldown = 0.6f;
    //private float lastInvalidSoundTime = -999f;

    private Coroutine radialAnimationRoutine;

    private ConstructionController selectedConstruction;
    private float sellTimer;

    // Vincula automáticamente las dependencias críticas mediante inyección en tiempo de ejecución si no se asignaron en el inspector
    private void Awake()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (shopUI == null) shopUI = FindAnyObjectByType<ShopUI>();
        if (detailsUI == null) detailsUI = FindAnyObjectByType<ConstructionDetailsUI>();
        if (database == null) database = FindAnyObjectByType<ConstructionDatabase>();
    }

    // Asegura el apagado inicial del menú radial para que comience inactivo en la escena
    private void Start()
    {
        Close();
    }

    // Monitorea el temporizador de retención mecánica ('Hold') necesario para confirmar la venta de la estructura seleccionada
    private void Update()
    {
        if (radialCanvas == null || !radialCanvas.enabled)
            return;

        if (selectedOption != RadialOption.Sell)
        {
            sellTimer = 0f;
            UpdateSellHoldVisual();
            return;
        }

        if (selectedConstruction == null || IsTempleSelected())
        {
            if (GameplaySoundPlayer.Instance != null)
                GameplaySoundPlayer.Instance.PlayInvalidPlacement();

            ShowStatus("El templo no se puede vender");
            return;
        }

        sellTimer += Time.deltaTime;
        UpdateSellHoldVisual();

        if (sellTimer >= sellHoldTime)
        {
            SellSelectedConstruction();
            Close();
            GameStateController.Instance.SetState(GameState.MapIdle);
        }
    }

    // Configura y despliega el menú radial posicionado sobre la estructura tridimensional correspondiente
    public void Open(ConstructionController construction)
    {
        if (WaveSpawner.Instance.IsWaveRunning())
        {
            if (GameplaySoundPlayer.Instance != null)
                GameplaySoundPlayer.Instance.PlayInvalidPlacement();
            ShowStatus("No puedes mejorar durante la oleada");
            return;
        }

        selectedConstruction = construction;
        selectedOption = RadialOption.None;
        sellTimer = 0f;

        MoveRadialToConstruction();

        if (radialCanvas != null)
            radialCanvas.enabled = true;
        PlayOpenAnimation();

        if (shopUI != null)
            shopUI.ShowDetailsMode();

        if (detailsUI != null)
            detailsUI.ShowBaseDetails(selectedConstruction);

        RefreshLevelTexts();
        RefreshVisuals();
        UpdateSellHoldVisual();
    }

    // Limpia las referencias de memoria y oculta el lienzo restableciendo los modos del panel lateral de la tienda
    public void Close()
    {
        selectedConstruction = null;
        selectedOption = RadialOption.None;
        sellTimer = 0f;

        if (radialCanvas != null)
            radialCanvas.enabled = false;

        if (shopUI != null)
            shopUI.ShowCardsMode();

        if (detailsUI != null)
            detailsUI.Clear();

        RefreshVisuals();
        UpdateSellHoldVisual();
    }

    // LÍNEA RARA / COMPLEJA: Segmentación por Cuadrantes de Entrada Vectorial Analógica ('ReadStick').
    // Procesa el vector bidimensional del joystick. Primero evalúa un umbral de zona muerta ('input.magnitude < 0.55f') para evitar
    // activaciones fantasmas por desgaste del hardware. Si se supera, compara los valores absolutos de los ejes X e Y para 
    // fragmentar la entrada en 4 regiones ortogonales perfectas (Izquierda, Derecha, Arriba, Abajo), mapeándolas de forma directa 
    // a las opciones del menú radial ('RadialOption') sin necesidad de realizar costosos cálculos trigonométricos de arco tangente.
    public void ReadStick(Vector2 input)
    {
        if (radialCanvas == null || !radialCanvas.enabled)
            return;

        if (input.magnitude < 0.55f)
        {
            if (selectedOption != RadialOption.None)
            {
                selectedOption = RadialOption.None;
                sellTimer = 0f;

                if (detailsUI != null)
                    detailsUI.ShowBaseDetails(selectedConstruction);

                RefreshVisuals();
                UpdateSellHoldVisual();
            }

            return;
        }

        RadialOption newOption;

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            newOption = input.x > 0 ? RadialOption.Ares : RadialOption.Hephaestus;
        }
        else
        {
            newOption = input.y > 0 ? RadialOption.Aphrodite : RadialOption.Sell;
        }

        if (newOption == selectedOption)
            return;

        selectedOption = newOption;
        sellTimer = 0f;

        if (detailsUI != null)
            detailsUI.ShowDetails(selectedConstruction, selectedOption);

        RefreshVisuals();
        UpdateSellHoldVisual();
    }

    // LÍNEA RARA / COMPLEJA: Ejecución del Sistema Transaccional de Mejoras Contextuales ('Confirm').
    // Inyecta dinámicamente las firmas de los dioses olímpicos ('GodType') en la estructura objetivo según la opción 
    // seleccionada en la UI. Este método altera la lógica del objeto en el mundo físico y desencadena la subida de nivel 
    // mediante la mutación de datos, actuando como el puente de confirmación definitivo entre la interfaz táctil y las estadísticas del juego.
    public void Confirm()
    {

        if (WaveSpawner.Instance.IsWaveRunning())
        {
            if (GameplaySoundPlayer.Instance != null)
                GameplaySoundPlayer.Instance.PlayInvalidPlacement();
            ShowStatus("No puedes hacer esto durante la oleada");
            return;
        }

        if (selectedConstruction == null || selectedOption == RadialOption.None)
            return;

        if (IsOptionLocked(selectedOption))
        {
            if (GameplaySoundPlayer.Instance != null)
                GameplaySoundPlayer.Instance.PlayInvalidPlacement();
            ShowStatus("Este camino de mejora está bloqueado");
            return;
        }

        switch (selectedOption)
        {
            case RadialOption.Aphrodite:
                selectedConstruction.SetSelectedGod(GodType.Aphrodite);
                selectedConstruction.Upgrade();
                GameplaySoundPlayer.Instance.PlayUpgrade();
                break;

            case RadialOption.Ares:
                selectedConstruction.SetSelectedGod(GodType.Ares);
                selectedConstruction.Upgrade();
                GameplaySoundPlayer.Instance.PlayUpgrade();
                break;

            case RadialOption.Hephaestus:
                selectedConstruction.SetSelectedGod(GodType.Hephaestus);
                selectedConstruction.Upgrade();
                GameplaySoundPlayer.Instance.PlayUpgrade();
                break;

            case RadialOption.Sell:
                ShowStatus("Holdea stick para vender");
                if (GameplaySoundPlayer.Instance != null)
                    GameplaySoundPlayer.Instance.PlaySell();
                return;
        }

        Close();
        GameStateController.Instance.SetState(GameState.MapIdle);
    }

    // LÍNEA RARA / COMPLEJA: Proyección de Coordenadas de Mundo Tridimensional a Pantalla Bidimensional con Delimitación de Bordes ('MoveRadialToConstruction').
    // Transforma la posición tridimensional (Vector3 XYZ) de la torre en el mapa a píxeles de pantalla (Vector3 XY) usando 'WorldToScreenPoint'.
    // Aplica un desfase vertical regulable ('screenOffset') para que el menú flote elegantemente sobre la estructura. Finalmente, restringe los
    // píxeles resultantes mediante un 'Mathf.Clamp' basado en un margen ('padding'), impidiendo que el panel radial se corte o desaparezca si 
    // el jugador selecciona una edificación que se encuentra muy cerca de los límites físicos de la pantalla del monitor.
    private void MoveRadialToConstruction()
    {
        if (selectedConstruction == null || radialPanel == null || mainCamera == null)
            return;

        Vector3 screenPosition = mainCamera.WorldToScreenPoint(selectedConstruction.transform.position);
        Vector3 targetPosition = screenPosition + (Vector3)screenOffset;

        float padding = 170f;

        targetPosition.x = Mathf.Clamp(targetPosition.x, padding, Screen.width - padding);
        targetPosition.y = Mathf.Clamp(targetPosition.y, padding, Screen.height - padding);

        radialPanel.position = targetPosition;
    }

    // Sincroniza las señales lumínicas de los botones y el estado de visibilidad de los costos económicos
    private void RefreshVisuals()
    {
        SetHighlight(aphroditeHighlight, selectedOption == RadialOption.Aphrodite);
        SetHighlight(aresHighlight, selectedOption == RadialOption.Ares);
        SetHighlight(hephaestusHighlight, selectedOption == RadialOption.Hephaestus);
        SetHighlight(sellHighlight, selectedOption == RadialOption.Sell);

        UpdateCostText(aphroditeCostText, RadialOption.Aphrodite);
        UpdateCostText(aresCostText, RadialOption.Ares);
        UpdateCostText(hephaestusCostText, RadialOption.Hephaestus);
        UpdateCostText(sellCostText, RadialOption.Sell);
    }

    // Refresca de forma masiva los textos informativos de los niveles de deidades correspondientes
    private void RefreshLevelTexts()
    {
        UpdateLevelText(aphroditeLevelText, RadialOption.Aphrodite);
        UpdateLevelText(aresLevelText, RadialOption.Ares);
        UpdateLevelText(hephaestusLevelText, RadialOption.Hephaestus);

        if (sellLevelText != null)
            sellLevelText.text = IsTempleSelected() ? "No Sell" : "Sell";
    }

    // Inspecciona la base de datos para imprimir el nivel futuro de la mejora o marcar el tope máximo de evolución
    private void UpdateLevelText(TextMeshProUGUI text, RadialOption option)
    {
        if (text == null) return;

        if (IsOptionLocked(option))
        {
            if (GameplaySoundPlayer.Instance != null)
                GameplaySoundPlayer.Instance.PlayInvalidPlacement();
            text.text = "Locked";
            return;
        }

        ConstructionData currentData = GetCurrentData();
        if (currentData == null)
        {
            text.text = "";
            return;
        }

        GodType god = GetGodFromOption(option);
        int nextLevel = currentData.level + 1;

        ConstructionData nextData = database.GetData(currentData.type, god, nextLevel);
        text.text = nextData == null ? "MAX" : "Level " + nextLevel;
    }

    // Calcula de manera contextual el valor numérico del costo o el valor exacto de reembolso por desmantelamiento
    private void UpdateCostText(TextMeshProUGUI text, RadialOption option)
    {
        if (text == null) return;

        bool shouldShow = selectedOption == option;
        text.gameObject.SetActive(shouldShow);

        if (!shouldShow) return;

        if (option == RadialOption.Sell)
        {
            ConstructionData currentData = GetCurrentData();

            if (currentData == null)
            {
                text.text = "";
                return;
            }

            if (currentData.type == ConstructionType.Temple)
            {
                text.text = "No";
                return;
            }

            text.text = GetTotalRefund(currentData).ToString("0");
            return;
        }

        if (IsOptionLocked(option))
        {
            if (GameplaySoundPlayer.Instance != null)
                GameplaySoundPlayer.Instance.PlayInvalidPlacement();
            text.text = "Locked";
            return;
        }

        ConstructionData data = GetCurrentData();
        if (data == null)
        {
            text.text = "";
            return;
        }

        GodType god = GetGodFromOption(option);
        int nextLevel = data.level + 1;
        ConstructionData nextData = database.GetData(data.type, god, nextLevel);

        text.text = nextData == null ? "MAX" : GetCostText(nextData);
    }

    // Altera la propiedad 'fillAmount' de la imagen radial para mostrar el progreso circular del temporizador de demolición
    private void UpdateSellHoldVisual()
    {
        bool isSelling = selectedOption == RadialOption.Sell && sellTimer > 0f;

        if (sellHoldFillObject != null)
            sellHoldFillObject.SetActive(isSelling);

        if (sellHoldFillImage != null)
            sellHoldFillImage.fillAmount = Mathf.Clamp01(sellTimer / sellHoldTime);
    }

    // Valida si una ruta de evolución divina está vedada por haber elegido previamente la devoción a otro dios
    private bool IsOptionLocked(RadialOption option)
    {
        if (option == RadialOption.Sell) return false;

        ConstructionData currentData = GetCurrentData();
        if (currentData == null || currentData.god == GodType.Base)
            return false;

        return currentData.god != GetGodFromOption(option);
    }

    // Comprueba si la estructura seleccionada corresponde al Templo principal (cuya venta está prohibida por diseño)
    private bool IsTempleSelected()
    {
        ConstructionData data = GetCurrentData();
        return data != null && data.type == ConstructionType.Temple;
    }

    // LÍNEA RARA / COMPLEJA: Algoritmo de Demolición Estructural e Iteración de Miembros de Grupo ('SellSelectedConstruction').
    // Procesa el desmantelamiento de la edificación. Si la estructura pertenece a un grupo acoplado ('selectedConstruction.group'), 
    // realiza una copia idéntica de la lista de sus miembros asociados ('new List<ConstructionController>') antes de destruirlos. 
    // Esto es crucial para prevenir un bug clásico de interrupción por mutación de colecciones ('InvalidOperationException'), lo que 
    // permite ejecutar el método de muerte ('.Die()') en cada componente esclavo y purgar el contenedor espacial limpiamente.
    private void SellSelectedConstruction()
    {
        if (selectedConstruction == null || IsTempleSelected())
            return;

        RefundConstruction(selectedConstruction);

        BaseConstruction baseConstruction = selectedConstruction.GetComponent<BaseConstruction>();
        if (baseConstruction == null) return;

        if (selectedConstruction.group != null && selectedConstruction.group.members.Count > 0)
        {
            List<ConstructionController> membersCopy = new List<ConstructionController>(selectedConstruction.group.members);

            foreach (ConstructionController member in membersCopy)
            {
                if (member == null) continue;

                BaseConstruction memberConstruction = member.GetComponent<BaseConstruction>();
                if (memberConstruction != null)
                    memberConstruction.Die();
            }

            Destroy(selectedConstruction.group.gameObject);
        }
        else
        {
            baseConstruction.Die();
        }

        BuildingManager.Instance.selectedConstruction = null;
        GameplaySoundPlayer.Instance.PlaySell();
        ShowStatus("Construcción vendida");
    }

    // Reembolsa los recursos financieros sumándolos al monedero global del jugador multiplicados por la tasa de retorno
    private void RefundConstruction(ConstructionController construction)
    {
        BaseConstruction baseConstruction = construction.GetComponent<BaseConstruction>();
        if (baseConstruction == null || baseConstruction.Data == null) return;

        foreach (WillProduction refund in GetRefundList(baseConstruction.Data))
        {
            WillManager.Instance.AddMoney(refund.type, refund.amount * refundPercent);
        }
    }

    // Devuelve la sumatoria aritmética neta de todos los recursos recuperables tras deconstruir la edificación
    private float GetTotalRefund(ConstructionData currentData)
    {
        float total = 0f;

        foreach (WillProduction refund in GetRefundList(currentData))
            total += refund.amount * refundPercent;

        return total;
    }

    // LÍNEA RARA / COMPLEJA: Cálculo de Reembolso Acumulativo Retroactivo por Niveles ('GetRefundList').
    // Resuelve el problema de devolver los recursos invertidos. No solo extrae el valor del nivel actual, sino que si la torre
    // fue mejorada progresivamente (por ejemplo, de nivel 1 a nivel 3), ejecuta un bucle histórico ('for') que consulta de forma 
    // secuencial en la base de datos los costos de manufactura de cada escalón evolutivo previo, acumulándolos en una lista unificada.
    private List<WillProduction> GetRefundList(ConstructionData currentData)
    {
        List<WillProduction> refunds = new List<WillProduction>();

        AddCosts(refunds, currentData);

        if (currentData.god != GodType.Base)
        {
            for (int level = 2; level <= currentData.level; level++)
                AddCosts(refunds, database.GetData(currentData.type, currentData.god, level));
        }

        return refunds;
    }

    // Fusiona o añade de forma limpia los costes de una estructura dentro del compendio acumulativo de reembolsos
    private void AddCosts(List<WillProduction> refunds, ConstructionData data)
    {
        if (data == null || data.willToPay == null) return;

        foreach (WillProduction cost in data.willToPay)
        {
            WillProduction existing = refunds.Find(x => x.type == cost.type);

            if (existing != null)
                existing.amount += cost.amount;
            else
                refunds.Add(new WillProduction { type = cost.type, amount = cost.amount });
        }
    }

    // Obtiene de forma segura el contenedor de datos ('ConstructionData') de la edificación analizada
    private ConstructionData GetCurrentData()
    {
        if (selectedConstruction == null) return null;

        BaseConstruction baseConstruction = selectedConstruction.GetComponent<BaseConstruction>();
        return baseConstruction != null ? baseConstruction.Data : null;
    }

    // Vincula la enumeración de control del panel radial con el identificador del panteón mitológico del juego
    private GodType GetGodFromOption(RadialOption option)
    {
        switch (option)
        {
            case RadialOption.Aphrodite: return GodType.Aphrodite;
            case RadialOption.Ares: return GodType.Ares;
            case RadialOption.Hephaestus: return GodType.Hephaestus;
            default: return GodType.Base;
        }
    }

    // Formatea los arreglos económicos de costo para extraer el primer valor válido en forma de cadena de texto
    private string GetCostText(ConstructionData data)
    {
        if (data.willToPay == null || data.willToPay.Count == 0)
            return "0";

        return data.willToPay[0].amount.ToString("0");
    }

    // Modifica de manera directa el aspecto gráfico del resalte visual de la opción apuntada
    private void SetHighlight(RadialOptionVisual highlight, bool active)
    {
        if (highlight != null)
            highlight.SetActiveVisual(active);
    }

    // Comunica alertas de estado al sistema flotante del HUD y escribe en la bitácora de desarrollo
    private void ShowStatus(string message)
    {
        if (StatusMessageUI.Instance != null)
            StatusMessageUI.Instance.ShowMessage(message);

        Debug.Log(message);
    }

    // Inicia de manera controlada la corrutina de escalado e interpolación del menú radial protegiendo hilos duplicados
    private void PlayOpenAnimation()
    {
        if (radialAnimationRoutine != null)
            StopCoroutine(radialAnimationRoutine);

        radialAnimationRoutine = StartCoroutine(OpenAnimationRoutine());
    }

    // LÍNEA RARA / COMPLEJA: Corrutina de Despliegue con Suavizado Hermite Cúbico ('OpenAnimationRoutine').
    // Controla la animación del panel a lo largo del tiempo de manera asíncrona mediante un bucle 'while (yield return null)'.
    // En lugar de usar una interpolación lineal cruda, calcula un factor de paso suavizado mediante 'Mathf.SmoothStep', 
    // el cual aplica una aceleración y desaceleración fluida en los extremos (fórmula de interpolación cúbica de Hermite).
    // Esto manipula el tamaño ('localScale') y la opacidad ('alpha') simultáneamente, confiriendo un acabado visual orgánico al HUD.
    private IEnumerator OpenAnimationRoutine()
    {
        float timer = 0f;

        radialPanel.localScale = Vector3.one * startScale;

        if (radialCanvasGroup != null)
            radialCanvasGroup.alpha = 0f;

        while (timer < openAnimationTime)
        {
            timer += Time.deltaTime;
            float t = timer / openAnimationTime;
            t = Mathf.SmoothStep(0f, 1f, t);

            radialPanel.localScale = Vector3.Lerp(
                Vector3.one * startScale,
                Vector3.one * endScale,
                t
            );

            if (radialCanvasGroup != null)
                radialCanvasGroup.alpha = t;

            yield return null;
        }

        radialPanel.localScale = Vector3.one * endScale;

        if (radialCanvasGroup != null)
            radialCanvasGroup.alpha = 1f;
    }

    // LÍNEA RARA / COMPLEJA: Ventana Estricta de Restricción Acústica por Cooldown ('PlayInvalidSoundWithCooldown').
    // Utiliza una marca de tiempo basada en 'Time.unscaledTime' para controlar el flujo de llamadas de audio.
    // Si el jugador realiza comandos erróneos de forma masiva en el joystick (lo que normalmente causaría un solapamiento 
    // estridente e insoportable del archivo de audio de error), este método bloquea las peticiones sucesivas hasta que 
    // transcurra la ventana de enfriamiento definida en 'invalidSoundCooldown', protegiendo la integridad acústica del juego.



    //private void PlayInvalidSoundWithCooldown()
    //{
    //    if (Time.unscaledTime < lastInvalidSoundTime + invalidSoundCooldown)
    //        return;

    //    lastInvalidSoundTime = Time.unscaledTime;

    //    if (GameplaySoundPlayer.Instance != null)
    //        GameplaySoundPlayer.Instance.PlayInvalidPlacement();
    //}
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Menú Radial Interactivo de Gestión y Evolución Divina (RadialMenuUI). Es una pieza central 
   de la experiencia de usuario (UX) para mecánicas del género Tower Defense o estrategia mitológica. Su labor principal 
   es proyectarse dinámicamente sobre el mapa de juego para permitirle al usuario especializar sus edificaciones bajo la 
   influencia de diferentes deidades (Afrodita, Ares, Hefesto) o ejecutar procesos seguros de venta y demolición.

   Características clave:
   1. Proyección Espacial Inteligente: Traduce dinámicamente coordenadas tridimensionales de juego a píxeles exactos 
      de pantalla ('WorldToScreenPoint'). Además, incorpora algoritmos de protección perimetral ('Clamp') para que el 
      menú jamás sea renderizado fuera de los límites visibles de la pantalla del monitor.
   2. Navegación Avanzada por Analógico: Diseñado con un fuerte enfoque multiplataforma (Consolas/PC). Segmenta las 
      direcciones vectoriales de los mandos mediante una matriz cruzada de valores absolutos, detectando la dirección 
      deseada por el jugador de manera instantánea y aislando las perturbaciones de ruido mecánico de los sticks.
   3. Economía de Deconstrucción Retroactiva: Cuenta con un motor financiero inteligente capaz de escanear el árbol 
      genealógico de mejoras de la edificación apuntada. Al calcular recursivamente la suma de costes históricos de todos 
      los niveles superados, asegura un reembolso justo ('refundPercent') al jugador tras deconstruir estructuras avanzadas.
   ========================================================================================================
*/