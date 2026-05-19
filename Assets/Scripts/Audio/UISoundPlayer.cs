using UnityEngine;

public class UISoundPlayer : MonoBehaviour
{
    [Header("UI Clips")]
    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private AudioClip confirmClip;
    [SerializeField] private AudioClip cancelClip;
    [SerializeField] private AudioClip openPanelClip;
    [SerializeField] private AudioClip closePanelClip;
    [SerializeField] private AudioClip errorClip;

    // Reproduce el sonido cuando el cursor pasa por encima de un botón (Hover)
    public void PlayHover()
    {
        Play(hoverClip);
    }

    // Reproduce el sonido de confirmación o clic positivo
    public void PlayConfirm()
    {
        Play(confirmClip);
    }

    // Reproduce el sonido de cancelación, retroceso o cierre
    public void PlayCancel()
    {
        Play(cancelClip);
    }

    // Reproduce el sonido de transición al abrir un menú o panel en pantalla
    public void PlayOpenPanel()
    {
        Play(openPanelClip);
    }

    // Reproduce el sonido de transición al ocultar o cerrar un menú o panel
    public void PlayClosePanel()
    {
        Play(closePanelClip);
    }

    // Reproduce un sonido de advertencia, error o acción no permitida en la interfaz
    public void PlayError()
    {
        Play(errorClip);
    }

    // Método privado interno que centraliza y redirige el clip hacia el sistema de audio principal
    private void Play(AudioClip clip)
    {
        // LÍNEA RARA / COMPLEJA: Se comunica con el Singleton del AudioManager. 
        // El condicional '!= null' es una validación de seguridad fundamental. Si por alguna razón el AudioManager 
        // no se encuentra cargado en la escena actual, evita que el juego se congele o arroje un error crítico de tipo "NullReferenceException".
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayUI(clip); // Envía el audio específicamente al canal asignado para la interfaz de usuario (UI)
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como una "Librería o Fachada de Sonidos para Interfaces" (UISoundPlayer). Su única y 
   principal función es centralizar e identificar de manera lógica todos los efectos de sonido relacionados 
   con la navegación de los menús del juego.

   Características clave:
   1. Organización en el Inspector: Agrupa en una sola sección visual ('[Header]') todos los recursos de audio 
      cortos vinculados a las acciones del usuario en las pantallas de opciones, inventarios, tiendas o menús.
   2. Modularidad y Desacoplamiento: Evita que los botones o los controladores de la UI tengan que buscar archivos 
      de audio específicos en las carpetas del proyecto. En su lugar, los componentes de la interfaz de Unity (como los 
      eventos 'OnClick' de los botones) simplemente mandan a llamar a métodos claros como 'PlayConfirm()' o 'PlayHover()'.
   3. Canalización Correcta: No reproduce el sonido por cuenta propia; se asegura de delegarle el trabajo al 
      'AudioManager' global a través del método 'PlayUI'. Esto garantiza que los sonidos de los menús se escuchen 
      en su respectivo canal mezclador y respeten la configuración de volumen que el jugador elija.
   ========================================================================================================
*/