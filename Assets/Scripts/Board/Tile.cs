using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private Renderer tileColor;
    private bool isOccupied = false;
    public bool IsOccupied => isOccupied;

    [SerializeField] private Color originalTileColor;

    public List<Tile> sameRowNeighbors = new List<Tile>();
    public List<Tile> upperRowNeighbors = new List<Tile>();

    public static List<Tile> AllTiles = new List<Tile>();

    void Awake()
    {
        tileColor = GetComponent<Renderer>();
        originalTileColor = tileColor.material.color;
        AllTiles.Add(this);
    }

    public void SetColor(Color color)
    {
        tileColor.material.color = color;
        originalTileColor = color;
    }

    void OnMouseEnter()
    {
        if (!WaveSpawner.Instance.IsWaveRunning())
        {
            BuildingManager.Instance.SetHoveredTile(this);
        }
        //BuildingManager.Instance.SetHoveredTile(this);
    }

    void OnMouseExit()
    {
        if (!WaveSpawner.Instance.IsWaveRunning())
        {
            BuildingManager.Instance.ClearHover();
        }
        //BuildingManager.Instance.ClearHover();
    }

    void OnMouseDown()
    {
        if (WaveSpawner.Instance.IsWaveRunning())
        {
            return;
        }

        if (isOccupied)
            return;

        //bool success = 
        BuildingManager.Instance.PlaceBuilding(this);

        //if (!success)
            //return;

        //SetOccupied(true); 
    }

    public void SetTempColor(Color color)
    {
        if (isOccupied)
            return;

        tileColor.material.color = color;
    }

    public void ResetSelf()
    {
        if (isOccupied)
            return;

        tileColor.material.color = originalTileColor;
    }

    public void SetOccupied(bool value)
    {
        isOccupied = value;

        if (isOccupied)
        {
            tileColor.material.color = Color.green;
        }
        else
        {
            ResetSelf();
        }
    }
}