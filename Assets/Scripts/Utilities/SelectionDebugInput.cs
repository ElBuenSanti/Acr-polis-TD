using UnityEngine;

// Debug input used to test structure selection
public class SelectionDebugInput : MonoBehaviour
{
    [SerializeField] private SelectionSystem selectionSystem;
    [SerializeField] private SelectableStructure templeStructure;
    [SerializeField] private SelectableStructure barracksStructure;
    [SerializeField] private SelectableStructure defenseStructure;

    private void Update()
    {
        if (selectionSystem == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            selectionSystem.SelectStructure(templeStructure);
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            selectionSystem.SelectStructure(barracksStructure);
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            selectionSystem.SelectStructure(defenseStructure);
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            selectionSystem.ClearSelection();
        }
    }
}