using UnityEngine;

public class GridSelectorVisual : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridSelector gridSelector;
    [SerializeField] private Transform visualObject;

    [Header("Settings")]
    [SerializeField] private float yOffset = 0.15f;

    // Inicializa la conexión lógica buscando el selector del tablero si no fue arrastrado en el Inspector
    private void Awake()
    {
        if (gridSelector == null)
            gridSelector = FindAnyObjectByType<GridSelector>();
    }

    // LÍNEA RARA / COMPLEJA: Uso del ciclo de vida 'LateUpdate' en lugar del 'Update' tradicional.
    // 'LateUpdate' se ejecuta de forma automática una vez por frame, pero estrictamente DESPUÉS de que todos los 
    // métodos 'Update' convencionales de la escena hayan terminado de procesar sus cálculos.
    // REGLA DE ORO: Se utiliza para suavizar elementos visuales o cámaras que siguen a otros objetos móviles. 
    // Al procesar aquí la posición, garantizamos que el 'GridSelector' ya calculó a qué casilla moverse en su propio 'Update', 
    // evitando un molesto efecto de parpadeo, desfase visual o "jittering" en la pantalla.
    private void LateUpdate()
    {
        // Validación preventiva múltiple para asegurar que ningún componente crítico falte en la memoria activa
        if (gridSelector == null || gridSelector.currentTile == null || visualObject == null)
            return;

        // Extrae la coordenada de posición tridimensional de la casilla actual seleccionada en el mapa
        Vector3 targetPosition = gridSelector.currentTile.transform.position;

        // LÍNEA RARA / COMPLEJA: Desfasamiento de Altura (Offset).
        // Suma un valor flotante directamente al eje 'Y' (vertical). Esto se hace para que el gráfico del selector o retícula 
        // no quede exactamente a la misma altura geométrica del suelo, lo cual provocaría un error de renderizado conocido como 
        // "Z-Fighting" (donde el motor gráfico se confunde y hace que las texturas del suelo y del indicador parpadeen entre sí).
        targetPosition.y += yOffset;

        // Teletransporta el objeto visual (malla, indicador o flecha) a la posición final calculada sobre la cuadrícula
        visualObject.position = targetPosition;
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Representador Estético o Espejo Visual del Cursor (GridSelectorVisual). Su única y 
   principal función es acoplar un objeto gráfico en 3D (un modelo, un aro luminoso o un marco de selección) a las 
   coordenadas lógicas calculadas por el script de posicionamiento del selector.

   Características clave:
   1. Separación Estricta de Datos y Presentación: Cumple de forma excelente con el principio de responsabilidad única. 
      El script anterior ('GridSelector') contiene toda la matemática abstracta y el álgebra lineal de los joysticks, 
      mientras que este script se limita de forma exclusiva a actualizar los gráficos tridimensionales del puntero en pantalla.
   2. Renderizado Sincronizado mediante Orden Jerárquico: Al delegar el posicionamiento al método de ciclo de vida 
      'LateUpdate', el motor gráfico de Unity asegura una fluidez óptima y elimina los tirones o micro-retrasos de imagen, 
      puesto que la estética visual se ajusta inmediatamente después de que la lógica de casillas toma una decisión.
   3. Prevención de Artefactos Gráficos (Z-Fighting): Modula mediante un desfasamiento ajustable en el inspector ('yOffset') 
      la altura relativa del cursor respecto a las casillas del tablero, garantizando que el marcador visual flote de forma 
      limpia y nítida sobre cualquier relieve o estructura del mapa de juego.
   ========================================================================================================
*/