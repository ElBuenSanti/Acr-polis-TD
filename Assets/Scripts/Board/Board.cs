using UnityEngine;

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private int rows = 10;

    [SerializeField] private int columns = 6;
    private float spaceBetweenTiles = 1.75f;
    private float spaceBetweenRows = 1.5f;
    private float rowOffset = 0f;

    private float hightValue;

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
            if(x % 2 != 0)
            {
                columns--;
                rowOffset = spaceBetweenTiles / 2;
            }
            else
            {
                columns++;
                rowOffset = 0f;
            }

                for (short z = 0; z < columns; z++)
                {
                hightValue = Random.Range(0f, 0.5f);
                positionOfTiles = new Vector3(x * spaceBetweenRows, hightValue, z * spaceBetweenTiles + rowOffset);
                    tileInstance = Instantiate(tilePrefab, positionOfTiles, Quaternion.Euler(90f, 0f, -90f)); //Quaternion.identity)
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
