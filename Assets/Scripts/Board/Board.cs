using UnityEngine;
using UnityEngine.AI; //

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private int rows = 2;
    [SerializeField] private int columns = 1;

    [SerializeField] private NavMeshSurface navSurface; 
    private string navMeshObject = "NavMeshSurfaceBoard"; 
    private string layerMaskName = "Tiles"; 

    private float spaceBetweenTiles = 3f; //1.75
    private float spaceBetweenRows = 0.8f; //1.5
    private float rowOffset = 0f;

    private Tile[,] tiles;

    //private float hightValue;

    void Start()
    {
        GameObject navObj = new GameObject(navMeshObject);
        navSurface = navObj.AddComponent<NavMeshSurface>();
        navSurface.collectObjects = CollectObjects.All;
        navSurface.layerMask = LayerMask.GetMask(layerMaskName); 
        

        GenerateBoard();
        AssignRowNeighbors();//
        navSurface.BuildNavMesh(); 
    }
    void GenerateBoard()
    {
        tiles = new Tile[rows, columns];
        Vector3 positionOfTiles;
        GameObject tileInstance;
        int currentColumns = columns;

        for (short x=0; x<rows; x++)
        {
            if(x % 2 != 0)
            {
                currentColumns--; 
                rowOffset = spaceBetweenTiles / 2;
            }
            else
            {
                currentColumns = columns; 
                rowOffset = 0f;
            }

                for (short z = 0; z < currentColumns; z++) 
                {
                    //hightValue = Random.Range(0f, 0.5f);
                    positionOfTiles = new Vector3(x * spaceBetweenRows, 0, z * spaceBetweenTiles + rowOffset); //y=hightValue
                tileInstance = Instantiate(tilePrefab, positionOfTiles, Quaternion.Euler(90f, 90f, -90f)); //y=0
                    Tile tile = tileInstance.GetComponent<Tile>();
                    tiles[x, z] = tile;


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


    void AssignRowNeighbors()
    {
        for (int x = 0; x < rows; x++)
        {
            for (int z = 0; z < columns; z++)
            {
                Tile tile = tiles[x, z];
                if (tile == null) continue;


                tile.sameRowNeighbors.Clear();
                tile.upperRowNeighbors.Clear();

                for (int i = 0; i < columns; i++)
                {
                    if (i != z && tiles[x, i] != null)
                        tile.sameRowNeighbors.Add(tiles[x, i]);
                }


                int upperRow = x + 1;

                if (upperRow < rows)
                {
                    for (int i = 0; i < columns; i++)
                    {
                        if (tiles[upperRow, i] != null)
                            tile.upperRowNeighbors.Add(tiles[upperRow, i]);
                    }
                }
            }
        }
    }
}
