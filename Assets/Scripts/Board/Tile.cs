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

    //Visual Change
    public void SetColor(Color color)
    {
        tileColor.material.color = color;
        originalTileColor = color;
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

    //Hover
    void OnMouseEnter() //HOOVER
    {
        if (!WaveSpawner.Instance.IsWaveRunning())
        {
            BuildingManager.Instance.SetHoveredTile(this);
        }
    }

    void OnMouseExit() //salir del hoover
    {
        if (!WaveSpawner.Instance.IsWaveRunning())
        {
            BuildingManager.Instance.ClearHover(); 
        }
    }

    void OnMouseDown()
    {
        if (WaveSpawner.Instance.IsWaveRunning())
        {
            return;
        }

        if (isOccupied)
            return;

        BuildingManager.Instance.PlaceBuilding(this);
    }

    //Tile Status
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