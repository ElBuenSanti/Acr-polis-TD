using System;
using UnityEngine;

public class Temple : Plaza
{
    public static Action<GodType> OnGodSelected;
    private ConstructionController controller;

    public static event Action OnTempleDestruction;

    protected override void Awake()
    {
        base.Awake();
        controller = GetComponent<ConstructionController>();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public override void Initialize(ConstructionData newData)
    {
        base.Initialize(newData);
    }

    //Funciones

    public void NotifyGodSelected(GodType god)
    {
        Debug.Log("Templo anuncia dios: " + god);
        OnGodSelected?.Invoke(god);
    }

    protected override void SpawnWill()
    {
        base.SpawnWill();
        Debug.Log("Pero de templo");
    }

    public override void OnDestruction()
    {
        base.OnDestruction();
        Debug.Log("Has perdido");
        OnTempleDestruction?.Invoke();
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