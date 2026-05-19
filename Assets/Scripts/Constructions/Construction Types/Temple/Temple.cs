using System;
using UnityEngine;

public class Temple : Plaza
{
    public static Action<GodType> OnGodSelected;
    //private ConstructionController controller;

    public static event Action OnTempleDestruction;


    //Temple Initialization
    protected override void Awake()
    {
        base.Awake();
        //controller = GetComponent<ConstructionController>();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public override void Initialize(ConstructionData newData)
    {
        base.Initialize(newData);
    }

    //Functions

    //When selecting God, it defines type of final boss...
    public void NotifyGodSelected(GodType god)
    {
        Debug.Log("El templo fue consagrado al Dios: " + god);
        OnGodSelected?.Invoke(god);
    }


    //When the temple is destroyed, it anounces the defeat of the player
    public override void OnDestruction()
    {
        base.OnDestruction();
        Debug.Log("Has perdido");
        OnTempleDestruction?.Invoke(); 
    }

    private bool blessingChosen;

    public bool HasBlessingChosen()
    {
        return blessingChosen;
    }

    public void MarkBlessingChosen()
    {
        blessingChosen = true;
    }
}

