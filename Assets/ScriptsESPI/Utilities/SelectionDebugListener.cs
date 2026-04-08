using UnityEngine;

// Debug listener used to test selection events
public class SelectionDebugListener : MonoBehaviour
{
    private void OnEnable()
    {
        GameEvents.StartListening(EventNames.StructureSelected, OnStructureSelected);
        GameEvents.StartListening(EventNames.StructureDeselected, OnStructureDeselected);
    }

    private void OnDisable()
    {
        GameEvents.StopListening(EventNames.StructureSelected, OnStructureSelected);
        GameEvents.StopListening(EventNames.StructureDeselected, OnStructureDeselected);
    }

    // Called when a structure is selected
    private void OnStructureSelected(object eventData)
    {
        ISelectable selectable = eventData as ISelectable;

        if (selectable == null)
        {
            return;
        }

        Debug.Log("Listener received selected structure: " + selectable.GetDisplayName());
    }

    // Called when a structure is deselected
    private void OnStructureDeselected(object eventData)
    {
        ISelectable selectable = eventData as ISelectable;

        if (selectable == null)
        {
            return;
        }

        Debug.Log("Listener received deselected structure: " + selectable.GetDisplayName());
    }
}