using UnityEngine;
using UnityEngine.EventSystems;

// Debug helper to print the currently selected UI object
public class UIDebugSelectedLogger : MonoBehaviour
{
    private GameObject lastSelectedObject;

    private void Update()
    {
        if (EventSystem.current == null)
        {
            return;
        }

        GameObject currentSelectedObject = EventSystem.current.currentSelectedGameObject;

        if (currentSelectedObject != lastSelectedObject)
        {
            lastSelectedObject = currentSelectedObject;

            if (currentSelectedObject != null)
            {
                Debug.Log("Current selected UI object: " + currentSelectedObject.name);
            }
            else
            {
                Debug.Log("Current selected UI object: None");
            }
        }
    }
}