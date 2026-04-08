using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Handles default menu focus for keyboard or controller navigation
public class MenuNavigationUI : MonoBehaviour
{
    [Header("Default Selection")]
    [SerializeField] private Selectable defaultSelected;

    private void OnEnable()
    {
        StartCoroutine(SelectDefaultWithDelay());
    }

    // Select the default UI element after the UI is fully enabled
    private IEnumerator SelectDefaultWithDelay()
    {
        yield return null;
        yield return null;
        yield return new WaitForEndOfFrame();

        SelectDefault();
    }

    // Select the default UI element when this menu becomes active
    public void SelectDefault()
    {
        if (defaultSelected == null || EventSystem.current == null)
        {
            Debug.LogWarning("MenuNavigationUI: Missing defaultSelected or EventSystem.");
            return;
        }

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(defaultSelected.gameObject);
        defaultSelected.Select();

        Debug.Log("MenuNavigationUI selected: " + defaultSelected.name);
    }
}