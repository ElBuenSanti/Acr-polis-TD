using UnityEngine;
using System.Collections;

public class Wall : BaseConstruction
{

    private bool canRepair = false;
    private Coroutine repairLoop;
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
        timeToBeDestroyed = 2f;

        if (repairLoop != null)
            StopCoroutine(repairLoop);

        repairLoop = StartCoroutine(RepairLoop());
    }

    public void RepairGroup() //para reparar  muro
    {
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
    }

    void HealToMax()
    {
        resistance = data.resistance;
    }

    //coorutina
    IEnumerator RepairLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeToSpawnAditaments);

            canRepair = true;

            Debug.Log("Muralla lista para repararse");
        }
    }
}
