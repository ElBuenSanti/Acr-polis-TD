using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RadialMenuUI : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private Canvas radialCanvas;
    [SerializeField] private RectTransform radialPanel;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Vector2 screenOffset = new Vector2(0f, 120f);

    [Header("Left Panel")]
    [SerializeField] private ShopUI shopUI;
    [SerializeField] private ConstructionDetailsUI detailsUI;

    [Header("Database")]
    [SerializeField] private ConstructionDatabase database;

    [Header("Highlights")]
    [SerializeField] private Image aphroditeHighlight;
    [SerializeField] private Image aresHighlight;
    [SerializeField] private Image hephaestusHighlight;
    [SerializeField] private Image sellHighlight;

    [Header("Cost Texts")]
    [SerializeField] private TextMeshProUGUI aphroditeCostText;
    [SerializeField] private TextMeshProUGUI aresCostText;
    [SerializeField] private TextMeshProUGUI hephaestusCostText;
    [SerializeField] private TextMeshProUGUI sellCostText;

    [Header("Level Texts")]
    [SerializeField] private TextMeshProUGUI aphroditeLevelText;
    [SerializeField] private TextMeshProUGUI aresLevelText;
    [SerializeField] private TextMeshProUGUI hephaestusLevelText;
    [SerializeField] private TextMeshProUGUI sellLevelText;

    [Header("Selection")]
    [SerializeField] private RadialOption selectedOption = RadialOption.None;

    [Header("Resources God Icon")]
    [SerializeField] private Image aphroditeCostIcon;
    [SerializeField] private Image aresCostIcon;
    [SerializeField] private Image hephaestusCostIcon;
    [SerializeField] private Image sellCostIcon;

    private ConstructionController selectedConstruction;

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (shopUI == null)
            shopUI = FindAnyObjectByType<ShopUI>();

        if (detailsUI == null)
            detailsUI = FindAnyObjectByType<ConstructionDetailsUI>();

        if (database == null)
            database = FindAnyObjectByType<ConstructionDatabase>();
    }

    private void Start()
    {
        Close();
    }

    public void Open(ConstructionController construction)
    {
        selectedConstruction = construction;
        selectedOption = RadialOption.None;

        MoveRadialToConstruction();

        if (radialCanvas != null)
            radialCanvas.enabled = true;

        if (shopUI != null)
            shopUI.ShowDetailsMode();

        if (detailsUI != null)
            detailsUI.Clear();

        RefreshLevelTexts();
        RefreshVisuals();
    }

    public void Close()
    {
        selectedConstruction = null;
        selectedOption = RadialOption.None;

        if (radialCanvas != null)
            radialCanvas.enabled = false;

        if (shopUI != null)
            shopUI.ShowCardsMode();

        if (detailsUI != null)
            detailsUI.Clear();

        RefreshVisuals();
    }

    public void ReadStick(Vector2 input)
    {
        if (radialCanvas == null || !radialCanvas.enabled)
            return;

        if (input.magnitude < 0.55f)
            return;

        RadialOption newOption;

        if (Mathf.Abs(input.x) > Mathf.Abs(input.y))
        {
            newOption = input.x > 0 ? RadialOption.Ares : RadialOption.Hephaestus;
        }
        else
        {
            newOption = input.y > 0 ? RadialOption.Aphrodite : RadialOption.Sell;
        }

        if (newOption == selectedOption)
            return;

        selectedOption = newOption;

        if (detailsUI != null)
            detailsUI.ShowDetails(selectedConstruction, selectedOption);

        RefreshVisuals();
    }

    public void Confirm()
    {
        if (selectedConstruction == null)
            return;

        if (selectedOption == RadialOption.None)
            return;

        if (IsOptionLocked(selectedOption))
        {
            Debug.Log("This upgrade path is locked.");
            return;
        }

        switch (selectedOption)
        {
            case RadialOption.Aphrodite:
                selectedConstruction.SetSelectedGod(GodType.Aphrodite);
                selectedConstruction.Upgrade();
                break;

            case RadialOption.Ares:
                selectedConstruction.SetSelectedGod(GodType.Ares);
                selectedConstruction.Upgrade();
                break;

            case RadialOption.Hephaestus:
                selectedConstruction.SetSelectedGod(GodType.Hephaestus);
                selectedConstruction.Upgrade();
                break;

            case RadialOption.Sell:
                Debug.Log("Sell is not implemented yet.");
                break;
        }

        Close();
        GameStateController.Instance.SetState(GameState.MapIdle);
    }

    private void MoveRadialToConstruction()
    {
        if (selectedConstruction == null || radialPanel == null || mainCamera == null)
            return;

        Vector3 screenPosition = mainCamera.WorldToScreenPoint(selectedConstruction.transform.position);
        radialPanel.position = screenPosition + (Vector3)screenOffset;
    }

    private void RefreshVisuals()
    {
        SetHighlight(aphroditeHighlight, selectedOption == RadialOption.Aphrodite);
        SetHighlight(aresHighlight, selectedOption == RadialOption.Ares);
        SetHighlight(hephaestusHighlight, selectedOption == RadialOption.Hephaestus);
        SetHighlight(sellHighlight, selectedOption == RadialOption.Sell);

        UpdateCostText(aphroditeCostText, RadialOption.Aphrodite);
        UpdateCostText(aresCostText, RadialOption.Ares);
        UpdateCostText(hephaestusCostText, RadialOption.Hephaestus);
        UpdateCostText(sellCostText, RadialOption.Sell);

        SetCostIconVisible(aphroditeCostIcon, selectedOption == RadialOption.Aphrodite);
        SetCostIconVisible(aresCostIcon, selectedOption == RadialOption.Ares);
        SetCostIconVisible(hephaestusCostIcon, selectedOption == RadialOption.Hephaestus);
        SetCostIconVisible(sellCostIcon, selectedOption == RadialOption.Sell);
    }

    private void RefreshLevelTexts()
    {
        UpdateLevelText(aphroditeLevelText, RadialOption.Aphrodite);
        UpdateLevelText(aresLevelText, RadialOption.Ares);
        UpdateLevelText(hephaestusLevelText, RadialOption.Hephaestus);

        if (sellLevelText != null)
            sellLevelText.text = "Sell";
    }

    private void UpdateLevelText(TextMeshProUGUI text, RadialOption option)
    {
        if (text == null)
            return;

        if (IsOptionLocked(option))
        {
            text.text = "Locked";
            return;
        }

        ConstructionData currentData = GetCurrentData();

        if (currentData == null)
        {
            text.text = "";
            return;
        }

        GodType god = GetGodFromOption(option);
        int nextLevel = currentData.level + 1;

        ConstructionData nextData = database.GetData(currentData.type, god, nextLevel);

        if (nextData == null)
        {
            text.text = "MAX";
            return;
        }

        text.text = "Level " + nextLevel;
    }

    private void UpdateCostText(TextMeshProUGUI text, RadialOption option)
    {
        if (text == null)
            return;

        bool shouldShow = selectedOption == option;
        text.gameObject.SetActive(shouldShow);

        if (!shouldShow)
            return;

        if (option == RadialOption.Sell)
        {
            text.text = "$";
            return;
        }

        if (IsOptionLocked(option))
        {
            text.text = "Locked";
            return;
        }

        ConstructionData currentData = GetCurrentData();

        if (currentData == null)
        {
            text.text = "";
            return;
        }

        GodType god = GetGodFromOption(option);
        int nextLevel = currentData.level + 1;

        ConstructionData nextData = database.GetData(currentData.type, god, nextLevel);

        if (nextData == null)
        {
            text.text = "MAX";
            return;
        }

        text.text = GetCostText(nextData);
    }

    private void SetCostIconVisible(Image image, bool active)
    {
        if (image != null)
            image.gameObject.SetActive(active);
    }
    private bool IsOptionLocked(RadialOption option)
    {
        if (option == RadialOption.Sell)
            return false;

        ConstructionData currentData = GetCurrentData();

        if (currentData == null)
            return false;

        if (currentData.god == GodType.Base)
            return false;

        GodType optionGod = GetGodFromOption(option);

        return currentData.god != optionGod;
    }

    private ConstructionData GetCurrentData()
    {
        if (selectedConstruction == null)
            return null;

        BaseConstruction baseConstruction = selectedConstruction.GetComponent<BaseConstruction>();

        if (baseConstruction == null)
            return null;

        return baseConstruction.Data;
    }

    private GodType GetGodFromOption(RadialOption option)
    {
        switch (option)
        {
            case RadialOption.Aphrodite:
                return GodType.Aphrodite;

            case RadialOption.Ares:
                return GodType.Ares;

            case RadialOption.Hephaestus:
                return GodType.Hephaestus;

            default:
                return GodType.Base;
        }
    }

    private string GetCostText(ConstructionData data)
    {
        if (data.willToPay == null || data.willToPay.Count == 0)
            return "0";

        WillProduction cost = data.willToPay[0];
        return cost.amount.ToString("0");
    }

    private void SetHighlight(Image image, bool active)
    {
        if (image != null)
            image.enabled = active;
    }
}