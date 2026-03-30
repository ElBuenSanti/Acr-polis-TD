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

