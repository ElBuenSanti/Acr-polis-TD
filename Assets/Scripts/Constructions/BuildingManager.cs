using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance;

    public Pooling constructionPooling;

    [Header("Building Data")]
    [SerializeField] private List<ConstructionData> buildingDataList;
    [SerializeField] public ConstructionData currentBuilding;
    [SerializeField] public ConstructionController selectedConstruction;
    [SerializeField] private ConstructionController constructionToMove;

    [Header("Preview")]
    [SerializeField] private Material validPreviewMaterial;
    [SerializeField] private Material invalidPreviewMaterial;

    private GameObject previewObject;
    private Tile hoveredTile;

    public int tempVariable = 0;

    private bool moveMode;
    private float hightOffset = 1.5f;

    private void Awake()
    {
        Instance = this;
        constructionPooling = FindAnyObjectByType<Pooling>();
    }

    private void Start()
    {
        currentBuilding = null;
    }

    public void SetConstructionIndex(int index)
    {
        if (index < 0 || index >= buildingDataList.Count)
            return;

        tempVariable = index;
        currentBuilding = buildingDataList[tempVariable];

        moveMode = false;
        constructionToMove = null;

        ShowStatus("Seleccionaste: " + currentBuilding.type);

        CreatePreviewObject();
        UpdatePreview();
    }

    public void ClearCurrentBuildingSelection()
    {
        currentBuilding = null;
        ClearPreviewObject();
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

    private void UpdatePreview()
    {
        foreach (Tile t in Tile.AllTiles)
            t.ResetSelf();

        if (currentBuilding == null || hoveredTile == null)
        {
            UpdatePreviewObject(null);
            return;
        }

        ApplyPreview(hoveredTile);
        UpdatePreviewObject(hoveredTile);
    }

    private void ApplyPreview(Tile tile)
    {
        bool valid = CanPlaceOnTile(tile);

        tile.SetTempColor(valid ? Color.yellow : Color.red);

        if (currentBuilding.type == ConstructionType.Wall)
        {
            foreach (Tile t in tile.sameRowNeighbors)
            {
                if (t != null)
                    t.SetTempColor(valid ? Color.cyan : Color.red);
            }

            foreach (Tile t in tile.upperRowNeighbors)
            {
                if (t != null)
                    t.SetTempColor(valid ? Color.green : Color.red);
            }
        }
    }

    public void Select(ConstructionController construction)
    {
        selectedConstruction = construction;
        Debug.Log("Seleccionado: " + construction.name);
    }

    public void PlaceBuilding(Tile tile)
    {
        if (WaveSpawner.Instance.IsWaveRunning())
        {
            PlayInvalidSound();
            ShowStatus("No puedes construir durante la oleada");
            return;
        }

        if (moveMode)
        {
            MoveConstruction(tile);
            return;
        }

        if (currentBuilding == null)
        {
            PlayInvalidSound();
            ShowStatus("Primero selecciona una construcción");
            return;
        }

        if (tile == null)
        {
            PlayInvalidSound();
            ShowStatus("Selecciona una casilla válida");
            return;
        }

        List<Tile> tilesToBuild = GetTilesToBuild(tile);

        foreach (Tile t in tilesToBuild)
        {
            if (t == null)
            {
                PlayInvalidSound();
                ShowStatus("Espacio inválido");
                return;
            }

            if (t.IsOccupied)
            {
                PlayInvalidSound();
                ShowStatus("Espacio ocupado");
                return;
            }
        }

        bool hasSameType = ExistsConstructionOfType(currentBuilding.type);
        List<WillProduction> costList = hasSameType ? currentBuilding.bonusWillToPay : currentBuilding.willToPay;

        foreach (WillProduction w in costList)
        {
            if (!WillManager.Instance.SpendMoney(w.type, w.amount))
            {
                PlayInvalidSound();
                ShowStatus("Te falta " + w.type + " para construir");
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
            if (t == null)
                continue;

            t.SetOccupied(true);

            GameObject building = constructionPooling.CreateObject(currentBuilding.prefab, t.transform);

            building.transform.position = t.transform.position + Vector3.up * hightOffset;
            building.transform.rotation = Quaternion.identity;

            BaseConstruction baseConstruction = building.GetComponent<BaseConstruction>();
            ConstructionController constructionController = building.GetComponent<ConstructionController>();

            if (group != null)
                constructionController.group = group;

            baseConstruction.SetTile(t);
            baseConstruction.Initialize(currentBuilding);
            constructionController.Initialize(currentBuilding);
        }

        if (GameplaySoundPlayer.Instance != null)
        {
            if (currentBuilding.type == ConstructionType.Wall)
                GameplaySoundPlayer.Instance.PlayBuildWall();
            else
                GameplaySoundPlayer.Instance.PlayBuildStructure();
        }

        ShowStatus("Construcción colocada");

        ClearCurrentBuildingSelection();
    }

    public void TryEnterMoveMode()
    {
        currentBuilding = null;
        ClearPreviewObject();
        UpdatePreview();

        if (WaveSpawner.Instance.IsWaveRunning())
        {
            PlayInvalidSound();
            ShowStatus("No puedes mover estructuras durante la oleada");
            return;
        }

        if (selectedConstruction == null)
        {
            PlayInvalidSound();
            ShowStatus("Selecciona una estructura para mover");
            return;
        }

        BaseConstruction baseConstruction = selectedConstruction.GetComponent<BaseConstruction>();

        if (baseConstruction == null || baseConstruction.Data == null)
        {
            PlayInvalidSound();
            ShowStatus("Estructura inválida");
            return;
        }

        if (baseConstruction.Data.type == ConstructionType.Temple)
        {
            PlayInvalidSound();
            ShowStatus("El templo no se puede mover");
            return;
        }

        moveMode = true;
        constructionToMove = selectedConstruction;

        ShowStatus("Modo mover activado");
    }

    private void MoveConstruction(Tile newTile)
    {
        if (constructionToMove == null)
        {
            PlayInvalidSound();
            ShowStatus("No hay estructura para mover");
            return;
        }

        if (newTile == null)
        {
            PlayInvalidSound();
            ShowStatus("Espacio inválido");
            return;
        }

        List<ConstructionController> constructionsToMove = new List<ConstructionController>();
        List<Tile> oldTiles = new List<Tile>();
        List<Tile> newTiles = new List<Tile>();

        if (constructionToMove.group == null)
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
                PlayInvalidSound();
                ShowStatus("Espacio inválido");
                return;
            }

            if (tile.IsOccupied && !oldTiles.Contains(tile))
            {
                PlayInvalidSound();
                ShowStatus("Espacio ocupado");
                return;
            }
        }

        foreach (Tile tile in oldTiles)
        {
            if (tile != null)
                tile.SetOccupied(false);
        }

        for (int i = 0; i < constructionsToMove.Count; i++)
        {
            ConstructionController construction = constructionsToMove[i];
            BaseConstruction baseConstruction = construction.GetComponent<BaseConstruction>();
            Tile targetTile = newTiles[i];

            targetTile.SetOccupied(true);

            construction.transform.position = targetTile.transform.position + Vector3.up * hightOffset;
            construction.transform.rotation = Quaternion.identity;

            baseConstruction.SetTile(targetTile);
        }

        if (constructionToMove.group != null)
            constructionToMove.group.tiles = newTiles;

        moveMode = false;
        constructionToMove = null;

        ShowStatus("Construcción movida con éxito");

        ClearCurrentBuildingSelection();
    }

    private bool ExistsConstructionOfType(ConstructionType type)
    {
        BaseConstruction[] all = FindObjectsByType<BaseConstruction>(FindObjectsSortMode.None);

        foreach (BaseConstruction b in all)
        {
            if (b != null && b.Data != null && b.Data.type == type)
                return true;
        }

        return false;
    }

    private List<Tile> GetTilesToBuild(Tile tile)
    {
        List<Tile> tiles = new List<Tile>();

        if (tile == null)
            return tiles;

        tiles.Add(tile);

        if (currentBuilding != null && currentBuilding.type == ConstructionType.Wall)
        {
            tiles.AddRange(tile.sameRowNeighbors);
            tiles.AddRange(tile.upperRowNeighbors);
        }

        return tiles;
    }

    private bool CanPlaceOnTile(Tile tile)
    {
        if (tile == null || currentBuilding == null)
            return false;

        List<Tile> tilesToBuild = GetTilesToBuild(tile);

        foreach (Tile t in tilesToBuild)
        {
            if (t == null || t.IsOccupied)
                return false;
        }

        return HasEnoughResources(currentBuilding);
    }

    private bool HasEnoughResources(ConstructionData data)
    {
        if (data == null)
            return false;

        bool hasSameType = ExistsConstructionOfType(data.type);
        List<WillProduction> costList = hasSameType ? data.bonusWillToPay : data.willToPay;

        foreach (WillProduction w in costList)
        {
            if (!WillManager.Instance.HasEnoughMoney(w.type, w.amount))
                return false;
        }

        return true;
    }

    private void CreatePreviewObject()
    {
        ClearPreviewObject();

        if (currentBuilding == null || currentBuilding.prefab == null)
            return;

        previewObject = Instantiate(currentBuilding.prefab);
        previewObject.name = "Preview_" + currentBuilding.type;

        foreach (Collider col in previewObject.GetComponentsInChildren<Collider>())
            col.enabled = false;

        foreach (MonoBehaviour behaviour in previewObject.GetComponentsInChildren<MonoBehaviour>())
            behaviour.enabled = false;

        ApplyPreviewMaterial(validPreviewMaterial);
        previewObject.SetActive(false);
    }

    private void ClearPreviewObject()
    {
        if (previewObject != null)
            Destroy(previewObject);

        previewObject = null;
    }

    private void UpdatePreviewObject(Tile tile)
    {
        if (previewObject == null || currentBuilding == null || tile == null)
        {
            if (previewObject != null)
                previewObject.SetActive(false);

            return;
        }

        previewObject.SetActive(true);
        previewObject.transform.position = tile.transform.position + Vector3.up * hightOffset;
        previewObject.transform.rotation = Quaternion.identity;

        bool valid = CanPlaceOnTile(tile);
        ApplyPreviewMaterial(valid ? validPreviewMaterial : invalidPreviewMaterial);
    }

    private void ApplyPreviewMaterial(Material material)
    {
        if (previewObject == null || material == null)
            return;

        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
            renderer.material = material;
    }

    private void PlayInvalidSound()
    {
        if (GameplaySoundPlayer.Instance != null)
            GameplaySoundPlayer.Instance.PlayInvalidPlacement();
    }

    private void ShowStatus(string message)
    {
        if (StatusMessageUI.Instance != null)
            StatusMessageUI.Instance.ShowMessage(message);

        Debug.Log(message);
    }
}