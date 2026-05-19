using UnityEngine;
using UnityEngine.InputSystem.XR;

public class ConstructionController : MonoBehaviour
{
    public ConstructionDatabase database;
    private Pooling pooling;
    public ConstructionGroup group;

    private ConstructionType type;
    private GodType selectedGod = GodType.Base;
    private int level;
    private int nextLevel;

    private bool blessingChosen;

    // Cachea los componentes de la base de datos de estructuras y el pooler para la mutación asíncrona de prefabs
    void Awake()
    {
        pooling = FindAnyObjectByType<Pooling>();
        database = FindAnyObjectByType<ConstructionDatabase>();
    }

    // Configura las propiedades de tipología, nivel e inyecta la referencia de este script al grupo de red si existe
    public void Initialize(ConstructionData data) //, Transform tileTransform
    {
        type = data.type;
        level = data.level;
        selectedGod = data.god;

        if (group != null)
        {
            group.Add(this);
        }
    }

    // LÍNEA RARA / COMPLEJA: Captura de Puntero por Motor de Físicas y Validación de Restricciones del Estado del Juego ('OnMouseDown').
    // Este método es una función callback nativa de Unity que se dispara cuando un rayo físico lanzado desde la cámara 
    // (Raycast) impacta contra el colisionador ('Collider') de este objeto. El script evalúa dos estados excluyentes: 
    // si el bucle de la horda no se está ejecutando en el frame actual ('!WaveSpawner.Instance.IsWaveRunning()') O si la 
    // estructura es un muro perimetral. Solo si se cumple alguna, reenvía la instancia al gestor central para su selección.
    void OnMouseDown()
    {
        if (!WaveSpawner.Instance.IsWaveRunning() || type == ConstructionType.Wall)
        {
            BuildingManager.Instance.Select(this);
        }
    }

    // Bloquea la alteración del camino mitológico si la estructura ya ha superado el umbral del nivel base
    public void SetSelectedGod(GodType god)
    {
        if (level > 1)
        {
            Debug.Log("Ya estas recorriendo el camino divino del Dios " + selectedGod);
            return;
        }

        selectedGod = god;
    }


    // Verifica la deidad asignada, debita los recursos de actualización y coordina si la mutación es individual o colectiva
    public void Upgrade()
    {
        if (selectedGod == GodType.Base)
        {
            Debug.Log("No se ha eleigo un camino divino");
            return;
        }

        nextLevel = level + 1; //int antes

        Debug.Log("Se ha evolucionado por el Dios: " + selectedGod);

        ConstructionData newData = database.GetData(type, selectedGod, nextLevel);

        if (newData == null)
        {
            Debug.Log("No hay más evoluciones con el Dios " + selectedGod);
            return;
        }

        foreach (var w in newData.willToPay)
        {
            if (!WillManager.Instance.SpendMoney(w.type, w.amount))
            {
                Debug.Log("Te falta " + w.type + " para mejorar");
                StatusMessageUI.Instance.ShowMessage("Te falta " + w.type + " para mejorar");
                return;
            }

        }

        if (group != null && group.members.Count > 0)
        {
            foreach (var member in group.members)
            {
                if (member != null)
                    member.UpgradeSingle(newData);
            }
        }
        else
        {
            UpgradeSingle(newData);
        }

    }

    // LÍNEA RARA / COMPLEJA: Transmutación Dinámica de Entidades con Traspaso de Punteros de Red y Pooling ('UpgradeSingle').
    // Cachea la matriz de transformación física y recupera la baldosa lógica asociada. Envía el objeto actual de vuelta al 
    // pool desactivándolo, e instancia el nuevo prefab evolucionado usando el optimizador de memoria 'pooling.CreateObject'. 
    // Si la estructura pertenece a un conjunto compuesto ('group != null'), realiza un intercambio crítico de referencias de software: 
    // remueve la instancia obsoleta ('this') e inserta el nuevo componente recién creado, manteniendo la integridad del grupo.
    void UpgradeSingle(ConstructionData newData)
    {
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;

        Tile currentTile = GetComponent<BaseConstruction>().GetTile();

        gameObject.SetActive(false);

        GameObject newBuilding = pooling.CreateObject(newData.prefab, currentTile.transform);

        newBuilding.transform.SetPositionAndRotation(position, rotation);

        var newConstruction = newBuilding.GetComponent<BaseConstruction>();

        var newController = newBuilding.GetComponent<ConstructionController>();

        if (group != null)
        {
            group.members.Remove(this);

            newController.group = group;

            group.Add(newController);
        }

        newConstruction.SetTile(currentTile);

        newConstruction.Initialize(newData);

        newController.Initialize(newData); //, currentTile.transform

        if (blessingChosen)
            newController.MarkBlessingChosen();

        var temple = GetComponent<Temple>();

        if (temple != null)
        {
            temple.NotifyGodSelected(selectedGod);
        }
    }

    // Devuelve el estado de activación del modificador o bendición de la estructura
    public bool HasBlessingChosen()
    {
        return blessingChosen;
    }

    // Registra de forma lógica la aplicación de la bendición divina en el controlador
    public void MarkBlessingChosen()
    {
        blessingChosen = true;
    }

}

/*
   ========================================================================================================
   DESCRIPCIÓN GENERAL DEL CÓDIGO
   ========================================================================================================
   Este script actúa como el Controlador de Evolución y Actualización de Estructuras (ConstructionController). 
   Su responsabilidad principal es gestionar el ciclo de vida evolutivo de las edificaciones del jugador, permitiendo 
   que las estructuras base muten y adopten características especializadas al alinearse con un camino mitológico 
   específico (`GodType`). Coordina tanto las interacciones de selección por clicks directos en la interfaz tridimensional 
   como la deducción de costos económicos y el reemplazo físico de mallas mediante la inyección de datos dinámicos.

   Características clave:
   1. Mecánica de Evolución por Bifurcación Divina: Permite que una construcción base elija una deidad patrona. 
      Almacena el estado jerárquico (`level`) y restringe la reelección del camino místico una vez que la estructura 
      ha avanzado más allá de su nivel inicial, asegurando la consistencia en el diseño de progresión táctica.
   2. Reemplazo de Prefabs en Caliente por Pooling: En lugar de destruir y recrear instancias de forma masiva (lo que 
      congelaría el juego por recolección de basura), desactiva la entidad antigua y solicita al sistema `Pooling` el nuevo 
      modelo correspondiente a la evolución, preservando los datos de posición espacial y la orientación original.
   3. Intercambio Sincrónico de Nodos en Estructuras Colectivas: Posee lógica para integrarse con clases de red y grupos 
      (`ConstructionGroup`). Al actualizar un fragmento de una estructura compuesta (como un tramo de murallas), propaga 
      la evolución a todos los miembros y actualiza dinámicamente las referencias internas de la lista sin romper los enlaces.
   ========================================================================================================
*/