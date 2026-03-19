using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private int rows = 10;

    [SerializeField] private int columns = 6;
    private float spaceBetweenTiles = 1f;

    void Start()
    {
        GenerateBoard();
    }

    void Update()
    {
        
    }

    void GenerateBoard()
    {
        Vector3 positionOfTiles;
        GameObject tileInstance;

        for (short x=0; x<rows; x++)
        {
            for(short z=0; z<columns; z++)
            {
                positionOfTiles = new Vector3(x * spaceBetweenTiles, 0, z * spaceBetweenTiles);
                tileInstance = Instantiate(tilePrefab, positionOfTiles, Quaternion.identity);

                Tile tile = tileInstance.GetComponent<Tile>();

                if ((x + z) % 2 == 0)
                {
                    tile.SetColor(Color.white);
                }
                else
                {
                    tile.SetColor(Color.black);
                }
            }
        }
    }
}
