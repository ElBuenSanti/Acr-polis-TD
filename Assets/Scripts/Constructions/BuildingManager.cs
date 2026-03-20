using UnityEngine;
using System.Collections.Generic;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;

    public GameObject prefabToCreate;
    private float hightOffset = 1.5f;

    /*
    public List<GameObject> templesPrefab;
    public List<GameObject> defensesPrefab;
    public List<GameObject> barracksPrefab;
    */

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        /*
        templesPrefab = new List<GameObject>();
        defensesPrefab = new List<GameObject>();
        barracksPrefab = new List<GameObject>();
        */
    }
    public void PlaceBuilding(Tile tile)
    {
        Instantiate(prefabToCreate, tile.transform.position + new Vector3(0, hightOffset, 0), Quaternion.identity);
    }
}
