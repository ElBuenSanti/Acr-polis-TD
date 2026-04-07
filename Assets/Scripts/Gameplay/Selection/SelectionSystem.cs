using UnityEngine;

// Handles structure selection and deselection
public class SelectionSystem : MonoBehaviour
{
    public ISelectable CurrentSelected { get; private set; }

    // Select a new structure
    public void SelectStructure(ISelectable newSelection)
    {
        if (newSelection == null)
        {
            return;
        }

        if (CurrentSelected == newSelection)
        {
            return;
        }

        ClearSelection(false);

        CurrentSelected = newSelection;
        CurrentSelected.OnSelected();

        Debug.Log("Current selected structure: " + CurrentSelected.GetDisplayName());
        GameEvents.TriggerEvent(EventNames.StructureSelected, newSelection);

        if (GameStateManager.Instance != null)
        {
            GameState currentState = GameStateManager.Instance.CurrentState;

            // Only switch to StructureSelected when not in combat
            if (currentState == GameState.Preparation)
            {
                GameStateManager.Instance.ChangeState(GameState.StructureSelected);
            }
        }
    }

    // Clear the current structure selection
    public void ClearSelection()
    {
        ClearSelection(true);
    }

    // Clear selection with optional state restore
    private void ClearSelection(bool restoreState)
    {
        if (CurrentSelected == null)
        {
            return;
        }

        ISelectable previousSelection = CurrentSelected;

        CurrentSelected.OnDeselected();

        Debug.Log("Selection cleared: " + previousSelection.GetDisplayName());
        GameEvents.TriggerEvent(EventNames.StructureDeselected, previousSelection);

        CurrentSelected = null;

        if (restoreState && GameStateManager.Instance != null)
        {
            GameState currentState = GameStateManager.Instance.CurrentState;

            if (currentState == GameState.StructureSelected)
            {
                GameStateManager.Instance.ChangeState(GameState.Preparation);
            }
        }
    }

    // Check if there is an active selected structure
    public bool HasSelection()
    {
        return CurrentSelected != null;
    }
}