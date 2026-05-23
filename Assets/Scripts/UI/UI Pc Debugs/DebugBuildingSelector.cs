using UnityEngine;
using UnityEngine.InputSystem;

public class DebugBuildingSelector : MonoBehaviour
{
    // LÍNEA RARA / COMPLEJA: Inicialización Arbitraria del Estado de Selección en el Gestor Arquitectónico ('Start').
    // Fuerza al arrancar la escena que el índice de prefabricado activo del 'BuildingManager' se configure por defecto en la posición cero. 
    // Al invocar el patrón de diseño Singleton ('Instance'), inyecta este entero en la máquina de estados del constructor, garantizando 
    // que el puntero de colocación inicie con una estructura válida (en este caso, la Barraca) sin requerir interacción previa del usuario.
    void Start()
    {
        BuildingManager.Instance.SetConstructionIndex(0); //
    }

    // LÍNEA RARA / COMPLEJA: Captura Híbrida de Eventos de Hardware por Polling Polimórfico de Entrada ('Update').
    // Ejecuta un muestreo síncrono por frame dividiendo las peticiones en dos arquitecturas distintas. Primero utiliza la API clásica 
    // 'Input.GetKeyDown' vinculada a códigos numéricos ASCII estáticos ('Alpha0' al 'Alpha3') para conmutar los planos de construcción. 
    // Acto seguido, transiciona al ecosistema moderno mediante 'Keyboard.current.mKey.wasPressedThisFrame'. Esta directiva del nuevo 
    // Input System interrumpe el flujo si la tecla física 'M' fue presionada exactamente en el ciclo de renderizado actual, 
    // disparando la transición lógica del gestor de construcción hacia el modo de recolocación o mudanza dinámica ('TryEnterMoveMode').
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
            BuildingManager.Instance.SetConstructionIndex(0); // BARRACA

        if (Input.GetKeyDown(KeyCode.Alpha1))
            BuildingManager.Instance.SetConstructionIndex(1); // DEFENSA

        if (Input.GetKeyDown(KeyCode.Alpha2))
            BuildingManager.Instance.SetConstructionIndex(2); // PLAZA

        if (Input.GetKeyDown(KeyCode.Alpha3))
            BuildingManager.Instance.SetConstructionIndex(3); // MURALLA

        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            BuildingManager.Instance.TryEnterMoveMode(); // MOVER
        }
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como una **Herramienta e Interfaz de Control para Depuración de Construcciones** (`DebugBuildingSelector`). 
   Su única y exclusiva responsabilidad en el ciclo del software es servir como un puente de pruebas para los desarrolladores y 
   diseñadores, permitiendo alterar instantáneamente el plano arquitectónico activo o activar modos especiales de interacción 
   mediante atajos directos del teclado físico, eludiendo la necesidad de interactuar con la interfaz gráfica de usuario (UI).

   Características arquitectónicas clave:
   1. Interfaz Inyectora de Comandos de Depuración (Cheats / Dev Shortcuts): Permite testear el ciclo completo de emplazamiento, 
      actualización y coste de los cuatro tipos principales de edificaciones (`Barraca`, `Defensa`, `Plaza`, `Muralla`) con solo presionar 
      los números superiores del teclado, acelerando radicalmente las fases de control de calidad y balance numérico.
   2. Convivencia Híbrida de Sistemas de Input: El script ilustra un caso particular de transición de software donde coexisten 
      el sistema de entradas tradicional (`UnityEngine.Input`) y el nuevo paquete modular de Unity (`UnityEngine.InputSystem`). 
      Ambos conviven de manera segura dentro del bucle de actualización constante sin generar conflictos de hilos o excepciones.
   3. Manipulación Remota Desacoplada (Singleton Access): No posee referencias directas a datos físicos ni instancía objetos por sí mismo; 
      utiliza el acceso estático global del `BuildingManager.Instance`. Esto mantiene al selector como un componente complementario 
      e independiente que puede ser desactivado o removido del proyecto final de lanzamiento (Release Build) sin romper ninguna mecánica core.
   ========================================================================================================
*/