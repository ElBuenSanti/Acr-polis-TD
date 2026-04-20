using UnityEngine;

public class GodDebugInput : MonoBehaviour
{
    void Update()
    {
        var selected = BuildingManager.Instance.selectedConstruction;

        if (selected == null)
            return;


        if (Input.GetKeyDown(KeyCode.A))
        {
            selected.SetSelectedGod(GodType.Ares);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            selected.SetSelectedGod(GodType.Aphrodite);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            selected.SetSelectedGod(GodType.Hephaestus);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            selected.Upgrade();
        }
    }
}