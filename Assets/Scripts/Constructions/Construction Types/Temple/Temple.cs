using UnityEngine;

public class Temple : Plaza
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public override void Initialize(ConstructionData newData)
    {
        base.Initialize(newData);
    }

    protected override void SpawnWill()
    {
        base.SpawnWill();
        Debug.Log("Pero de templo");
    }
}


//private const string nameOfEvent = "soldierCreated";
//const sring que no cambia jamás, variable de solo lectura.



/*
private void OnEnable()
{
    EventManager.StartListening(nameOfEvent, OnSoliderCreated);
}

private void OnDisable()
{
    EventManager.StopListening(nameOfEvent, OnSoliderCreated);
}

public void OnSoliderCreated(object referenceOfObject) //no hay restruicciones con object
{
    int getNumber = (int)referenceOfObject;
    Debug.Log(getNumber);
}
*/