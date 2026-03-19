using UnityEngine;

public class Tile : MonoBehaviour
{
    private Renderer tileColor;
    [SerializeField] private Color originalTileColor;

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
        tileColor.material.color = Color.green;
        originalTileColor = Color.green;
    }

}
