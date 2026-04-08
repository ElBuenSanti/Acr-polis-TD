using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Handles default focus for HUD groups like build cards or action buttons
public class HUDNavigationUI : MonoBehaviour
{
    [Header("Default Selection")]
    [SerializeField] private Selectable defaultSelected;

    private void OnEnable()
    {
        StartCoroutine(SelectDefaultAtEndOfFrame());
    }

    private IEnumerator SelectDefaultAtEndOfFrame()
    {
        yield return null;
        yield return new WaitForEndOfFrame();
        SelectDefault();
    }

    // Force selection of the configured HUD element
    public void SelectDefault()
    {
        if (defaultSelected == null || EventSystem.current == null)
        {
            return;
        }

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(defaultSelected.gameObject);
        defaultSelected.Select();

        Debug.Log("HUDNavigationUI selected: " + defaultSelected.name);
    }
}