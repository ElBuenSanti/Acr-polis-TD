using UnityEngine;
using System.Collections;

public class Barracks : ConstructionBehaviour
{
    public Pooling soldierPooling;
    public GameObject soldierPrefab;
    private float timeToSpawnSoldiers= 10f;
    

    public override void Start()
    {
        base.Start();
        soldierPooling = FindAnyObjectByType<Pooling>();
        StartCoroutine(SpawnLoop());
    }

    //Coorutinas
    IEnumerator SpawnLoop()
    {
        while (isActiveAndEnabled)
        {
            GameObject newSoldier = soldierPooling.CreateObject(soldierPrefab, transform);

            if (newSoldier.TryGetComponent<HandToHandSoldier>(out var soldier))
            {
                soldier.barrack = this;
                soldier.SetTeam(Team.Ally);
            }

            /*
            GameObject newSoldier = soldierPooling.CreateObject(soldierPrefab, transform);
            newSoldier.GetComponent<HandToHandSoldier>().barrack = this;
            */

            yield return new WaitForSeconds(timeToSpawnSoldiers);
        }
    }


}