using System.Collections.Generic;
using UnityEngine;

public class Pooling : MonoBehaviour
{
    private Dictionary<GameObject, List<GameObject>> poolingDictionary;

    void Awake()
    {
        poolingDictionary = new Dictionary<GameObject, List<GameObject>>();
    }

    public GameObject CreateObject(GameObject prefab, Transform spawnPoint)
    {
        if (spawnPoint == null)
        {
            Debug.LogError("SpawnPoint es NULL");
            return null;
        }

        if (!poolingDictionary.ContainsKey(prefab))
        {
            poolingDictionary[prefab] = new List<GameObject>();
        }

        List<GameObject> objectList = poolingDictionary[prefab];


        for (int i = 0; i < objectList.Count; i++)
        {
            if (!objectList[i].activeInHierarchy)
            {
                GameObject obj = objectList[i];

                /*
                obj.SetActive(true);

                obj.transform.SetParent(null); //antes SpawnPoint

                obj.transform.position = spawnPoint.position;
                obj.transform.rotation = Quaternion.identity;
                */
                obj.transform.SetParent(null);
                obj.transform.position = spawnPoint.position;
                obj.transform.rotation = Quaternion.identity;

                obj.SetActive(true);

                return obj;
            }
        }


        //GameObject newObject = Instantiate(prefab, spawnPoint.position, Quaternion.identity);

        GameObject newObject = Instantiate(prefab);
        newObject.transform.SetParent(null);
        newObject.transform.position = spawnPoint.position;
        newObject.transform.rotation = Quaternion.identity;
        objectList.Add(newObject);

        return newObject;
    }

}

    /*

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

        GameObject newObjectToInstantiate = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
        objectList.Add(newObjectToInstantiate);

        return newObjectToInstantiate;
    }
}
    */

