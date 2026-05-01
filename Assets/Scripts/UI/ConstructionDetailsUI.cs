using UnityEngine;
using TMPro;

public class ConstructionDetailsUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI costText;

    [Header("Stats")]
    [SerializeField] private StatBarUI damageStat;
    [SerializeField] private StatBarUI rangeStat;
    [SerializeField] private StatBarUI speedStat;

    [Header("Database")]
    [SerializeField] private ConstructionDatabase database;

    [Header("Stat Max Values")]
    [SerializeField] private float maxDamage = 100f;
    [SerializeField] private float maxRange = 20f;
    [SerializeField] private float maxSpeed = 10f;

    private void Awake()
    {
        if (database == null)
            database = FindAnyObjectByType<ConstructionDatabase>();
    }

    public void ShowDetails(ConstructionController construction, RadialOption option)
    {
        if (construction == null || option == RadialOption.None)
        {
            Clear();
            return;
        }

        BaseConstruction baseConstruction = construction.GetComponent<BaseConstruction>();

        if (baseConstruction == null || baseConstruction.Data == null)
        {
            Clear();
            return;
        }

        ConstructionData currentData = baseConstruction.Data;

        if (option == RadialOption.Sell)
        {
            ShowSellDetails(currentData);
            return;
        }

        GodType god = GetGodFromOption(option);

        if (IsLocked(currentData, god))
        {
            titleText.text = god + " Locked";
            costText.text = "Path unavailable";

            damageStat.SetStat("Damage", currentData.attackDamage, currentData.attackDamage, maxDamage);
            rangeStat.SetStat("Range", currentData.range, currentData.range, maxRange);
            speedStat.SetStat("Speed", currentData.actionVelocity, currentData.actionVelocity, maxSpeed);
            return;
        }

        int nextLevel = currentData.level + 1;
        ConstructionData nextData = database.GetData(currentData.type, god, nextLevel);

        if (nextData == null)
        {
            titleText.text = god + " MAX";
            costText.text = "No more upgrades";

            damageStat.SetStat("Damage", currentData.attackDamage, currentData.attackDamage, maxDamage);
            rangeStat.SetStat("Range", currentData.range, currentData.range, maxRange);
            speedStat.SetStat("Speed", currentData.actionVelocity, currentData.actionVelocity, maxSpeed);
            return;
        }

        titleText.text = god + " Lv. " + nextLevel;
        costText.text = GetCostText(nextData);

        damageStat.SetStat("Damage", currentData.attackDamage, nextData.attackDamage, maxDamage);
        rangeStat.SetStat("Range", currentData.range, nextData.range, maxRange);
        speedStat.SetStat("Speed", currentData.actionVelocity, nextData.actionVelocity, maxSpeed);
    }

    public void Clear()
    {
        if (titleText != null)
            titleText.text = "Structure Details";

        if (costText != null)
            costText.text = "";

        if (damageStat != null)
            damageStat.Clear();

        if (rangeStat != null)
            rangeStat.Clear();

        if (speedStat != null)
            speedStat.Clear();
    }

    private void ShowSellDetails(ConstructionData currentData)
    {
        titleText.text = "Sell";
        costText.text = "Recover resources";

        damageStat.SetStat("Damage", currentData.attackDamage, 0, maxDamage);
        rangeStat.SetStat("Range", currentData.range, 0, maxRange);
        speedStat.SetStat("Speed", currentData.actionVelocity, 0, maxSpeed);
    }

    private bool IsLocked(ConstructionData currentData, GodType selectedGod)
    {
        if (currentData.god == GodType.Base)
            return false;

        return currentData.god != selectedGod;
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
            return "Free";

        WillProduction cost = data.willToPay[0];

        return cost.amount.ToString("0") + " " + cost.type;
    }
}