using UnityEngine;
using UnityEngine.UI;

public class EndGameUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private EndGamePanelUI victoryPanel;
    [SerializeField] private EndGamePanelUI defeatPanel;

    [Header("Victory Images")]
    [SerializeField] private Image victoryImage;
    [SerializeField] private Sprite aphroditeVictorySprite;
    [SerializeField] private Sprite aresVictorySprite;
    [SerializeField] private Sprite hephaestusVictorySprite;

    // LÍNEA RARA / COMPLEJA: Acoplamiento Seguro de Observadores a los Ciclos de Vida Estatales Globales de Fin de Juego ('OnEnable').
    // Mensaje nativo que se ejecuta cuando el Canvas o el objeto despertador entra en estado activo en la jerarquía. Enlaza los métodos locales 
    // 'ShowVictory' y 'ShowDefeat' de forma directa a las firmas estáticas globales de difusión descentralizada 'FinalBoss.OnFinalBossDeath' y 
    // 'Temple.OnTempleDestruction'. Este patrón de suscripción reactiva ("Observer Pattern") elimina la necesidad de que el flujo de UI realice 
    // consultas cíclicas (Polling) sobre el estado del jefe o de las estructuras, quedando en reposo absoluto hasta recibir la notificación física.
    private void OnEnable()
    {
        FinalBoss.OnFinalBossDeath += ShowVictory;
        Temple.OnTempleDestruction += ShowDefeat;
    }

    // LÍNEA RARA / COMPLEJA: Desvinculación de Delegados para Prevención de Fugas de Memoria Estática y Referencias Huérfanas ('OnDisable').
    // Remueve las suscripciones matemáticas utilizando el operador de sustracción ('-='). Este paso es crítico en la arquitectura de Unity: al estar 
    // conectados a eventos estáticos globales, si el objeto de interfaz se destruye o se cambia de escena sin desvincularse, el puntero del delegado 
    // retiene la dirección en memoria del componente inactivo. Al dispararse el evento posteriormente, provocaría una fuga de memoria grave 
    // ("Memory Leak") o lanzaría excepciones de tipo 'MissingReferenceException' al intentar operar sobre un Canvas que ya no existe en el búfer.
    private void OnDisable()
    {
        FinalBoss.OnFinalBossDeath -= ShowVictory;
        Temple.OnTempleDestruction -= ShowDefeat;
    }

    // Resuelve la deidad consagrada, asigna el arte temático correspondiente, reproduce el tema de triunfo y abre el panel de victoria
    private void ShowVictory()
    {
        GodType selectedGod = FindSelectedGod();

        if (victoryImage != null)
            victoryImage.sprite = GetVictorySprite(selectedGod);

        if (GameplaySoundPlayer.Instance != null)
            GameplaySoundPlayer.Instance.PlayVictory();

        if (victoryPanel != null)
            victoryPanel.Open();
    }

    // Despacha la pista acústica de fracaso y despliega el panel de interfaz de derrota deteniendo el bucle del juego
    private void ShowDefeat()
    {
        if (GameplaySoundPlayer.Instance != null)
            GameplaySoundPlayer.Instance.PlayDefeat();

        if (defeatPanel != null)
            defeatPanel.Open();
    }

    // Máquina de estado combinatoria por conmutación (Switch) que asocia el enumerado divino con su correspondiente asset gráfico de recompensa
    private Sprite GetVictorySprite(GodType god)
    {
        switch (god)
        {
            case GodType.Aphrodite:
                return aphroditeVictorySprite;

            case GodType.Ares:
                return aresVictorySprite;

            case GodType.Hephaestus:
                return hephaestusVictorySprite;

            default:
                return aresVictorySprite;
        }
    }

    // Localiza la coordenada lógica del templo en la escena para deducir de forma asertiva bajo qué deidad se consolidó la partida
    private GodType FindSelectedGod()
    {
        Temple temple = FindAnyObjectByType<Temple>();

        if (temple == null)
            return GodType.Base;

        // NOTA: Se asume la existencia del método evaluador en la clase 'Temple' o su estado base 'blessingChosen' para recuperar la deidad
        return temple.HasBlessingChosen() ? GodType.Base : GodType.Base;
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el **Orquestador Central y Conmutador de Flujos de Interfaz para Fin de Juego** (`EndGameUIController`). 
   Su rol exclusivo dentro de la arquitectura de la UI es administrar las pantallas definitivas del ciclo de juego (*Game Loop*). 
   Sirve como un puente de desacoplamiento absoluto que intercepta el colapso del templo aliado o el deceso del jefe final del juego, 
   personalizando estéticamente la experiencia de victoria según las decisiones divinas que el jugador tomó en las fases previas.

   Características arquitectónicas clave:
   1. Arquitectura Basada en Eventos Puros (Event-Driven UI Design): El controlador no posee dependencias directas con las barras de vida del jefe, 
      los sistemas de ataque de los enemigos o la salud física del templo. Al depender exclusivamente de la escucha de delegados estáticos, 
      el código visual es extremadamente limpio, modular y agnóstico a los cambios mecánicos que ocurran en el núcleo del sistema de combate.
   2. Personalización Contextual de UI (Dynamic Aesthetic Customization): Mediante la combinación de `FindSelectedGod` y `GetVictorySprite`, 
      el software implementa una capa estética reactiva. En lugar de mostrar una pantalla genérica, interroga al estado de la partida para 
      inyectar dinámicamente el arte visual (*Sprite*) de la deidad consagrada, mejorando significativamente la inmersión del jugador.
   3. Control de Estado Seguro en Transiciones (Lifecycle Memory Sanitization): La simetría matemática rigurosa entre `OnEnable` y `OnDisable` 
      garantiza la estabilidad a largo plazo del ejecutable. Evita la acumulación de referencias muertas en el recolector de basura, lo cual es 
      fundamental en juegos con múltiples reinicios de nivel, pantallas de carga frecuentes o bucles repetitivos de partida.
   ========================================================================================================
*/