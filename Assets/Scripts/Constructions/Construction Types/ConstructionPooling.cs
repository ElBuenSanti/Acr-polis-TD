using System.Collections.Generic;
using UnityEngine;

public class ConstructionPooling : MonoBehaviour
{
    private Dictionary<GameObject, List<GameObject>> poolingDictionary;

    void Awake()
    {
        poolingDictionary = new Dictionary<GameObject, List<GameObject>>();
    }

    public GameObject CreateObject(GameObject prefab, Transform spawnPoint)
    {
        if (!poolingDictionary.ContainsKey(prefab))
        {
            poolingDictionary[prefab] = new List<GameObject>();
        }

        List<GameObject> objectList = poolingDictionary[prefab];


        for (int i = 0; i < objectList.Count; i++)
        {
            if (!objectList[i].activeInHierarchy)
            {
                objectList[i].SetActive(true);
                objectList[i].transform.position = spawnPoint.position;
                return objectList[i];
            }
        }

        GameObject newObj = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        objectList.Add(newObj);

        return newObj;
    }
}

/*public class SoldierPooling : MonoBehaviour
{
    public List<GameObject> createdSoldiers;
    public GameObject soldiersToCreate;
    void Start()
    {
        createdSoldiers = new List<GameObject>();
    }

    public GameObject CreateSoldier(Transform spawnPoint)
    {
        GameObject pivotObject;

        for (int i = 0; i < createdSoldiers.Count; i++)
        {
            pivotObject = createdSoldiers[i];

            if (!pivotObject.activeInHierarchy)
            {
                pivotObject.SetActive(true);
                pivotObject.transform.position = spawnPoint.position;
                return pivotObject;
            }
        }

        GameObject newSoldier =Instantiate(soldiersToCreate, spawnPoint.position, Quaternion.identity);
        createdSoldiers.Add(newSoldier);
        return newSoldier;
    }

}
*/
