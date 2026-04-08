using UnityEngine;

// Debug helper to move focus between HUD groups
public class HUDGroupNavigationDebug : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HUDNavigationUI buildCardsNavigation;
    [SerializeField] private HUDNavigationUI actionPanelNavigation;

    private void Update()
    {
        // Focus build cards group
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (buildCardsNavigation != null)
            {
                buildCardsNavigation.SelectDefault();
            }
        }

        // Focus action panel group
        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (actionPanelNavigation != null)
            {
                actionPanelNavigation.SelectDefault();
            }
        }
    }
}