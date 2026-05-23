using System.Collections.Generic;
using UnityEngine;



// LÍNEA RARA / COMPLEJA: Serialización Estricta de Clases Contenedoras de Datos Económicos Tácticos ([System.Serializable]).
// Al anteponer este atributo, se le instruye al compilador de C# y al pipeline de Unity que traduzcan los campos lógicos 
// de esta clase a un formato binario legible por el Inspector. Esto permite que la clase actúe como una estructura de datos 
// compuesta o tupla personalizada, haciendo posible que aparezca y sea editada directamente dentro de arrays o listas dinámicas 
// desde la interfaz gráfica del motor sin necesidad de heredar de 'MonoBehaviour'.
[System.Serializable]
public class WillProduction
{
    public Will type;
    public float amount;
}


// LÍNEA RARA / COMPLEJA: Atributo de Inyección Procedural en el Menú de Creación de Activos de Datos ([CreateAssetMenu]).
// Declara un punto de entrada en el subsistema de la factoría del editor nativo de Unity. Al definir 'menuName = "Construction/Data"', 
// expone de forma directa una directiva de comandos en el menú contextual de "Click Derecho -> Create", permitiendo a los diseñadores 
// de niveles instanciar archivos contenedores independientes físicos (.asset) en el disco duro basados en esta plantilla de variables.
[CreateAssetMenu(menuName = "Construction/Data")]
public class ConstructionData : ScriptableObject
{
    public ConstructionType type;
    public GodType god;

    public int level;

    public float resistance;
    public float aditamentResistance;
    public float attackDamage;

    public List<WillProduction> willObtaied;
    public float actionVelocity;

    public float movementSpeed;
    public float range;
    public float proyectileVelocity;

    public List<WillProduction> willToPay;
    public List<WillProduction> bonusWillToPay;

    public GameObject prefab;
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el **Contenedor Inmutable de Arquitectura de Datos de Estructuras** (`ConstructionData`). 
   Al heredar de la clase especializada de Unity `ScriptableObject`, no se comporta como un componente que se cuelga 
   de un objeto físico en el escenario (`Transform`), sino como un contenedor de metadatos puro y aislado que reside 
   únicamente en los archivos de recursos del proyecto (`Assets`). Sirve como una "plantilla de configuración central" 
   para alimentar dinámicamente a scripts como `BaseConstruction`, `Barracks`, y `Arrow`.

   Características arquitectónicas clave:
   1. Memoria Compartida y Optimización de Peso: Al utilizar un `ScriptableObject`, si tienes 150 muros idénticos en el tablero 
      de juego, todos ellos apuntarán a un único archivo físico de `ConstructionData`. Esto elimina la redundancia de datos en 
      memoria RAM, evitando que cada instancia duplique variables idénticas de salud, rango o daño.
   2. Arquitectura de Datos Desacoplada: Separa de manera absoluta la configuración teórica de balance (valores numéricos de 
      diseño, costos, velocidades) de la representación física y gráfica del juego en la escena (`GameObject prefab`). Esto permite 
      cambiar las propiedades o el modelo visual de una torre en segundos sin necesidad de editar código fuente o alterar los niveles.
   3. Estructura de Costos y Producción Dinámica: Mediante el uso de la clase auxiliar serializada `WillProduction`, el asset dota 
      al sistema económico de una enorme flexibilidad. Permite pre-configurar de forma limpia listas complejas de divisas tanto 
      para la recompensa por impacto (`willObtaied`) como para los costos de construcción base (`willToPay`) y penalizaciones o bonus.
   ========================================================================================================
*/