using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;

    [SerializeField]
    private List<ConstructionData> buildingDataList;
    [SerializeField]
    public ConstructionData currentBuilding;
    [SerializeField]
    public ConstructionController selectedConstruction;

    BaseConstruction baseConstruction;
    ConstructionController constructionController;

    public int tempVariable = 0;

    public Pooling constructionPooling;

    private float hightOffset = 1.5f;
    private Tile hoveredTile;

    //moveConstruction
    private bool moveMode;
    [SerializeField]
    public ConstructionController constructionToMove;
    //

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
        //move
        if (moveMode)
        {
            MoveConstruction(tile);
            return;
        }
        //
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
            constructionController.Initialize(currentBuilding, t.transform);
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

    public void TryEnterMoveMode()
    {
        if (selectedConstruction == null)
            return;

        BaseConstruction baseConstruction = selectedConstruction.GetComponent<BaseConstruction>();

        if (baseConstruction.Data.type == ConstructionType.Temple)
        {
            Debug.Log("El templo no se puede mover");
            return;
        }

        moveMode = true;
        constructionToMove = selectedConstruction;

        Debug.Log("Modo mover activado");
    }

    void MoveConstruction(Tile newTile)
    {
        if (constructionToMove == null)
            return;

        List<ConstructionController> constructionsToMove = new List<ConstructionController>();

        List<Tile> oldTiles = new List<Tile>();

        List<Tile> newTiles = new List<Tile>();

        if (constructionToMove.group == null) //construcción básica
        {
            constructionsToMove.Add(constructionToMove);

            BaseConstruction baseConstruction = constructionToMove.GetComponent<BaseConstruction>();

            oldTiles.Add(baseConstruction.GetTile());
            newTiles.Add(newTile);
        }
        else
        {
            constructionsToMove.AddRange(constructionToMove.group.members);
            oldTiles.AddRange(constructionToMove.group.tiles);
            newTiles.Add(newTile);
            newTiles.AddRange(newTile.sameRowNeighbors);
            newTiles.AddRange(newTile.upperRowNeighbors);
        }

        foreach (Tile tile in newTiles)
        {
            if (tile == null)
            {
                Debug.Log("Espacio inválido");
                return;
            }

            if (tile.IsOccupied && !oldTiles.Contains(tile))
            {
                Debug.Log("Espacio ocupado");
                return;
            }
        }

        foreach (Tile tile in oldTiles)
        {
            tile.SetOccupied(false);
        }

        for (int i = 0; i < constructionsToMove.Count; i++)
        {
            ConstructionController construction = constructionsToMove[i];

            Tile targetTile = newTiles[i];

            BaseConstruction baseConstruction = construction.GetComponent<BaseConstruction>();

            targetTile.SetOccupied(true);

            construction.transform.position = targetTile.transform.position + Vector3.up * hightOffset;

            construction.transform.rotation = Quaternion.identity;

            baseConstruction.SetTile(targetTile);
        }

        if (constructionToMove.group != null)
        {
            constructionToMove.group.tiles = newTiles;
        }

        moveMode = false;
        constructionToMove = null;

        Debug.Log("Construcción movida");
    }

    /*
    void MoveConstruction(Tile newTile)
    {
        if (constructionToMove == null)
            return;

        BaseConstruction baseConstruction = constructionToMove.GetComponent<BaseConstruction>();

        Tile oldTile = baseConstruction.GetTile();

        if (newTile.IsOccupied)
        {
            Debug.Log("Tile ocupada");
            return;
        }

        oldTile.SetOccupied(false);

        newTile.SetOccupied(true);

        constructionToMove.transform.position = newTile.transform.position + Vector3.up * hightOffset;
        constructionToMove.transform.rotation = Quaternion.identity;

        baseConstruction.SetTile(newTile);

        constructionToMove.SetTileTransform(newTile.transform);

        moveMode = false;
        constructionToMove = null;

        Debug.Log("Construcción movida");
    }
    */

}