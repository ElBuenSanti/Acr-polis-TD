using UnityEngine;
using System.Collections.Generic;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;

    [SerializeField]
    private List<GameObject> buildingPrefabs;
    private GameObject selectedBuilding;
    public short tempVariable = 0;

    //public GameObject prefabToCreate;
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
        selectedBuilding = buildingPrefabs[tempVariable];

        GameObject building = constructionPooling.CreateObject(selectedBuilding, tile.transform);
        building.transform.position = tile.transform.position + new Vector3(0, hightOffset, 0);
        building.transform.rotation = Quaternion.identity;
        /*
        GameObject building = constructionPooling.CreateObject(prefabToCreate, tile.transform);
        building.transform.position = tile.transform.position + new Vector3(0, hightOffset, 0);
        building.transform.rotation = Quaternion.identity;
        */
    }
}
