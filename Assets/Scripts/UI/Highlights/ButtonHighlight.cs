using UnityEngine;
using UnityEngine.EventSystems;

// LÍNEA RARA / COMPLEJA: Herencia de Interfaces del Event System ('ISelectHandler', 'IDeselectHandler').
// Al agregar estas interfaces después de 'MonoBehaviour', obligamos a este script a implementar los métodos 
// 'OnSelect' y 'OnDeselect'. Esto le permite a Unity "avisarle" a este botón exacto cuándo ha sido apuntado o 
// deseleccionado mediante la navegación de un Gamepad (cruceta/joystick) o el teclado, sin depender del mouse.
public class ButtonHighlight : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public RectTransform rect;

    private Vector3 normalScale;
    private Vector3 selectedScale = Vector3.one * 1.1f;

    // Captura el tamaño inicial y las proporciones nativas del botón en la interfaz
    private void Awake()
    {
        normalScale = rect.localScale;
    }

    // Método automático de la interfaz 'ISelectHandler' que se dispara en el instante en que el cursor del mando se posa sobre el botón
    public void OnSelect(BaseEventData eventData)
    {
        // Incrementa el tamaño del botón instantáneamente a un 110% (1.1f) para darle feedback visual claro al jugador
        rect.localScale = selectedScale;
    }

    // Método automático de la interfaz 'IDeselectHandler' que se dispara cuando el jugador mueve el mando hacia otro elemento
    public void OnDeselect(BaseEventData eventData)
    {
        // Restablece el botón a sus dimensiones originales (100%) cuando pierde el enfoque o selección
        rect.localScale = normalScale;
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Retroalimentador de Foco para Botones (ButtonHighlight). Su función principal es 
   modificar la escala física de un botón de la interfaz de usuario en el momento exacto en el que es seleccionado 
   durante la navegación del menú.

   Características clave:
   1. Optimizado para Controles y Teclados: A diferencia de los scripts tradicionales que detectan cuando el puntero 
      del mouse pasa por encima ('IPointerEnterHandler'), este script utiliza el sistema de selección por eventos 
      de Unity, lo cual es indispensable para que los menús funcionen correctamente al usar mandos de consola (Gamepad).
   2. Feedback Visual Instantáneo: Proporciona una respuesta de escalado limpia y directa para que el usuario identifique 
      sin margen de error en qué sección de las opciones, la tienda o el menú de pausa se encuentra ubicado su cursor virtual.
   3. Arquitectura Limpia y Ligera: No requiere de un bucle de actualización continua ('Update'), lo que significa 
      que consume cero recursos de procesamiento mientras el jugador navega. Solo se ejecuta en hilos de respuesta de forma 
      reactiva cuando el 'EventSystem' de Unity despacha activamente una acción de enfoque.
   ========================================================================================================
*/