using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

// Handles the popup that shows information about the currently selected build card
public class BuildCardInfoUI : MonoBehaviour
{
    [Header("Content Root")]
    [SerializeField] private GameObject contentRoot;

    [Header("Text References")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text typeText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text roleText;

    [Header("Data References")]
    [SerializeField] private BuildCardInfoDatabase buildCardInfoDatabase;

    [Header("Optional References")]
    [SerializeField] private GameObject pauseMenuContent;
    [SerializeField] private GameObject victoryPanelContent;
    [SerializeField] private GameObject defeatPanelContent;
    [SerializeField] private GameObject blessingSelectionContent;

    public bool IsOpen { get; private set; }

    private BuildType currentBuildType;

    private void Start()
    {
        SetPopupVisible(false);
    }

    // Open the popup for a specific build card
    public void OpenForBuildType(BuildType buildType)
    {
        currentBuildType = buildType;
        ApplyBuildInfo(buildType);
        HideOtherOverlayContent();
        SetPopupVisible(true);
        SelectDefaultIfPossible();

        Debug.Log("BuildCardInfoUI opened for: " + buildType);
    }

    // Close the popup
    public void ClosePopup()
    {
        SetPopupVisible(false);

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        Debug.Log("BuildCardInfoUI closed.");
    }

    // Toggle popup for a specific build card
    public void ToggleForBuildType(BuildType buildType)
    {
        if (IsOpen && currentBuildType == buildType)
        {
            ClosePopup();
            return;
        }

        OpenForBuildType(buildType);
    }

    // Apply the card info depending on the selected build type
    private void ApplyBuildInfo(BuildType buildType)
    {
        if (buildCardInfoDatabase == null)
        {
            Debug.LogWarning("BuildCardInfoUI: BuildCardInfoDatabase reference is missing.");
            SetFallbackTexts();
            return;
        }

        bool hasData = buildCardInfoDatabase.TryGetBuildCardInfo(buildType, out BuildCardInfoData data);

        if (!hasData || data == null)
        {
            Debug.LogWarning("BuildCardInfoUI: No data found for " + buildType);
            SetFallbackTexts();
            return;
        }

        SetTexts(
            data.title,
            data.type,
            data.description,
            data.cost,
            data.role
        );
    }

    private void SetFallbackTexts()
    {
        SetTexts(
            "Unknown Card",
            "Unknown Type",
            "No description available.",
            "Cost: N/A",
            "Role: N/A"
        );
    }

    // Set all visible text fields
    private void SetTexts(string title, string type, string description, string cost, string role)
    {
        if (titleText != null)
        {
            titleText.text = title;
        }

        if (typeText != null)
        {
            typeText.text = type;
        }

        if (descriptionText != null)
        {
            descriptionText.text = description;
        }

        if (costText != null)
        {
            costText.text = cost;
        }

        if (roleText != null)
        {
            roleText.text = role;
        }
    }

    // Hide other fullscreen overlays when this popup opens
    private void HideOtherOverlayContent()
    {
        if (pauseMenuContent != null)
        {
            pauseMenuContent.SetActive(false);
        }

        if (victoryPanelContent != null)
        {
            victoryPanelContent.SetActive(false);
        }

        if (defeatPanelContent != null)
        {
            defeatPanelContent.SetActive(false);
        }

        if (blessingSelectionContent != null)
        {
            blessingSelectionContent.SetActive(false);
        }
    }

    // Show or hide the popup
    private void SetPopupVisible(bool isVisible)
    {
        IsOpen = isVisible;

        if (contentRoot != null)
        {
            contentRoot.SetActive(isVisible);
        }
    }

    // Select the default button inside the popup if available
    private void SelectDefaultIfPossible()
    {
        if (contentRoot == null)
        {
            return;
        }

        MenuNavigationUI navigationUI = contentRoot.GetComponent<MenuNavigationUI>();

        if (navigationUI != null)
        {
            navigationUI.SelectDefault();
        }
    }
}