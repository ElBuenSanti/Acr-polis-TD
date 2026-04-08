using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Handles a single upgrade option inside the radial menu
public class RadialOptionButtonUI : MonoBehaviour
{
    [Header("Option Info")]
    [SerializeField] private UpgradeBranchType branchType = UpgradeBranchType.BranchA;
    [SerializeField] private string optionTitle = "Option";

    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Image backgroundImage;

    [Header("Visual Colors")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = new Color(0.75f, 0.75f, 0.75f, 1f);
    [SerializeField] private Color lockedColor = new Color(0.4f, 0.4f, 0.4f, 1f);
    [SerializeField] private Color maxedColor = new Color(0.65f, 0.65f, 0.65f, 1f);
    [SerializeField] private Color notAffordableColor = new Color(0.75f, 0.5f, 0.5f, 1f);

    public UpgradeBranchType BranchType => branchType;
    public string OptionTitle => optionTitle;

    private RadialMenuUI radialMenuUI;
    private bool isLocked;
    private bool isMaxed;
    private bool isAffordable = true;

    private void Start()
    {
        if (titleText != null)
        {
            titleText.text = optionTitle;
        }

        if (button != null)
        {
            button.onClick.AddListener(OnOptionPressed);
        }

        SetSelected(false);
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnOptionPressed);
        }
    }

    // Assign the parent radial menu controller
    public void SetRadialMenu(RadialMenuUI menuUI)
    {
        radialMenuUI = menuUI;
    }

    // Called when the option button is pressed
    private void OnOptionPressed()
    {
        if (isLocked || isMaxed || !isAffordable)
        {
            Debug.Log("This branch cannot be selected.");
            return;
        }

        if (radialMenuUI != null)
        {
            radialMenuUI.SelectOption(this);
        }
    }

    // Update this option visual state
    public void SetSelected(bool isSelected)
    {
        if (backgroundImage == null)
        {
            return;
        }

        if (isLocked)
        {
            backgroundImage.color = lockedColor;
        }
        else if (isMaxed)
        {
            backgroundImage.color = maxedColor;
        }
        else if (!isAffordable)
        {
            backgroundImage.color = notAffordableColor;
        }
        else
        {
            backgroundImage.color = isSelected ? selectedColor : normalColor;
        }
    }

    // Update this option lock state
    public void SetLocked(bool locked)
    {
        isLocked = locked;
        RefreshInteractableState();
        SetSelected(false);
    }

    // Update this option maxed state
    public void SetMaxed(bool maxed)
    {
        isMaxed = maxed;
        RefreshInteractableState();
        SetSelected(false);
    }

    // Update affordability state
    public void SetAffordable(bool affordable)
    {
        isAffordable = affordable;
        RefreshInteractableState();
        SetSelected(false);
    }

    // Update the displayed level text
    public void SetLevelText(string textValue)
    {
        if (levelText != null)
        {
            levelText.text = textValue;
        }
    }

    // Update the displayed cost text
    public void SetCostText(string textValue)
    {
        if (costText != null)
        {
            costText.text = textValue;
        }
    }

    // Refresh button interactable state
    private void RefreshInteractableState()
    {
        if (button != null)
        {
            button.interactable = !isLocked && !isMaxed && isAffordable;
        }
    }
}