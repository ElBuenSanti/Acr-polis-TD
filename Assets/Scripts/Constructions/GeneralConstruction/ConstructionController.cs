using UnityEngine;
using UnityEngine.InputSystem.XR;

public class ConstructionController : MonoBehaviour
{
    public ConstructionDatabase database;
    private Pooling pooling;
    public ConstructionGroup group; //

    private ConstructionType type;
    //private GodType god, selectedGod;
    private int level;

    private GodType currentGod;
    private GodType selectedGod = GodType.Base;

    private Transform parentTile;


    void Awake()
    {
        pooling = FindAnyObjectByType<Pooling>();
        database = FindAnyObjectByType<ConstructionDatabase>();
    }

    public void Initialize(ConstructionData data)
    {
        type = data.type;
        //god = data.god;
        level = data.level;
        parentTile = transform.parent;
        selectedGod = data.god;

        if (group != null)
        {
            group.Add(this);
        }
    }

    public void Upgrade()
    {
        int nextLevel = level + 1;

        Debug.Log("UPGRADE con dios: " + selectedGod);

        ConstructionData newData = database.GetData(type, selectedGod, nextLevel); //god

        if (newData == null)
        {
            Debug.Log("No hay más niveles");
            return;
        }

        foreach (var w in newData.willToPay)
        {
            if (!WillManager.Instance.SpendMoney(w.type, w.amount))
            {
                Debug.Log("No te alcanza");
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

    void OnMouseDown()
    {
        if (!WaveSpawner.Instance.IsWaveRunning() || type == ConstructionType.Wall)
        {
            BuildingManager.Instance.Select(this);
        }
        //BuildingManager.Instance.Select(this);
        //Upgrade();
    }

    public void SetSelectedGod(GodType god)
    {
        if (level > 1)
        {
            Debug.Log("El dios ya está fijado");
            return;
        }

        selectedGod = god;
    }

    void UpgradeSingle(ConstructionData newData)
    {
        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;

        gameObject.SetActive(false);

        GameObject newBuilding = pooling.CreateObject(newData.prefab, null);

        newBuilding.transform.SetPositionAndRotation(position, rotation);

        var newConstruction = newBuilding.GetComponent<BaseConstruction>();
        var newController = newBuilding.GetComponent<ConstructionController>();

        if (group != null)
        {
            newController.group = group;
        }
         

        newConstruction.Initialize(newData);
        newController.Initialize(newData);
    }

}






/*
Vector3 position = transform.position;
Quaternion rotation = transform.rotation;


gameObject.SetActive(false);

Debug.Log("NewData: " + newData);
Debug.Log("Prefab: " + newData.prefab);
Debug.Log("ParentTile: " + parentTile);
GameObject newBuilding = pooling.CreateObject(newData.prefab, parentTile);


newBuilding.transform.SetPositionAndRotation(position, rotation);

var newConstruction = newBuilding.GetComponent<BaseConstruction>(); //BaseConstruction
var newController = newBuilding.GetComponent<ConstructionController>(); //ConstructionController

newConstruction.Initialize(newData);
newController.Initialize(newData);
*/