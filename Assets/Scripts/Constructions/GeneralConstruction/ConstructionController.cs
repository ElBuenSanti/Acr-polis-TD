using UnityEngine;
using UnityEngine.InputSystem.XR;

public class ConstructionController : MonoBehaviour
{
    public ConstructionDatabase database;
    private Pooling pooling;

    private ConstructionType type;
    private GodType god, selectedGod;
    private int level;

    private Transform parentTile;


    void Awake()
    {
        pooling = FindAnyObjectByType<Pooling>();
        database = FindAnyObjectByType<ConstructionDatabase>();
    }

    public void Initialize(ConstructionData data)
    {
        type = data.type;
        god = data.god;
        level = data.level;
        parentTile = transform.parent;
    }

    public void Upgrade()
    {
        int nextLevel = level + 1;

            ConstructionData newData = database.GetData(type, god, nextLevel);

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

        /*
        if(!WillManager.Instance.SpendMoney(newData.willToPay, newData.cost))
        {
            Debug.Log("No te alcanza");
            return;
        }
        */

        Vector3 position = transform.position;
        Quaternion rotation = transform.rotation;


        gameObject.SetActive(false);

        Debug.Log("NewData: " + newData);
        Debug.Log("Prefab: " + newData.prefab);
        Debug.Log("ParentTile: " + parentTile);
        GameObject newBuilding = pooling.CreateObject(newData.prefab, parentTile);

        /*
        newBuilding.transform.position = position;
        newBuilding.transform.rotation = rotation;
        */
        newBuilding.transform.SetPositionAndRotation(position, rotation);

        var newConstruction = newBuilding.GetComponent<BaseConstruction>(); //BaseConstruction
        var newController = newBuilding.GetComponent<ConstructionController>(); //ConstructionController

        newConstruction.Initialize(newData);
        newController.Initialize(newData);

    }

    void OnMouseDown()
    {
        Upgrade();
    }

}