using UnityEngine;

public class Temple : ConstructionBehaviour
{
    //private const string nameOfEvent = "soldierCreated";
    //const sring que no cambia jamás, variable de solo lectura.


    public override void Start()
    {
        base.Start();
    }

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
}
