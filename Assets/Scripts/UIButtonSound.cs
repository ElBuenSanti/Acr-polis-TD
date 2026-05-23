using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButtonSound : MonoBehaviour, ISelectHandler, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private UISoundPlayer uiSoundPlayer;

    // Cachea de forma automatizada la referencia del reproductor de sonido buscando el componente en la escena activa
    private void Awake()
    {
        if (uiSoundPlayer == null)
            uiSoundPlayer = FindAnyObjectByType<UISoundPlayer>();
    }

    // LÍNEA RARA / COMPLEJA: Intercepción del Evento de Selección del Sistema de Navegación por Mando o Teclado ('OnSelect').
    // Invocado automáticamente por el 'EventSystem' cuando el elemento recibe el foco de navegación (Highlight) mediante las flechas del teclado, 
    // las palancas (D-Pad/Joystick) de un mando o navegación por tabulación. Al verificar de forma segura que 'uiSoundPlayer' sea válido, 
    // despacha de forma asertiva la reproducción del efecto 'PlayHover()', garantizando accesibilidad de audio idéntica para periféricos 
    // físicos sin depender del cursor del ratón.
    public void OnSelect(BaseEventData eventData)
    {
        if (uiSoundPlayer != null)
            uiSoundPlayer.PlayHover();
    }

    // LÍNEA RARA / COMPLEJA: Intercepción del Evento de Entrada del Cursor Espacial sobre el Área del Colisionador UI ('OnPointerEnter').
    // Mensaje nativo gatillado por el módulo de entrada de Unity en el instante exacto en que la proyección geométrica del cursor del ratón 
    // o un punto de contacto táctil ingresa en las fronteras de la bounding-box del componente visual. Al igual que con el foco analógico, 
    // delega la orden síncrona al motor de audio central ejecuntando 'PlayHover()', proveyendo retroalimentación sensorial inmediata al usuario.
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (uiSoundPlayer != null)
            uiSoundPlayer.PlayHover();
    }

    // Captura el clic físico o pulsación táctil liberando la orden de disparo acústico de confirmación al sistema reproductor de sonido
    public void OnPointerClick(PointerEventData eventData)
    {
        if (uiSoundPlayer != null)
            uiSoundPlayer.PlayConfirm();
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el **Disparador Polimórfico de Efectos Acústicos para Botones de Interfaz** (`UIButtonSound`). 
   Su responsabilidad dentro de la arquitectura de UI es abstraer e interceptar de forma genérica las interacciones de bajo nivel del usuario 
   para canalizar de manera centralizada la reproducción de audio táctico, vinculando la capa del sistema de eventos (`EventSystems`) 
   con un reproductor de sonido persistente desacoplado (`UISoundPlayer`).

   Características arquitectónicas clave:
   1. Soporte Multiperiférico Híbrido (Cross-Platform Input Alignment): Al implementar tanto `ISelectHandler` como las interfaces 
      de puntero, el software unifica de forma transparente el comportamiento acústico del juego. Funciona a la perfección tanto en 
      computadoras que utilicen un ratón clásico, en pantallas móviles táctiles, como en consolas donde el jugador navega por el Canvas 
      saltando entre botones mediante el foco direccional de un control gamepad.
   2. Desacoplamiento y Dependencia Inversa (Service Locator Pattern Fallback): La clase no arrastra una ruta dura hacia el audio en disco 
      ni expone archivos `.wav` de forma local. En su lugar, consume las APIs globales expuestas por `UISoundPlayer`. Al usar `FindAnyObjectByType` 
      en `Awake`, mitiga errores humanos de diseño, inyectando la referencia automáticamente en tiempo de ejecución si el desarrollador 
      olvidó arrastrar el objeto en el editor.
   3. Optimización de Llamadas de Mensajes sobre el Motor: Al depender directamente de interfaces puras en lugar de encuestar cíclicamente 
      el estado de los periféricos en bucles costosos (`Update`), el componente permanece inerte en memoria en un estado latente. Solo 
      consume ciclos de procesamiento en la CPU cuando el `EventSystem` de Unity le inyecta las estructuras de datos de interacción de forma 
      reactiva por interrupciones.
   ========================================================================================================
*/