using UnityEngine;
using System.Collections;

public class Wall : BaseConstruction
{

    private bool canRepair = false;
    private Coroutine repairCooldown;


    //Initialization of Wall
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if (repairCooldown != null)
            StopCoroutine(repairCooldown);
    }

    public override void Initialize(ConstructionData newData)
    {
        base.Initialize(newData);
        timeToBeDestroyed = 2f;

        canRepair = true;

    }


    //Functions

    //Repair Wall
    public void RepairGroup() 
    {
        if (!canRepair)
        {
            Debug.Log("Aún no se puede reparar");
            return;
        }
        var controller = GetComponent<ConstructionController>();

        if (controller == null || controller.group == null)
            return;

        foreach (var w in data.bonusWillToPay)
        {
            if (!WillManager.Instance.SpendMoney(w.type, w.amount))
            {
                Debug.Log("No te alcanza para reparar");
                return;
            }
        }
        foreach (var member in controller.group.members)
        {
            var wall = member.GetComponent<Wall>();

            if (wall != null)
                wall.HealToMax();
        }

        Debug.Log("Muralla completamente reparada");
        canRepair = false;

        if (repairCooldown != null)
            StopCoroutine(repairCooldown);

        repairCooldown = StartCoroutine(RepairCooldown());
    }

    void HealToMax()
    {
        resistance = data.resistance;
    }




    //Cooroutines
    IEnumerator RepairCooldown()
    {
        Debug.Log("Reparación en cooldown");

        yield return new WaitForSeconds(timeToSpawnAditaments);

        canRepair = true;
        repairCooldown = null;

        Debug.Log("Muralla lista para repararse");
    }
}
