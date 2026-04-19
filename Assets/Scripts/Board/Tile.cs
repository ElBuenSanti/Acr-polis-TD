/*using UnityEngine;

public class Tile : MonoBehaviour
{
    private Renderer tileColor;
    private bool isOccupied = false;
    [SerializeField] private Color originalTileColor;

    public Tile left;
    public Tile right;
    public Tile up;
    public Tile down;


    void Awake()
    {
        tileColor = GetComponent<Renderer>();
    }

    public void SetColor(Color color)
    {
        tileColor.material.color = color;
        originalTileColor = color;
    }

    void OnMouseEnter()
    {
        tileColor.material.color = Color.yellow;
    }

    void OnMouseExit()
    {
        tileColor.material.color = originalTileColor;
    }

    void OnMouseDown()
    {
        if (!isOccupied)
        {
            tileColor.material.color = Color.green;
            originalTileColor = Color.green;
            isOccupied = true;
            BuildingManager.Instance.PlaceBuilding(this);
        }

    }

}

using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private Renderer tileColor;

    private bool isOccupied = false;

    [SerializeField] private Color originalTileColor;

    public List<Tile> sameRowNeighbors = new List<Tile>();
    public List<Tile> upperRowNeighbors = new List<Tile>();
    public static List<Tile> AllTiles = new List<Tile>();

    void Awake()
    {
        tileColor = GetComponent<Renderer>();
        AllTiles.Add(this);
        Debug.Log("Tile registrado: " + transform.position);
    }

    public void SetColor(Color color)
    {
        tileColor.material.color = color;
        originalTileColor = color;
    }

    void OnMouseEnter()
    {
        HandleHover(true);
    }

    void OnMouseExit()
    {
        HandleHover(false);
    }

    void OnMouseDown()
    {
        if (isOccupied)
            return;

        isOccupied = true;

        tileColor.material.color = Color.green;
        originalTileColor = Color.green;

        BuildingManager.Instance.PlaceBuilding(this);
    }


    void HandleHover(bool enter)
    {
        var data = BuildingManager.Instance.currentBuilding;

        if (data == null)
            return;

        if (enter)
            ApplyHighlight(data);
        else
            ResetHighlight(data);
    }


    void ApplyHighlight(ConstructionData data)
    {
        tileColor.material.color = Color.yellow;

        if (data.type == ConstructionType.Wall)
        {
            foreach (Tile t in sameRowNeighbors)
            {
                if (t != null)
                    t.SetTempColor(Color.cyan);
            }

            foreach (Tile t in upperRowNeighbors)
            {
                if (t != null)
                    t.SetTempColor(Color.green);
            }
        }
    }

    void ResetHighlight(ConstructionData data)
    {
        ResetSelf();

        if (data.type == ConstructionType.Wall)
        {
            foreach (Tile t in sameRowNeighbors)
            {
                if (t != null)
                    t.ResetSelf();
            }

            foreach (Tile t in upperRowNeighbors)
            {
                if (t != null)
                    t.ResetSelf();
            }
        }
    }

    public void SetTempColor(Color color)
    {
        if (!isOccupied)
            tileColor.material.color = color;
    }

    public void ResetSelf()
    {
        tileColor.material.color = originalTileColor;
    }



}

*/
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
        AllTiles.Add(this);
    }

    public void SetColor(Color color)
    {
        tileColor.material.color = color;
        originalTileColor = color;
    }

    void OnMouseEnter()
    {
        BuildingManager.Instance.SetHoveredTile(this);
    }

    void OnMouseExit()
    {
        BuildingManager.Instance.ClearHover();
    }

    void OnMouseDown()
    {
        if (isOccupied)
            return; //si ya esta ocupado, no hagas nada

        bool success = BuildingManager.Instance.PlaceBuilding(this); //si se logró construir

        if (!success)
            return; //si no no hagas nada

        isOccupied = true;

        tileColor.material.color = Color.green;
        originalTileColor = Color.green;
    }

    public void SetTempColor(Color color)
    {
        tileColor.material.color = color;
    }

    public void ResetSelf()
    {
        tileColor.material.color = originalTileColor;
    }
}