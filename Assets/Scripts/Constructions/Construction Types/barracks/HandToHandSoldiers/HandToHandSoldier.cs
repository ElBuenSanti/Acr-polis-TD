using JetBrains.Annotations;
using UnityEngine;

public class HandToHandSoldier : MonoBehaviour
{
    public Barracks barrack;
    void Start()
    {
        SoldierCreated();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SoldierCreated()
    {
        //EventManager.TriggerEvent("soldierCreated", 100);
        Debug.Log("Soldado");
    }
}
