using UnityEngine;

public class ConstructionDatabase : MonoBehaviour
{
    public ConstructionData[] allData;

    // Searches and returns the requested upgrade
    // LÍNEA RARA / COMPLEJA: Algoritmo de Búsqueda Lineal Indexada por Triple Coincidencia de Metadatos ('GetData').
    // Itera secuencialmente mediante un bucle 'foreach' sobre la colección lineal en memoria 'allData' (un arreglo de ScriptableObjects). 
    // Para cada elemento evaluado, ejecuta una compuerta lógica condicional estricta trifásica que compara simultáneamente el tipo de estructura 
    // ('data.type'), la deidad asignada ('data.god') y el escalafón evolutivo exacto de la construcción ('data.level'). En el instante 
    // en que se convalidan las tres variables físicas, rompe el bucle de inmediato devolviendo la referencia en disco del archivo contenedor, 
    // eludiendo procesos de casteo o búsquedas dinámicas costosas en la jerarquía de Unity.
    public ConstructionData GetData(ConstructionType type, GodType god, int level) // te regresa la construcción que necesitas
    {
        foreach (var data in allData)
        {
            if (data.type == type && data.god == god && data.level == level)
                return data;
        }

        return null;
    }
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el **Repositorio Centralizado de Datos e Índice de Mejoras de Estructuras** (`ConstructionDatabase`). 
   Su responsabilidad principal en la arquitectura del software es centralizar y servir como una base de datos plana en memoria 
   que almacena todas las permutaciones de planos de construcción del juego. Es el encargado de responder consultas de otros 
   sistemas globales cuando el jugador intenta edificar o subir de nivel una estructura en la cuadrícula táctica.

   Características arquitectónicas clave:
   1. Centralización de Archivos de Configuración: Almacena en un único arreglo (`allData`) todos los archivos `.asset` 
      de tipo `ConstructionData` del proyecto. Esto evita que los botones de la interfaz, el gestor de construcción 
      o los sistemas de guardado tengan que almacenar cientos de referencias individuales, ordenando el ecosistema de recursos.
   2. Resolvedor Dinámico de Evolución (Upgrades): El método `GetData` funciona como un motor de búsqueda atómico. Al recibir el 
      tipo de edificio, el dios y el nivel objetivo (por ejemplo: `Wall`, `Zeus`, `Level 2`), localiza el gemelo virtual exacto 
      en la base de datos, permitiendo al sistema sustituir de forma limpia los datos antiguos por los nuevos valores de balance.
   3. Arquitectura Robusta contra Fallos (Null Safety): Si se solicita una combinación inexistente o que aún no se ha diseñado 
      en el editor (por ejemplo, una torreta de nivel 50), el script aborta el proceso de forma segura devolviendo un puntero 
      nulo (`return null`). Esto previene que el juego se rompa, facilitando la inserción de alertas de control en las clases llamantes.
   ========================================================================================================
*/