using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatBarUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI statNameText;
    [SerializeField] private Image baseBar;
    [SerializeField] private Image changeBar;

    [Header("Colors")]
    [SerializeField] private Color baseColor = Color.gray;
    [SerializeField] private Color upgradeColor = Color.green;
    [SerializeField] private Color downgradeColor = Color.red;
    [SerializeField] private Color sameColor = Color.clear;

    // LÍNEA RARA / COMPLEJA: Normalización Aritmética Blindada contra Divisiones por Cero ('SetStat').
    // Calcula los porcentajes de relleno ('currentFill' y 'nextFill') dividiendo los valores absolutos entre el tope máximo.
    // Utiliza un operador ternario ('maxValue <= 0 ? 0 : ...') que actúa como un escudo matemático para interceptar cualquier 
    // valor nulo o negativo en la salud/daño de la torre, evitando errores de desbordamiento o valores indeterminados (NaN). 
    // Finalmente, 'Mathf.Clamp01' restringe estrictamente el flotante resultante dentro del intervalo cerrado de [0f, 1f].
    public void SetStat(string statName, float currentValue, float nextValue, float maxValue)
    {
        if (statNameText != null)
            statNameText.text = statName;

        float currentFill = maxValue <= 0 ? 0 : Mathf.Clamp01(currentValue / maxValue);
        float nextFill = maxValue <= 0 ? 0 : Mathf.Clamp01(nextValue / maxValue);

        if (baseBar != null)
        {
            baseBar.color = baseColor;
            baseBar.fillAmount = currentFill;
        }

        if (changeBar == null)
            return;

        // LÍNEA RARA / COMPLEJA: Evaluación de Tolerancia en Precisión de Punto Flotante ('Mathf.Approximately').
        // Debido a las imprecisiones binarias nativas de los procesadores al operar números tipo 'float', hacer 'currentFill == nextFill' 
        // puede fallar por diferencias infinitesimales (ej. 0.5000001 frente a 0.5). Este método compara ambos valores aplicando un 
        // margen de tolerancia épsilon ($\epsilon$). Si la diferencia es insignificante, asume igualdad, oculta la barra con 'sameColor' 
        // y vacía su relleno para ahorrar operaciones de renderizado en el HUD.
        if (Mathf.Approximately(currentFill, nextFill))
        {
            changeBar.color = sameColor;
            changeBar.fillAmount = 0f;
            return;
        }

        // LÍNEA RARA / COMPLEJA: Algoritmo de Superposición de Barras para Previsualización de Cambios (Buff/Debuff).
        // Si el valor futuro es mayor ('nextFill > currentFill'), tiñe la barra de mejora ('upgradeColor') y estira su relleno hasta el 'nextFill'. 
        // Como la barra de cambios se ubica detrás (o delante con mezcla adecuada) de la barra base, el exceso sobresale simulando la ganancia. 
        // En caso de decremento, la tiñe de 'downgradeColor' y clava su relleno en el 'currentFill' original; al vaciarse la barra base por debajo, 
        // el segmento remanente expone visualmente la pérdida exacta de estadísticas que sufrirá la edificación.
        if (nextFill > currentFill)
        {
            changeBar.color = upgradeColor;
            changeBar.fillAmount = nextFill;
        }
        else
        {
            changeBar.color = downgradeColor;
            changeBar.fillAmount = currentFill;
        }
    }

    // Restablece por completo los contenedores de texto y vacía los factores de relleno de ambas imágenes de la UI
    public void Clear()
    {
        if (statNameText != null)
            statNameText.text = "";

        if (baseBar != null)
            baseBar.fillAmount = 0f;

        if (changeBar != null)
            changeBar.fillAmount = 0f;
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Controlador de Barras de Estadísticas Comparativas (StatBarUI). Es un componente 
   avanzado de interfaces gráficas para videojuegos RPG, estrategia o Tower Defense, acoplado comúnmente al panel 
   de detalles de la tienda o menús de herrería. Su objetivo primordial es mostrarle al usuario de forma intuitiva y 
   colorida el impacto exacto que tendrá una mejora o alteración en los atributos de una unidad (salud, daño, rango) 
   antes de confirmar la transacción.

   Características clave:
   1. Sistema de Doble Relleno de Imagen (Fill Amount): Manipula de forma síncrona dos componentes 'Image' de tipo 
      'Filled'. Al desfasar y superponer los valores de relleno de forma inteligente, el sistema puede representar 
      tanto incrementos (barras verdes sobresalientes) como reducciones (bloques de peligro rojos) en un único gráfico.
   2. Seguridad de Punto Flotante: Utiliza funciones de comparación épsilon de Unity ('Mathf.Approximately') para blindar 
      la UI ante fallos de redondeo gráfico, asegurando que los segmentos de cambio se apaguen perfectamente cuando los 
      valores actuales y futuros sean lógicamente idénticos.
   3. Optimización de Memoria y Estado Vacío: Cuenta con un método 'Clear()' integrado que limpia las cadenas de caracteres 
      y desactiva los pesos de renderizado ('fillAmount = 0f'), permitiendo que el contenedor sea reciclado de forma limpia 
      por los gestores de la UI al deseleccionar las estructuras del mapa.
   ========================================================================================================
*/