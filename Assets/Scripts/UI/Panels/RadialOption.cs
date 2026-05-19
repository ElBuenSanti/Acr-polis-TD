public enum RadialOption
{
    None,
    Hephaestus,
    Aphrodite,
    Ares,
    Sell
}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este archivo define el Enumerado de Opciones del Menú Radial (RadialOption). Es una estructura de datos de tipo 
   valor ('Value Type') que sirve como un diccionario de constantes fuertemente tipadas, utilizada por el sistema 
   de interfaz de usuario y los controladores tácticos para identificar de forma unívoca qué sección o comando ha 
   sido apuntado por el jugador en el HUD.

   Características clave:
   1. Indexación Entera Subyacente: En C#, los enumerados asignan por defecto un valor entero secuencial a cada 
      elemento comenzando desde cero si no se especifica lo contrario. De este modo, en memoria el compilador traduce:
      - None = 0
      - Hephaestus = 1
      - Aphrodite = 2
      - Ares = 3
      - Sell = 4
   2. Seguridad de Tipos (Type Safety): Evita el uso de cadenas de texto crudas ("strings") o números mágicos ("int") 
      para controlar los estados del menú. Esto previene errores de dedo en tiempo de desarrollo y permite que el 
      compilador valide que las estructuras condicionales (como los bloques 'switch') manejen únicamente opciones válidas.
   3. Arquitectura Conectiva Divina: Mapea directamente los caminos de evolución mitológica (Hefesto, Afrodita, Ares) 
      y las acciones utilitarias de demolición (Sell), sirviendo como la bandera de estado primordial que el script 
      'RadialMenuUI' transmite a los gestores económicos y de construcción al confirmar un comando.
   ========================================================================================================
*/