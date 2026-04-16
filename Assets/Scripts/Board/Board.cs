using UnityEngine;
using UnityEngine.AI; //

public class Board : MonoBehaviour
{
    [SerializeField] private GameObject tilePrefab;
    [SerializeField] private int rows = 10;
    [SerializeField] private int columns = 6;

    [SerializeField] private NavMeshSurface navSurface; 
    private string navMeshObject = "NavMeshSurfaceBoard"; 
    private string layerMaskName = "Tiles"; 

    private float spaceBetweenTiles = 1.75f;
    private float spaceBetweenRows = 1.5f;
    private float rowOffset = 0f;

    //private float hightValue;

    void Start()
    {
        GameObject navObj = new GameObject(navMeshObject);
        navSurface = navObj.AddComponent<NavMeshSurface>();
        navSurface.collectObjects = CollectObjects.All;
        navSurface.layerMask = LayerMask.GetMask(layerMaskName); 
        

        GenerateBoard();
        navSurface.BuildNavMesh(); 
    }
    void GenerateBoard()
    {
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
                tileInstance = Instantiate(tilePrefab, positionOfTiles, Quaternion.Euler(90f, 0f, -90f)); 
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
