using System.Collections.Generic;
using Mono.Cecil.Cil;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;

    [SerializeField]
    private List<ConstructionData> buildingDataList; // ?? CAMBIO

    public short tempVariable = 0;

    public Pooling constructionPooling;

    private float hightOffset = 1.5f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        constructionPooling = FindAnyObjectByType<Pooling>();
    }

    public void PlaceBuilding(Tile tile)
    {
        ConstructionData data = buildingDataList[tempVariable];

        GameObject building = constructionPooling.CreateObject(data.prefab, tile.transform);

        building.transform.position = tile.transform.position + new Vector3(0, hightOffset, 0);
        building.transform.rotation = Quaternion.identity;

        BaseConstruction construction = building.GetComponent<BaseConstruction>();
        construction.Initialize(data);
    }
}
/*
using System.Collections.Generic;
using Mono.Cecil.Cil;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;

    [SerializeField]
    //private List<GameObject> buildingPrefabs;
    private GameObject selectedBuilding;
    public short tempVariable = 0;

    public Pooling constructionPooling;

    private float hightOffset = 1.5f;

    [SerializeField]
    private List<ConstructionData> buildingDataList;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        constructionPooling = FindAnyObjectByType<Pooling>();
    }

    /*
    public void PlaceBuilding(Tile tile)
    {
        selectedBuilding = buildingPrefabs[tempVariable];

        GameObject building = constructionPooling.CreateObject(selectedBuilding, tile.transform);
        building.transform.position = tile.transform.position + new Vector3(0, hightOffset, 0);
        building.transform.rotation = Quaternion.identity;

    }
    
    public void PlaceBuilding(Tile tile)
    {
        ConstructionData data = buildingDataList[tempVariable];

        GameObject building = constructionPooling.CreateObject(data.prefab, tile.transform);

        building.transform.position = tile.transform.position + new Vector3(0, hightOffset, 0);
        building.transform.rotation = Quaternion.identity;

        // ?? AQUÍ ESTÁ LA MAGIA
        Construction construction = building.GetComponent<Construction>();
        construction.Initialize(data);
    }
}
*/