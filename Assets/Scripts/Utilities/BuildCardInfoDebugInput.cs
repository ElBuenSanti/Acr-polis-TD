using UnityEngine;

// Debug input used to test build card selection and info popup
public class BuildCardInfoDebugInput : MonoBehaviour
{
    [SerializeField] private BuildCardInfoUI buildCardInfoUI;
    [SerializeField] private BuildCardSelectionMemory buildCardSelectionMemory;

    private void Update()
    {
        if (buildCardInfoUI == null || buildCardSelectionMemory == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            buildCardSelectionMemory.SetSelectedBuildType(BuildType.Temple);
            Debug.Log("Debug: Temple selected.");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            buildCardSelectionMemory.SetSelectedBuildType(BuildType.Barracks);
            Debug.Log("Debug: Barracks selected.");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            buildCardSelectionMemory.SetSelectedBuildType(BuildType.Defense);
            Debug.Log("Debug: Defense selected.");
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (buildCardSelectionMemory.HasSelectedBuildCard)
            {
                buildCardInfoUI.OpenForBuildType(buildCardSelectionMemory.CurrentBuildType);
            }
            else
            {
                Debug.Log("Debug: No build card selected for info popup.");
            }
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            buildCardInfoUI.ClosePopup();
        }
    }
}