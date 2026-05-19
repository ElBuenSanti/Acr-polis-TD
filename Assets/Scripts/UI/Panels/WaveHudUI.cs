using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveHUDUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image waveFillImage;
    [SerializeField] private TextMeshProUGUI waveText;
    [SerializeField] private TextMeshProUGUI milestoneText;

    [Header("Colors")]
    [SerializeField] private Color normalColor = Color.green;
    [SerializeField] private Color milestoneColor = Color.yellow;
    [SerializeField] private Color finalWaveColor = Color.red;

    private int currentWave;
    private int totalWaves;
    private bool isSubscribed;

    // LÍNEA RARA / COMPLEJA: Inicialización Asíncrona con Bloqueo de Consecución Condicional ('Start' como IEnumerator).
    // En Unity, si transformas el método 'Start' en un 'IEnumerator', el motor lo procesa nativamente como una Corrutina. 
    // Al ejecutar 'yield return new WaitUntil(...)', suspendemos el hilo de ejecución frame a frame sin congelar el juego, 
    // esperando a que el Singleton 'WaveSpawner.Instance' sea instanciado en memoria. Esto previene de raíz errores fatales 
    // de referencia nula ('NullReferenceException') cuando la UI y los scripts de lógica se despiertan en un orden impredecible.
    private IEnumerator Start()
    {
        yield return new WaitUntil(() => WaveSpawner.Instance != null);

        Subscribe();

        currentWave = WaveSpawner.Instance.GetCurrentWaveNumber();
        totalWaves = WaveSpawner.Instance.GetTotalWaves();

        SetConstructionPhase();
    }

    // Libera las referencias a los eventos al destruirse el objeto para evitar fugas de memoria por recolección de basura
    private void OnDestroy()
    {
        Unsubscribe();
    }

    // LÍNEA RARA / COMPLEJA: Vinculación de Escucha de Eventos con Bandera de Control ('Subscribe').
    // Se suscribe a los delegados ('OnWaveIndexChanged', 'OnWaveStarted', 'OnWaveEnded') expuestos por el gestor de oleadas. 
    // Utiliza un booleano de control ('isSubscribed') como interruptor de seguridad. Esto evita la doble suscripción accidental, 
    // un bug crítico donde un método se dispara múltiples veces ante un mismo evento, duplicando lógica visual o corrompiendo la UI.
    private void Subscribe()
    {
        if (isSubscribed)
            return;

        WaveSpawner.Instance.OnWaveIndexChanged += UpdateWaveIndex;
        WaveSpawner.Instance.OnWaveStarted += OnWaveStarted;
        WaveSpawner.Instance.OnWaveEnded += OnWaveEnded;

        isSubscribed = true;
    }

    // LÍNEA RARA / COMPLEJA: Desvinculación de Delegados con Resguardo Anti-Huérfanos ('Unsubscribe').
    // Desconecta los métodos observadores del sujeto emisor ('WaveSpawner.Instance'). Incluye una doble comprobación: que esté 
    // suscrito y que el Spawner no haya sido destruido previamente al cambiar de escena. Si no desvinculáramos los métodos 
    // con el operador '-=', la UI destruida dejaría una "referencia fantasma" en el delegado, provocando excepciones cuando el Spawner 
    // intente notificar un cambio a un objeto que ya no existe en la jerarquía.
    private void Unsubscribe()
    {
        if (!isSubscribed || WaveSpawner.Instance == null)
            return;

        WaveSpawner.Instance.OnWaveIndexChanged -= UpdateWaveIndex;
        WaveSpawner.Instance.OnWaveStarted -= OnWaveStarted;
        WaveSpawner.Instance.OnWaveEnded -= OnWaveEnded;

        isSubscribed = false;
    }

    // Calcula de manera matemática la proporción flotante del progreso de la campaña y actualiza el factor de llenado de la imagen
    private void UpdateCampaignProgress()
    {
        if (waveFillImage == null || totalWaves <= 0)
            return;

        float progress = (float)currentWave / totalWaves;
        waveFillImage.fillAmount = Mathf.Clamp01(progress);
    }

    // Sincroniza las variables locales de conteo de hordas, refresca los títulos y recalcula los hitos del mapa
    private void UpdateWaveIndex(int wave, int total)
    {
        currentWave = wave;
        totalWaves = total;

        if (waveText != null)
            waveText.text = GetWaveTitle(currentWave);

        UpdateCampaignProgress();
        UpdateMilestoneVisual();
    }

    // Activa el contenedor tipográfico de la oleada actual e invoca el redibujado de barras al dar inicio al combate
    private void OnWaveStarted()
    {
        currentWave = WaveSpawner.Instance.GetCurrentWaveNumber();
        totalWaves = WaveSpawner.Instance.GetTotalWaves();

        if (waveText != null)
        {
            waveText.gameObject.SetActive(true);
            waveText.text = GetWaveTitle(currentWave);
        }

        UpdateCampaignProgress();
        UpdateMilestoneVisual();
    }

    // Modifica los textos del HUD para denotar que el peligro inmediato ha cesado y es seguro volver a edificar
    private void OnWaveEnded()
    {
        SetConstructionPhase();
    }

    // Oculta el rótulo de oleada, tiñe la barra al color por defecto y anuncia la tregua de construcción en el lienzo
    private void SetConstructionPhase()
    {
        UpdateCampaignProgress();

        if (waveText != null)
            waveText.gameObject.SetActive(false);

        if (milestoneText != null)
            milestoneText.text = "Fase de construcción";

        if (waveFillImage != null && currentWave != totalWaves)
            waveFillImage.color = normalColor;
    }

    // Evalúa el índice actual mediante una estructura condicional jerárquica para mutar los colores e hitos narrativos del HUD
    private void UpdateMilestoneVisual()
    {
        if (waveFillImage == null || milestoneText == null)
            return;

        if (currentWave == totalWaves)
        {
            waveFillImage.color = finalWaveColor;
            milestoneText.text = "BATALLA FINAL";
        }
        else if (currentWave == 15)
        {
            waveFillImage.color = milestoneColor;
            milestoneText.text = "Presagio final";
        }
        else if (currentWave == 10)
        {
            waveFillImage.color = milestoneColor;
            milestoneText.text = "Ira divina";
        }
        else if (currentWave == 5)
        {
            waveFillImage.color = milestoneColor;
            milestoneText.text = "Primer asalto";
        }
        else
        {
            waveFillImage.color = normalColor;
            milestoneText.text = "Defiende la Acrópolis";
        }
    }

    // Formatea y retorna la cadena de caracteres específica que se imprimirá en los encabezados principales de la interfaz
    private string GetWaveTitle(int wave)
    {
        if (wave == totalWaves)
            return "OLEADA FINAL";

        if (wave == 15)
            return "OLEADA XV · PRESAGIO FINAL";

        if (wave == 10)
            return "OLEADA X · IRA DIVINA";

        if (wave == 5)
            return "OLEADA V · PRIMER ASALTO";

        return "OLEADA " + wave;
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Visualizador del Progreso de Oleadas en el HUD (WaveHUDUI). Es el encargado de procesar 
   y mostrar en tiempo real la información sobre la oleada actual del nivel, la fase del juego (fase de construcción 
   vs fase de combate), el porcentaje de avance general de la campaña mediante una barra de relleno (`Image.fillAmount`) 
   y los hitos o "milestones" narrativos especiales (como las oleadas clave de jefes o eventos mitológicos).

   Características clave:
   1. Arquitectura Orientada a Eventos (Observer Pattern): En lugar de interrogar de forma pesada e ineficiente al Spawner 
      en un método 'Update' frame a frame, este componente permanece dormido y solo reacciona de forma reactiva y limpia 
      cuando el núcleo del juego dispara notificaciones mediante delegados de C# (`+=`).
   2. Inicialización Segura y Desacoplada: Al heredar e implementar 'Start' como un enumerador asíncronos mediante `WaitUntil`, 
      se elimina la dependencia rígida en el orden de carga de los Scripts de Unity en la escena, garantizando la estabilidad 
      del HUD incluso en cargas pesadas de datos.
   3. Control Dinámico de Estética por Hitos: Centraliza los cambios de diseño estético del HUD, alterando dinámicamente 
      las cadenas de texto de `TextMeshProUGUI` y los esquemas cromáticos de la barra según el peligro de la horda (verde para 
      fases normales, amarillo para hitos intermedios y rojo para la batalla final por la Acrópolis).
   ========================================================================================================
*/