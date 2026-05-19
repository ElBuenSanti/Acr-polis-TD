using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConstructionDetailsUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI costText;

    [Header("Stats")]
    [SerializeField] private StatBarUI resistanceStat;
    [SerializeField] private StatBarUI aditamentResistanceStat;
    [SerializeField] private StatBarUI damageStat;
    [SerializeField] private StatBarUI actionSpeedStat;
    [SerializeField] private StatBarUI movementSpeedStat;
    [SerializeField] private StatBarUI rangeStat;
    [SerializeField] private StatBarUI projectileSpeedStat;

    [Header("Database")]
    [SerializeField] private ConstructionDatabase database;

    [Header("Max Values")]
    [SerializeField] private float maxResistance = 1000f;
    [SerializeField] private float maxAditamentResistance = 500f;
    [SerializeField] private float maxDamage = 500f;
    [SerializeField] private float maxActionSpeed = 10f;
    [SerializeField] private float maxMovementSpeed = 10f;
    [SerializeField] private float maxRange = 20f;
    [SerializeField] private float maxProjectileSpeed = 20f;

    [Header("Sell")]
    [SerializeField] private float refundPercent = 0.75f;

    // Busca automáticamente el repositorio de datos de construcción si no fue asignado en el Inspector
    private void Awake()
    {
        if (database == null)
            database = FindAnyObjectByType<ConstructionDatabase>();
    }

    // Extrae y proyecta las estadísticas actuales de la edificación seleccionada sin proyecciones de cambio
    public void ShowBaseDetails(ConstructionController construction)
    {
        if (construction == null)
        {
            Clear();
            return;
        }

        BaseConstruction baseConstruction = construction.GetComponent<BaseConstruction>();

        if (baseConstruction == null || baseConstruction.Data == null)
        {
            Clear();
            return;
        }

        ConstructionData data = baseConstruction.Data;

        if (titleText != null)
            titleText.text = data.type + " Base";

        if (costText != null)
            costText.text = "Current stats";

        // LÍNEA RARA / COMPLEJA: 'ShowStats(data, data)'.
        // Pasa el mismo contenedor de datos tanto para el valor actual como para el valor modificado.
        // Esto le indica internamente a las barras de estadísticas ('StatBarUI') que no hay ninguna alteración, 
        // congelando las barras en un color base neutral sin animar previsualizaciones de aumento o decremento.
        ShowStats(data, data);
    }

    // LÍNEA RARA / COMPLEJA: Sistema Predictivo de Interfaz basado en Menús Radiales ('RadialOption').
    // Reacciona en tiempo real según el botón físico o la opción flotante sobre la que el jugador posicione el cursor.
    // Discierne de forma dinámica si debe renderizar un estado de bloqueo de camino divino, el nivel máximo alcanzado, 
    // la devaluación por desmantelamiento (venta) o la previsualización exacta del siguiente nivel de la estructura.
    public void ShowDetails(ConstructionController construction, RadialOption option)
    {
        if (construction == null || option == RadialOption.None)
        {
            ShowBaseDetails(construction);
            return;
        }

        BaseConstruction baseConstruction = construction.GetComponent<BaseConstruction>();

        if (baseConstruction == null || baseConstruction.Data == null)
        {
            Clear();
            return;
        }

        ConstructionData currentData = baseConstruction.Data;

        if (option == RadialOption.Sell)
        {
            ShowSellDetails(currentData);
            return;
        }

        GodType god = GetGodFromOption(option);

        // Caso A: El camino divino está bloqueado debido a una elección de deidad previa incompatible
        if (IsLocked(currentData, god))
        {
            if (titleText != null)
                titleText.text = god + " Locked";

            if (costText != null)
                costText.text = "Path unavailable";

            ShowStats(currentData, currentData);
            return;
        }

        int nextLevel = currentData.level + 1;
        ConstructionData nextData = database.GetData(currentData.type, god, nextLevel);

        // Caso B: No existen más configuraciones de nivel en la base de datos (Estructura al máximo)
        if (nextData == null)
        {
            if (titleText != null)
                titleText.text = god + " MAX";

            if (costText != null)
                costText.text = "No more upgrades";

            ShowStats(currentData, currentData);
            return;
        }

        // Caso C: Camino libre. Proyecta los textos informativos y el contraste comparativo de estadísticas
        if (titleText != null)
            titleText.text = god + " Lv. " + nextLevel;

        if (costText != null)
            costText.text = GetCostText(nextData);

        ShowStats(currentData, nextData);
    }

    // Gestiona la previsualización del desmantelamiento de la estructura y el vaciado completo de sus barras
    private void ShowSellDetails(ConstructionData currentData)
    {
        // REGLA DE ORO DE JUEGO: Los templos principales actúan como bases de operaciones vitales.
        // Impedir su venta mediante código duro previene que el jugador rompa el bucle de juego o se quede sin nexo de control.
        if (currentData.type == ConstructionType.Temple)
        {
            if (titleText != null)
                titleText.text = "Cannot Sell";

            if (costText != null)
                costText.text = "Temple cannot be sold";

            ShowStats(currentData, currentData);
            return;
        }

        if (titleText != null)
            titleText.text = "Sell";

        if (costText != null)
            costText.text = GetRefundText(currentData);

        ShowStats(currentData, currentData);
    }

    // Inyecta los valores correspondientes en cada una de las 7 barras de estadísticas de la interfaz
    private void ShowStats(ConstructionData currentData, ConstructionData nextData)
    {
        resistanceStat.SetStat("Resistance", currentData.resistance, nextData.resistance, maxResistance);
        aditamentResistanceStat.SetStat("Aditament", currentData.aditamentResistance, nextData.aditamentResistance, maxAditamentResistance);
        damageStat.SetStat("Damage", currentData.attackDamage, nextData.attackDamage, maxDamage);
        actionSpeedStat.SetStat("Action Speed", currentData.actionVelocity, nextData.actionVelocity, maxActionSpeed);
        movementSpeedStat.SetStat("Move Speed", currentData.movementSpeed, nextData.movementSpeed, maxMovementSpeed);
        rangeStat.SetStat("Range", currentData.range, nextData.range, maxRange);
        projectileSpeedStat.SetStat("Projectile", currentData.proyectileVelocity, nextData.proyectileVelocity, maxProjectileSpeed);
    }

    // Limpia los textos informativos y reinicia los componentes visuales de las barras de estadísticas
    public void Clear()
    {
        if (titleText != null)
            titleText.text = "Structure Details";

        if (costText != null)
            costText.text = "";

        resistanceStat.Clear();
        aditamentResistanceStat.Clear();
        damageStat.Clear();
        actionSpeedStat.Clear();
        movementSpeedStat.Clear();
        rangeStat.Clear();
        projectileSpeedStat.Clear();
    }

    // Comprueba si la estructura ya está comprometida con una deidad diferente a la seleccionada
    private bool IsLocked(ConstructionData currentData, GodType selectedGod)
    {
        if (currentData.god == GodType.Base)
            return false;

        return currentData.god != selectedGod;
    }

    // Conversor lógico que mapea las opciones del menú radial con sus respectivos identificadores de deidad
    private GodType GetGodFromOption(RadialOption option)
    {
        switch (option)
        {
            case RadialOption.Aphrodite:
                return GodType.Aphrodite;

            case RadialOption.Ares:
                return GodType.Ares;

            case RadialOption.Hephaestus:
                return GodType.Hephaestus;

            default:
                return GodType.Base;
        }
    }

    // Construye la cadena de texto formateada para representar los costos de adquisición o mejora
    private string GetCostText(ConstructionData data)
    {
        if (data.willToPay == null || data.willToPay.Count == 0)
            return "Free";

        string text = "";

        for (int i = 0; i < data.willToPay.Count; i++)
        {
            WillProduction cost = data.willToPay[i];

            text += cost.amount.ToString("0") + " " + cost.type;

            if (i < data.willToPay.Count - 1)
                text += " / ";
        }

        return text;
    }

    // LÍNEA RARA / COMPLEJA: Formateador dinámico de texto de devoluciones económicas.
    // Traduce la lista de estructuras de costo a un formato de texto legible para el HUD, aplicando el factor de penalización 
    // por venta ('refundPercent') e intercalando caracteres de separación (" / ") de manera limpia y sin dejar residuos al final.
    private string GetRefundText(ConstructionData currentData)
    {
        List<WillProduction> refunds = GetRefundList(currentData);

        if (refunds.Count == 0)
            return "Refund: 0";

        string text = "Refund: ";

        for (int i = 0; i < refunds.Count; i++)
        {
            text += (refunds[i].amount * refundPercent).ToString("0") + " " + refunds[i].type;

            if (i < refunds.Count - 1)
                text += " / ";
        }

        return text;
    }

    // LÍNEA RARA / COMPLEJA: Algoritmo de Cálculo Retroactivo de Reembolsos de Construcción.
    // Evita pérdidas económicas injustas para el jugador. Cuando una estructura ha sido mejorada a nivel 3 o 4, 
    // no basta con calcular el costo de su nivel actual. Este bucle camina hacia atrás en la historia de la torre, 
    // extrayendo los costos guardados en la base de datos desde el nivel base (Nivel 1) e iterando de manera ascendente 
    // para acumular de forma precisa cada recurso invertido por el jugador a lo largo de la partida.
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

    // LÍNEA RARA / COMPLEJA: Agregador y Consolidador de Listas de Recursos mediante Predicados Lambda ('Find').
    // Resuelve el problema de la mezcla de diferentes tipos de divisas o energías. Al procesar un costo, busca mediante 
    // una expresión matemática compacta ('x => x.type == cost.type') si el recurso ya fue registrado previamente en la lista.
    // Si existe, incrementa su valor numérico; de lo contrario, inyecta un nuevo nodo de datos en el vector de reembolsos.
    private void AddCosts(List<WillProduction> refunds, ConstructionData data)
    {
        if (data == null || data.willToPay == null)
            return;

        foreach (WillProduction cost in data.willToPay)
        {
            WillProduction existing = refunds.Find(x => x.type == cost.type);

            if (existing != null)
                existing.amount += cost.amount;
            else
                refunds.Add(new WillProduction { type = cost.type, amount = cost.amount });
        }
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Inspector y Comparador Avanzado de Estadísticas de Estructuras (ConstructionDetailsUI). 
   Es el núcleo de información de la interfaz de usuario para mecánicas de estrategia y Tower Defense, responsable de 
   calcular, formatear y proyectar las fluctuaciones numéricas de las 7 estadísticas de combate y resistencia del juego.

   Características clave:
   1. Sistema de Previsualización Diferencial (Estadísticas Futuras): Está diseñado para trabajar en perfecta sinergia 
      con las barras de estadísticas ('StatBarUI'). Al enviarle un estado actual y un estado futuro potencial ('nextData'), 
      la interfaz es capaz de mostrarle al jugador de forma visual cuánto aumentarán sus atributos (ej: iluminando en verde 
      el segmento extra de la barra de daño o rango) antes de que gaste sus recursos.
   2. Motor de Reembolso Histórico Acumulativo: Implementa un robusto sistema contable para la economía del juego. 
      Al rastrear y sumar el costo total invertido en cada nivel intermedio de una estructura, garantiza que la venta de 
      torres avanzadas devuelva un porcentaje exacto y justo ('refundPercent') de toda la fortuna invertida en ella.
   3. Interfaz Centrada en Datos (Data-Driven UI): El script no almacena información de balances de juego por sí mismo. 
      Depende por completo de los datos crudos extraídos de la base de datos de estructuras ('ConstructionDatabase'). Esto 
      permite a los diseñadores de juego modificar los valores de daño, coste o velocidad de las torres en los archivos de 
      configuración sin necesidad de reescribir o alterar una sola línea de código del HUD.
   ========================================================================================================
*/