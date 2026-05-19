using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadialMenuController : MonoBehaviour
{
    public GameObject theMenu;

    public GameObject objRed, objBlue, objGreen, objYellow;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    // Escucha de manera continua las pulsaciones del teclado para abrir el menú o alternar los objetos de colores
    void Update()
    {
        // LÍNEA RARA / COMPLEJA: Captura Discreta de Evento de Entrada por Hardware ('Input.GetKeyDown').
        // Evalúa en el frame exacto en que el usuario presiona la tecla física 'Tab'. A diferencia de 'Input.GetKey' 
        // (que devuelve verdadero de manera continua mientras la tecla permanezca hundida), 'GetKeyDown' solo se dispara 
        // una vez por pulsación, evitando que el lienzo del menú sufra micro-parpadeos o reinicios de estado continuos.
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            theMenu.SetActive(true);
        }



        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchRed();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            SwitchBlue();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            SwitchGreen();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SwitchYellow();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            SwitchAll();
        }
    }

    // LÍNEA RARA / COMPLEJA: Conmutación de Visibilidad de Objeto mediante Negación Lógica Unaria ('SwitchRed').
    // Obtiene el estado de visibilidad real del GameObject en la escena mediante '.activeInHierarchy'. Al aplicar el operador 
    // de negación unaria ('!'), invierte de forma booleana dicho valor (si es verdadero pasa a ser falso y viceversa). 
    // Al inyectar el resultado en '.SetActive()', el script commuta el objeto simulando un interruptor de encendido/apagado 
    // en una sola línea de ejecución limpia, eliminando la necesidad de escribir bloques condicionales 'if/else' redundantes.
    public void SwitchRed()
    {
        Debug.Log("Red");
        objRed.SetActive(!objRed.activeInHierarchy);
    }

    // Invierte el estado de activación actual del GameObject azul en la jerarquía de la escena
    public void SwitchBlue()
    {
        objBlue.SetActive(!objBlue.activeInHierarchy);
    }

    // Invierte el estado de activación actual del GameObject verde en la jerarquía de la escena
    public void SwitchGreen()
    {
        objGreen.SetActive(!objGreen.activeInHierarchy);
    }

    // Invierte el estado de activación actual del GameObject amarillo en la jerarquía de la escena
    public void SwitchYellow()
    {
        objYellow.SetActive(!objYellow.activeInHierarchy);
    }

    // Ejecuta de forma secuencial los métodos de conmutación individuales para alterar simultáneamente todos los GameObjects de la escena
    public void SwitchAll()
    {
        SwitchRed();
        SwitchBlue();
        SwitchGreen();
        SwitchYellow();
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Controlador de Conmutación de Elementos del Menú Radial (RadialMenuController). Es un 
   componente utilitario y de testeo de interfaz de usuario diseñado para capturar eventos directos del teclado del 
   desarrollador o jugador. Su propósito principal es alternar de forma dinámica la visibilidad (activación/desactivación) 
   de un menú principal y una serie de GameObjects representados por colores individuales (Rojo, Azul, Verde, Amarillo).

   Características clave:
   1. Captura Única de Entrada de Hardware: Emplea la API nativa de entrada de Unity (`Input.GetKeyDown`) mapeada a 
      códigos de teclado estándar (`KeyCode`). Esto garantiza un control preciso y un comportamiento determinista del 
      software, respondiendo únicamente ante la pulsación inicial de los botones asignados (`Tab`, `Q`, `W`, `E`, `R`, `T`).
   2. Conmutación Segura mediante Estado de Jerarquía: Al evaluar la propiedad `.activeInHierarchy` en lugar de `.activeSelf`, 
      el script toma en consideración si el objeto está realmente visible en el mundo o si está apagado indirectamente 
      debido a que su objeto padre en la jerarquía de Unity está desactivado, evitando errores de desfasaje de estados.
   3. Modularidad y Comportamiento Cascada: Estructura la alteración de los GameObjects mediante métodos individuales 
      dedicados para cada color. Esto permite reutilizar las funciones de manera limpia y segura en métodos de acción 
      masiva como `SwitchAll()`, facilitando la depuración rápida de elementos gráficos desde el inspector de Unity.
   ========================================================================================================
*/