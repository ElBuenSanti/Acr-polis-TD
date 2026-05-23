using UnityEngine;

public class GodDebugInput : MonoBehaviour
{
    void Update()
    {
        var selected = BuildingManager.Instance.selectedConstruction;

        if (selected == null)
            return;


        // LÍNEA RARA / COMPLEJA: Mapeo de Entrada y Asignación Dinámica de Deidades por Estado de Enfoque ('Update').
        // Evalúa de forma síncrona en cada ciclo de renderizado el estado físico de los periféricos mediante 'Input.GetKeyDown'. 
        // Si detecta la pulsación de las constantes ASCII ('A', 'F' o 'H'), rompe el flujo estático e invoca el método mutador 
        // 'selected.SetSelectedGod' inyectando el enumerado correspondiente ('GodType.Ares', 'Aphrodite' o 'Hephaestus'). Esto altera 
        // de forma instantánea el perfil teológico de la edificación seleccionada actualmente en el 'BuildingManager', preparando sus 
        // metadatos para la posterior bifurcación de habilidades o cambios cosméticos en el juego.
        if (Input.GetKeyDown(KeyCode.A)) // ELEGIR DIOS
        {
            selected.SetSelectedGod(GodType.Ares);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            selected.SetSelectedGod(GodType.Aphrodite);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            selected.SetSelectedGod(GodType.Hephaestus);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            selected.Upgrade(); // &COBNFIRMAR EVOLUCION
        }

        // LÍNEA RARA / COMPLEJA: Extracción Segura de Componentes y Propagación Colectiva de Reparación en Red ('Input.GetKeyDown(KeyCode.R)').
        // Al presionar la tecla 'R', el script intenta resolver dinámicamente si la estructura seleccionada posee la lógica específica 
        // de un segmento de muro mediante 'selected.GetComponent<Wall>()'. Para evitar excepciones de referencia nula, implementa una 
        // compuerta defensiva ('if (wall != null)'). Si la aserción es afirmativa, delega el control al método 'RepairGroup()', desencadenando 
        // un algoritmo interno que propaga el restablecimiento de la salud física a través de todo el grafo de muros conectados en su red.
        if (Input.GetKeyDown(KeyCode.R)) // REPARAR MURALLA
        {
            var wall = selected.GetComponent<Wall>();

            if (wall != null)
            {
                wall.RepairGroup();
            }
        }
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como la **Interfaz de Comandos de Depuración para la Evolución Teológica y Mantenimiento** (`GodDebugInput`). 
   Su principal y única función en el ciclo de desarrollo del software es proveer una botonera rápida de hardware (Dev Hotkeys) 
   para testear las mecánicas avanzadas del juego: permite modificar la deidad patrona de un edificio, forzar su subida de nivel 
   mecánica (`Upgrade`) o ejecutar rutinas de restauración de salud perimetral de forma inmediata sin transitar por menús de UI.

   Características arquitectónicas clave:
   1. Control Centralizado por Selección Activa: El script implementa un filtro preventivo al inicio del `Update` evaluando el puntero 
      `BuildingManager.Instance.selectedConstruction`. Si el usuario no ha hecho clic sobre ningún edificio del mapa, el código aborta 
      de inmediato, eliminando cualquier consumo innecesario de ciclos de procesamiento del teclado.
   2. Transición y Evolución Polimórfica (Upgrades): Al presionar la barra espaciadora, invoca el método general `Upgrade()`. Gracias 
      a la herencia, este comando funcionará de forma transparente tanto si la estructura seleccionada es una torre defensiva (`Defense`), 
      un cuartel (`Barracks`) o una muralla (`Wall`), aplicando los nuevos parámetros de balance de manera abstracta.
   3. Extensibilidad de Tipo mediante Consultas Seguras (Type-Safe Querying): La sección dedicada a la reparación demuestra un diseño 
      robusto. En lugar de asumir que cualquier edificio puede ser reparado en grupo, solicita explícitamente el componente `Wall`. Si la 
      estructura elegida es de otro tipo (como una torre de arqueros), el código ignora la pulsación de forma limpia, evitando errores en consola.
   ========================================================================================================
*/