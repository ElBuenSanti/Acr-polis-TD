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

    //Initialize new construction with upgrade
    void Awake()
    {
        pooling = FindAnyObjectByType<Pooling>();
        database = FindAnyObjectByType<ConstructionDatabase>();
    }

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

    //Select a construction to upgrade JUST in between waves or it is a wall
    void OnMouseDown()
    {
        if (!WaveSpawner.Instance.IsWaveRunning() || type == ConstructionType.Wall)
        {
            BuildingManager.Instance.Select(this);
        }
    }

    //Upgrade construction by selecting a God.
    public void SetSelectedGod(GodType god)
    {
        if (level > 1)
        {
            Debug.Log("Ya estas recorriendo el camino divino del Dios " + selectedGod);
            return;
        }

        selectedGod = god;
    }


    public void Upgrade()
    {
        if(selectedGod == GodType.Base)
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

    public bool HasBlessingChosen()
    {
        return blessingChosen;
    }

    public void MarkBlessingChosen()
    {
        blessingChosen = true;
    }

}





