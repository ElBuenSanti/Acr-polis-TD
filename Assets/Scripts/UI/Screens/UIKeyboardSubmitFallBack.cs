using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Fallback submit for selected UI buttons using keyboard
public class UIKeyboardSubmitFallback : MonoBehaviour
{
    private void Update()
    {
        if (EventSystem.current == null)
        {
            return;
        }

        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;

        if (selectedObject == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Space))
        {
            Button selectedButton = selectedObject.GetComponent<Button>();

            if (selectedButton != null && selectedButton.interactable)
            {
                selectedButton.onClick.Invoke();
            }
        }
    }
}