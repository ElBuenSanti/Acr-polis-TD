using UnityEngine;

public class EndGameUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private EndGamePanelUI victoryPanel;
    [SerializeField] private EndGamePanelUI defeatPanel;

    // Método de ciclo de vida de Unity que se ejecuta automáticamente cuando el objeto se activa en la escena
    private void OnEnable()
    {
        // LÍNEA RARA / COMPLEJA: Suscripción a Eventos (Observer Pattern).
        // El operador '+=' significa que este script se queda "escuchando" a que ocurra una acción en otra clase.
        // Cuando la clase 'FinalBoss' avise que el jefe murió, automáticamente se disparará el método 'ShowVictory' de este script.
        // Cuando la clase 'Temple' avise que el templo fue destruido, se disparará el método 'ShowDefeat'.
        FinalBoss.OnFinalBossDeath += ShowVictory;
        Temple.OnTempleDestruction += ShowDefeat;
    }

    // Método de ciclo de vida de Unity que se ejecuta automáticamente cuando el objeto se desactiva o se destruye
    private void OnDisable()
    {
        // LÍNEA RARA / COMPLEJA: Cancelación de Suscripción a Eventos.
        // El operador '-=' es indispensable para "dejar de escuchar" el evento antes de que el objeto desaparezca de la memoria.
        // REGLA DE ORO: Si te suscribes a un evento en 'OnEnable', DEBES desuscribirte en 'OnDisable'. Si lo olvidas, 
        // Unity arrastrará un error grave conocido como "Fuga de Memoria" (Memory Leak) intentando llamar a un objeto que ya no existe.
        FinalBoss.OnFinalBossDeath -= ShowVictory;
        Temple.OnTempleDestruction -= ShowDefeat;
    }

    // Método encargado de gestionar toda la lógica visual y sonora cuando el jugador gana la partida
    private void ShowVictory()
    {
        // Apaga de forma segura la música de fondo y los sonidos ambientales
        if (MusicManager.Instance != null)
            MusicManager.Instance.StopAllMusicAndAmbience();

        // Reproduce el efecto de sonido o fanfarria de victoria
        if (GameplaySoundPlayer.Instance != null)
            GameplaySoundPlayer.Instance.PlayVictory();

        // Activa en pantalla el panel o menú visual que le indica al jugador que ganó
        if (victoryPanel != null)
            victoryPanel.Open();
    }

    // Método encargado de gestionar toda la lógica visual y sonora cuando el jugador pierde la partida
    private void ShowDefeat()
    {
        // Apaga la música de fondo para darle un tono dramático a la derrota
        if (MusicManager.Instance != null)
            MusicManager.Instance.StopAllMusicAndAmbience();

        // Reproduce el efecto de sonido o melodía de derrota
        if (GameplaySoundPlayer.Instance != null)
            GameplaySoundPlayer.Instance.PlayDefeat();

        // Activa en pantalla el panel o menú visual de Game Over
        if (defeatPanel != null)
            defeatPanel.Open();
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Controlador de Fin de Partida (EndGameUIController). Su función principal es 
   detectar el desenlace del juego (Victoria o Derrota) para congelar/limpiar la atmósfera sonora y desplegar 
   las pantallas correspondientes para el usuario.

   Características clave:
   1. Programación Basada en Eventos: En lugar de usar un bucle 'Update' que gaste procesador preguntando a cada 
      milisegundo si el jefe ya murió o si el templo sigue en pie, este script permanece totalmente dormido hasta que 
      los scripts 'FinalBoss' o 'Temple' lanzan un "grito" (evento) notificando el fin del juego. Esto es altamente óptimo.
   2. Orquestación del Desenlace: Funciona como un puente de comunicación inter-modular. Al recibir la alerta de fin de 
      partida, coordina de manera ordenada al 'MusicManager' (para silenciar el mapa), al 'GameplaySoundPlayer' (para 
      dar el impacto sonoro) y a sus propios componentes visuales ('EndGamePanelUI').
   3. Arquitectura Limpia: Separa por completo la lógica de las mecánicas (salud del jefe, daño al templo) de la lógica 
      de presentación e interfaz de usuario, cumpliendo con las buenas prácticas de desarrollo en Unity.
   ========================================================================================================
*/