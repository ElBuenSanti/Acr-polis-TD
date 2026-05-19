using System.Collections.Generic;
using UnityEngine;

public class WillManager : MonoBehaviour
{
    public static WillManager Instance;

    private Dictionary<Will, float> money = new Dictionary<Will, float>();

    // LÍNEA RARA / COMPLEJA: Inicialización Arbitrada por Singleton Estricto y Población Procedural por Reflexión ('Awake').
    // Implementa una barrera de control Singleton: si 'Instance' es nula, reclama la exclusividad global; si está duplicada, 
    // se auto-destruye mediante 'Destroy(gameObject)' para evitar conflictos en memoria. Posteriormente, utiliza la API de reflexión 
    // 'System.Enum.GetValues' para extraer dinámicamente todos los literales del enumerador 'Will'. Popula de manera automatizada 
    // el diccionario asignando un fondo inicial de 1000 unidades a la moneda de tipo 'Agape' y reseteando las demás a cero.
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(gameObject);
        }

        foreach (Will w in System.Enum.GetValues(typeof(Will)))
        {
            if (w == Will.Agape)
            {
                money[w] = 1000;
            }
            else
            {
                money[w] = 0;
            }

        }
    }

    // Incrementa los fondos de una divisa mística indexando directamente el elemento en el mapa de claves del diccionario
    public void AddMoney(Will type, float amount)
    {
        money[type] += amount;
        Debug.Log($"+{amount} {type} | Total: {money[type]}");
    }

    // LÍNEA RARA / COMPLEJA: Deducción Financiera Atómica por Evaluación de Solvencia Condicional ('SpendMoney').
    // Este método opera como una transacción atómica segura para la economía del juego. En lugar de sustraer los fondos a ciegas, 
    // invoca de forma preventiva la función evaluadora 'CanAfford'. Si la consulta lógica retorna falso, el flujo se interrumpe 
    // de inmediato devolviendo 'false' al script solicitante; de lo contrario, aplica la deducción aritmética por operador compuesto 
    // ('-=') sobre el nodo indexado por la clave 'Will' e imprime la actualización en consola antes de confirmar el éxito.
    public bool SpendMoney(Will type, float amount)
    {
        if (!CanAfford(type, amount))
            return false;

        money[type] -= amount;
        Debug.Log($"-{amount} {type} | Total: {money[type]}");
        return true;
    }

    // Realiza una validación booleana directa comparando el saldo de la divisa solicitada contra el monto a debitar
    public bool CanAfford(Will type, float amount)
    {
        return money[type] >= amount;
    }

    // como consulta, util para UI
    // Extrae y devuelve el saldo flotante actual almacenado en el diccionario bajo la clave de la divisa especificada
    public float GetMoney(Will type)
    {
        return money[type];
    }

    // Encapsula y redirige la llamada hacia el método de solvencia económica para unificar criterios de validación de la UI
    public bool HasEnoughMoney(Will type, float amount)
    {
        return CanAfford(type, amount);
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Administrador Global del Sistema de Recursos Económicos (WillManager). Es el encargado 
   de centralizar, auditar y procesar todas las transacciones financieras dentro del juego, controlando múltiples divisas 
   esotéricas o tipos de energía mitológica representados por el enumerador `Will`. Sirve como puente de validación 
   crucial para los gestores de construcción y de interfaz de usuario (`BuildingManager`, `ConstructionController`), 
   impidiendo que se realicen operaciones o evoluciones si las cuentas del jugador no cuentan con fondos suficientes.

   Características clave:
   1. Patrón Singleton Centralizado Seguro: Garantiza que exista una única billetera lógica a lo largo de toda la sesión 
      de juego. Al implementar la auto-destrucción preventiva en el método `Awake`, previene bugs críticos de duplicación 
      de datos o variables corruptas si la escena se recarga o si se arrastran múltiples instancias por error.
   2. Arquitectura Eficiente por Diccionarios de C#: Utiliza una estructura de datos indexada (`Dictionary<Will, float>`) 
      en lugar de variables flotantes individuales y rígidas. Esto le confiere una complejidad de tiempo de búsqueda constante 
      u $O(1)$, haciendo que las consultas de saldo, adiciones o cobros de capital sean instantáneas independientemente 
      de cuántos tipos de recursos se añadan al diseño del juego en el futuro.
   3. Inicialización Automatizada y Escalable: Gracias al uso de la reflexión mediante `System.Enum.GetValues`, el script 
      se adapta de forma automática si se agregan nuevos tipos de monedas al código. Registra de forma limpia cada clave 
      en el diccionario y configura el balance inicial del juego (como el fondo inicial de 1000 para `Agape`) sin necesidad 
      de mapear manualmente cada entrada en el inspector.
   ========================================================================================================
*/