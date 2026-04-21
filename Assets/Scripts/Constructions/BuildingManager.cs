using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;

    [SerializeField]
    private List<ConstructionData> buildingDataList;
    public ConstructionData currentBuilding;
    public ConstructionController selectedConstruction;

    BaseConstruction baseConstruction;
    ConstructionController constructionController;

    public int tempVariable = 0;

    public Pooling constructionPooling;

    private float hightOffset = 1.5f;
    private Tile hoveredTile;

    void Awake()
    {
        Instance = this;
        constructionPooling = FindAnyObjectByType<Pooling>();
    }

    void Start()
    {
        currentBuilding = null;
    }


    //Funciones

    public void SetConstructionIndex(int index)
    {
        if (index < 0 || index >= buildingDataList.Count)
            return;

        tempVariable = index;

        currentBuilding = buildingDataList[tempVariable];

        UpdatePreview();
    }



    public void SetHoveredTile(Tile tile)
    {
        hoveredTile = tile;
        UpdatePreview();
    }

    public void ClearHover()
    {
        hoveredTile = null;
        UpdatePreview();
    }


    void UpdatePreview()
    {
        foreach (Tile t in Tile.AllTiles)
            t.ResetSelf();

        if (currentBuilding == null || hoveredTile == null)
            return;

        ApplyPreview(hoveredTile);
    }

    void ApplyPreview(Tile tile)
    {
        tile.SetTempColor(Color.yellow);

        if (currentBuilding.type == ConstructionType.Wall)
        {
            foreach (Tile t in tile.sameRowNeighbors)
                t.SetTempColor(Color.cyan);

            foreach (Tile t in tile.upperRowNeighbors)
                t.SetTempColor(Color.green);
        }
    }

    public void PlaceBuilding(Tile tile)
    {
        if (currentBuilding == null)
            return;

        List<Tile> tilesToBuild = new List<Tile>();

        tilesToBuild.Add(tile);

        if (currentBuilding.type == ConstructionType.Wall) //si es muralla ver si se puede poner ahí
        {
            tilesToBuild.AddRange(tile.sameRowNeighbors);
            tilesToBuild.AddRange(tile.upperRowNeighbors);

            foreach (Tile t in tilesToBuild)
            {
                if (t == null)
                    return;

                if (t.IsOccupied)
                {
                    Debug.Log("Espacio ocupado");
                    return;
                }
            }
        }

        bool hasSameType = ExistsConstructionOfType(currentBuilding.type);

        var costList = hasSameType
            ? currentBuilding.bonusWillToPay
            : currentBuilding.willToPay;

        foreach (var w in costList)
        {
            if (!WillManager.Instance.SpendMoney(w.type, w.amount))
            {
                Debug.Log("No te alcanza");
                return;
            }
        }

        /*
        foreach (var w in currentBuilding.willToPay) //si tiene dinero
        {
            if (!WillManager.Instance.SpendMoney(w.type, w.amount))
            {
                Debug.Log("No te alcanza");
                return;
            }
        }
        */

        ConstructionGroup group = null;

        if (tilesToBuild.Count > 1)
        {
            GameObject groupObj = new GameObject("ConstructionGroup");
            group = groupObj.AddComponent<ConstructionGroup>();
            group.tiles = tilesToBuild;
        }

        foreach (Tile t in tilesToBuild)
        {
            if (t == null) continue;

            t.SetOccupied(true);

            GameObject building = constructionPooling.CreateObject(currentBuilding.prefab, t.transform);

            building.transform.position = t.transform.position + Vector3.up * hightOffset;
            building.transform.rotation = Quaternion.identity;

            var baseConstruction = building.GetComponent<BaseConstruction>();
            var constructionController = building.GetComponent<ConstructionController>();

            if (group != null)
            {
                constructionController.group = group;
            }

            baseConstruction.SetTile(t);

            baseConstruction.Initialize(currentBuilding);
            constructionController.Initialize(currentBuilding);
        }

        return;
    }

    public void Select(ConstructionController construction)
    {
        selectedConstruction = construction;
        Debug.Log("Seleccionado: " + construction.name);
    }

    bool ExistsConstructionOfType(ConstructionType type)
    {
        BaseConstruction[] all = FindObjectsByType<BaseConstruction>(FindObjectsSortMode.None);

        foreach (var b in all)
        {
            if (b.Data.type == type)
            {
                return true;
            }
        }

        return false;
    }

}