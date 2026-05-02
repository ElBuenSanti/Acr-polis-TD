using System.Collections.Generic;
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

    [Header("Sell Hold")]
    [SerializeField] private float sellHoldTime = 3f;
    [SerializeField] private float refundPercent = 0.75f;
    [SerializeField] private Image sellHoldFillImage;

    [Header("Selection")]
    [SerializeField] private RadialOption selectedOption = RadialOption.None;

    private ConstructionController selectedConstruction;
    private float sellTimer;

    private void Awake()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        if (shopUI == null) shopUI = FindAnyObjectByType<ShopUI>();
        if (detailsUI == null) detailsUI = FindAnyObjectByType<ConstructionDetailsUI>();
        if (database == null) database = FindAnyObjectByType<ConstructionDatabase>();
    }

    private void Start()
    {
        Close();
    }

    private void Update()
    {
        if (radialCanvas == null || !radialCanvas.enabled)
            return;

        if (selectedOption != RadialOption.Sell)
        {
            sellTimer = 0f;
            UpdateSellHoldVisual();
            return;
        }

        if (selectedConstruction == null || IsTempleSelected())
        {
            ShowStatus("El templo no se puede vender");
            return;
        }
        
        sellTimer += Time.deltaTime;
        UpdateSellHoldVisual();

        if (sellTimer >= sellHoldTime)
        {
            SellSelectedConstruction();
            Close();
            GameStateController.Instance.SetState(GameState.MapIdle);
        }
    }

    public void Open(ConstructionController construction)
    {
        if (WaveSpawner.Instance.IsWaveRunning())
        {
            ShowStatus("No puedes mejorar durante la oleada");
            return;
        }

        selectedConstruction = construction;
        selectedOption = RadialOption.None;
        sellTimer = 0f;

        MoveRadialToConstruction();

        if (radialCanvas != null)
            radialCanvas.enabled = true;

        if (shopUI != null)
            shopUI.ShowDetailsMode();

        if (detailsUI != null)
            detailsUI.ShowBaseDetails(selectedConstruction);

        RefreshLevelTexts();
        RefreshVisuals();
        UpdateSellHoldVisual();
    }

    public void Close()
    {
        selectedConstruction = null;
        selectedOption = RadialOption.None;
        sellTimer = 0f;

        if (radialCanvas != null)
            radialCanvas.enabled = false;

        if (shopUI != null)
            shopUI.ShowCardsMode();

        if (detailsUI != null)
            detailsUI.Clear();

        RefreshVisuals();
        UpdateSellHoldVisual();
    }

    public void ReadStick(Vector2 input)
    {
        if (radialCanvas == null || !radialCanvas.enabled)
            return;

        if (input.magnitude < 0.55f)
        {
            if (selectedOption != RadialOption.None)
            {
                selectedOption = RadialOption.None;
                sellTimer = 0f;

                if (detailsUI != null)
                    detailsUI.ShowBaseDetails(selectedConstruction);

                RefreshVisuals();
                UpdateSellHoldVisual();
            }

            return;
        }

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
        sellTimer = 0f;

        if (detailsUI != null)
            detailsUI.ShowDetails(selectedConstruction, selectedOption);

        RefreshVisuals();
        UpdateSellHoldVisual();
    }

    public void Confirm()
    {

        if (WaveSpawner.Instance.IsWaveRunning())
        {
            ShowStatus("No puedes hacer esto durante la oleada");
            return;
        }

        if (selectedConstruction == null || selectedOption == RadialOption.None)
            return;

        if (IsOptionLocked(selectedOption))
        {
            ShowStatus("Este camino de mejora está bloqueado");
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
                ShowStatus("Holdea stick para vender");
                return;
        }

        Close();
        GameStateController.Instance.SetState(GameState.MapIdle);
    }

    private void MoveRadialToConstruction()
    {
        if (selectedConstruction == null || radialPanel == null || mainCamera == null)
            return;

        Vector3 screenPosition = mainCamera.WorldToScreenPoint(selectedConstruction.transform.position);
        Vector3 targetPosition = screenPosition + (Vector3)screenOffset;

        float padding = 170f;

        targetPosition.x = Mathf.Clamp(targetPosition.x, padding, Screen.width - padding);
        targetPosition.y = Mathf.Clamp(targetPosition.y, padding, Screen.height - padding);

        radialPanel.position = targetPosition;
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
    }

    private void RefreshLevelTexts()
    {
        UpdateLevelText(aphroditeLevelText, RadialOption.Aphrodite);
        UpdateLevelText(aresLevelText, RadialOption.Ares);
        UpdateLevelText(hephaestusLevelText, RadialOption.Hephaestus);

        if (sellLevelText != null)
            sellLevelText.text = IsTempleSelected() ? "No Sell" : "Sell";
    }

    private void UpdateLevelText(TextMeshProUGUI text, RadialOption option)
    {
        if (text == null) return;

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
        text.text = nextData == null ? "MAX" : "Level " + nextLevel;
    }

    private void UpdateCostText(TextMeshProUGUI text, RadialOption option)
    {
        if (text == null) return;

        bool shouldShow = selectedOption == option;
        text.gameObject.SetActive(shouldShow);

        if (!shouldShow) return;

        if (option == RadialOption.Sell)
        {
            ConstructionData currentData = GetCurrentData();

            if (currentData == null)
            {
                text.text = "";
                return;
            }

            if (currentData.type == ConstructionType.Temple)
            {
                text.text = "No";
                return;
            }

            text.text = GetTotalRefund(currentData).ToString("0");
            return;
        }

        if (IsOptionLocked(option))
        {
            text.text = "Locked";
            return;
        }

        ConstructionData data = GetCurrentData();
        if (data == null)
        {
            text.text = "";
            return;
        }

        GodType god = GetGodFromOption(option);
        int nextLevel = data.level + 1;
        ConstructionData nextData = database.GetData(data.type, god, nextLevel);

        text.text = nextData == null ? "MAX" : GetCostText(nextData);
    }

    private void UpdateSellHoldVisual()
    {
        if (sellHoldFillImage == null)
            return;

        sellHoldFillImage.fillAmount = Mathf.Clamp01(sellTimer / sellHoldTime);
    }

    private bool IsOptionLocked(RadialOption option)
    {
        if (option == RadialOption.Sell) return false;

        ConstructionData currentData = GetCurrentData();
        if (currentData == null || currentData.god == GodType.Base)
            return false;

        return currentData.god != GetGodFromOption(option);
    }

    private bool IsTempleSelected()
    {
        ConstructionData data = GetCurrentData();
        return data != null && data.type == ConstructionType.Temple;
    }

    private void SellSelectedConstruction()
    {
        if (selectedConstruction == null || IsTempleSelected())
            return;

        RefundConstruction(selectedConstruction);

        BaseConstruction baseConstruction = selectedConstruction.GetComponent<BaseConstruction>();
        if (baseConstruction == null) return;

        if (selectedConstruction.group != null && selectedConstruction.group.members.Count > 0)
        {
            List<ConstructionController> membersCopy = new List<ConstructionController>(selectedConstruction.group.members);

            foreach (ConstructionController member in membersCopy)
            {
                if (member == null) continue;

                BaseConstruction memberConstruction = member.GetComponent<BaseConstruction>();
                if (memberConstruction != null)
                    memberConstruction.Die();
            }

            Destroy(selectedConstruction.group.gameObject);
        }
        else
        {
            baseConstruction.Die();
        }

        BuildingManager.Instance.selectedConstruction = null;
        ShowStatus("Construcción vendida");
    }

    private void RefundConstruction(ConstructionController construction)
    {
        BaseConstruction baseConstruction = construction.GetComponent<BaseConstruction>();
        if (baseConstruction == null || baseConstruction.Data == null) return;

        foreach (WillProduction refund in GetRefundList(baseConstruction.Data))
        {
            WillManager.Instance.AddMoney(refund.type, refund.amount * refundPercent);
        }
    }

    private float GetTotalRefund(ConstructionData currentData)
    {
        float total = 0f;

        foreach (WillProduction refund in GetRefundList(currentData))
            total += refund.amount * refundPercent;

        return total;
    }

    private List<WillProduction> GetRefundList(ConstructionData currentData)
    {
        List<WillProduction> refunds = new List<WillProduction>();

        AddCosts(refunds, currentData);

        if (currentData.god != GodType.Base)
        {
            for (int level = 2; level <= currentData.level; level++)
                AddCosts(refunds, database.GetData(currentData.type, currentData.god, level));
        }

        return refunds;
    }

    private void AddCosts(List<WillProduction> refunds, ConstructionData data)
    {
        if (data == null || data.willToPay == null) return;

        foreach (WillProduction cost in data.willToPay)
        {
            WillProduction existing = refunds.Find(x => x.type == cost.type);

            if (existing != null)
                existing.amount += cost.amount;
            else
                refunds.Add(new WillProduction { type = cost.type, amount = cost.amount });
        }
    }

    private ConstructionData GetCurrentData()
    {
        if (selectedConstruction == null) return null;

        BaseConstruction baseConstruction = selectedConstruction.GetComponent<BaseConstruction>();
        return baseConstruction != null ? baseConstruction.Data : null;
    }

    private GodType GetGodFromOption(RadialOption option)
    {
        switch (option)
        {
            case RadialOption.Aphrodite: return GodType.Aphrodite;
            case RadialOption.Ares: return GodType.Ares;
            case RadialOption.Hephaestus: return GodType.Hephaestus;
            default: return GodType.Base;
        }
    }

    private string GetCostText(ConstructionData data)
    {
        if (data.willToPay == null || data.willToPay.Count == 0)
            return "0";

        return data.willToPay[0].amount.ToString("0");
    }

    private void SetHighlight(Image image, bool active)
    {
        if (image != null)
            image.enabled = active;
    }


    private void ShowStatus(string message)
    {
        if (StatusMessageUI.Instance != null)
            StatusMessageUI.Instance.ShowMessage(message);

        Debug.Log(message);
    }
}