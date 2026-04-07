using TMPro;
using UnityEngine;

// Handles the left smart panel UI for the currently selected structure
public class SmartPanelUI : MonoBehaviour
{
    [Header("Content Root")]
    [SerializeField] private GameObject contentRoot;

    [Header("Text References")]
    [SerializeField] private TMP_Text structureNameText;
    [SerializeField] private TMP_Text structureTypeText;
    [SerializeField] private TMP_Text structureDescriptionText;
    [SerializeField] private TMP_Text structureStatsText;

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

    private void Start()
    {
        ClearPanel();
        SetContentVisible(false);
    }

    // Called when a structure is selected
    private void OnStructureSelected(object eventData)
    {
        SelectableStructure selectedStructure = eventData as SelectableStructure;

        if (selectedStructure == null)
        {
            return;
        }

        structureNameText.text = selectedStructure.GetDisplayName();
        structureTypeText.text = selectedStructure.GetStructureType();
        structureDescriptionText.text = selectedStructure.GetDescription();
        structureStatsText.text = selectedStructure.GetStats();

        SetContentVisible(true);
    }

    // Called when a structure is deselected
    private void OnStructureDeselected(object eventData)
    {
        ClearPanel();
        SetContentVisible(false);
    }

    // Reset the panel text values
    private void ClearPanel()
    {
        if (structureNameText != null) structureNameText.text = "";
        if (structureTypeText != null) structureTypeText.text = "";
        if (structureDescriptionText != null) structureDescriptionText.text = "";
        if (structureStatsText != null) structureStatsText.text = "";
    }

    // Show or hide the panel content
    private void SetContentVisible(bool isVisible)
    {
        if (contentRoot != null)
        {
            contentRoot.SetActive(isVisible);
        }
    }
}