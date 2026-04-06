using UnityEngine;
using System.Collections;

public class Barracks : MonoBehaviour, IConstructable
{
    public Pooling soldierPooling;
    public GameObject soldierPrefab;
    private float timeToSpawnSoldiers= 10f;

    void Start()
    {
        soldierPooling = FindAnyObjectByType<Pooling>();
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            GameObject newSoldier = soldierPooling.CreateObject(soldierPrefab, transform);
            newSoldier.GetComponent<HandToHandSoldier>().barrack = this;

            yield return new WaitForSeconds(timeToSpawnSoldiers);
        }
    }

    public void ReceiveDamage() { }
    public void Recover() { }
    public void Upgrade() { }
}