////using System.Collections.Generic;
////using UnityEngine;
////using TMPro;

////public class ConstructionDetailsUI : MonoBehaviour
////{
////    [Header("UI")]
////    [SerializeField] private TextMeshProUGUI titleText;
////    [SerializeField] private TextMeshProUGUI costText;

////    [Header("Stats")]
////    [SerializeField] private StatBarUI damageStat;
////    [SerializeField] private StatBarUI rangeStat;
////    [SerializeField] private StatBarUI speedStat;

////    [Header("Database")]
////    [SerializeField] private ConstructionDatabase database;

////    [Header("Stat Max Values")]
////    [SerializeField] private float maxDamage = 100f;
////    [SerializeField] private float maxRange = 20f;
////    [SerializeField] private float maxSpeed = 10f;

////    [Header("Sell")]
////    [SerializeField] private float refundPercent = 0.5f;

////    private void Awake()
////    {
////        if (database == null)
////            database = FindAnyObjectByType<ConstructionDatabase>();
////    }

////    public void ShowBaseDetails(ConstructionController construction)
////    {
////        if (construction == null)
////        {
////            Clear();
////            return;
////        }

////        BaseConstruction baseConstruction = construction.GetComponent<BaseConstruction>();

////        if (baseConstruction == null || baseConstruction.Data == null)
////        {
////            Clear();
////            return;
////        }

////        ConstructionData data = baseConstruction.Data;

////        titleText.text = data.type + " Base";
////        costText.text = "Current stats";

////        damageStat.SetStat("Damage", data.attackDamage, data.attackDamage, maxDamage);
////        rangeStat.SetStat("Range", data.range, data.range, maxRange);
////        speedStat.SetStat("Speed", data.actionVelocity, data.actionVelocity, maxSpeed);
////    }

////    public void ShowDetails(ConstructionController construction, RadialOption option)
////    {
////        if (construction == null || option == RadialOption.None)
////        {
////            ShowBaseDetails(construction);
////            return;
////        }

////        BaseConstruction baseConstruction = construction.GetComponent<BaseConstruction>();

////        if (baseConstruction == null || baseConstruction.Data == null)
////        {
////            Clear();
////            return;
////        }

////        ConstructionData currentData = baseConstruction.Data;

////        if (option == RadialOption.Sell)
////        {
////            ShowSellDetails(currentData);
////            return;
////        }

////        GodType god = GetGodFromOption(option);

////        if (IsLocked(currentData, god))
////        {
////            titleText.text = god + " Locked";
////            costText.text = "Path unavailable";

////            damageStat.SetStat("Damage", currentData.attackDamage, currentData.attackDamage, maxDamage);
////            rangeStat.SetStat("Range", currentData.range, currentData.range, maxRange);
////            speedStat.SetStat("Speed", currentData.actionVelocity, currentData.actionVelocity, maxSpeed);
////            return;
////        }

////        int nextLevel = currentData.level + 1;
////        ConstructionData nextData = database.GetData(currentData.type, god, nextLevel);

////        if (nextData == null)
////        {
////            titleText.text = god + " MAX";
////            costText.text = "No more upgrades";

////            damageStat.SetStat("Damage", currentData.attackDamage, currentData.attackDamage, maxDamage);
////            rangeStat.SetStat("Range", currentData.range, currentData.range, maxRange);
////            speedStat.SetStat("Speed", currentData.actionVelocity, currentData.actionVelocity, maxSpeed);
////            return;
////        }

////        titleText.text = god + " Lv. " + nextLevel;
////        costText.text = GetCostText(nextData);

////        damageStat.SetStat("Damage", currentData.attackDamage, nextData.attackDamage, maxDamage);
////        rangeStat.SetStat("Range", currentData.range, nextData.range, maxRange);
////        speedStat.SetStat("Speed", currentData.actionVelocity, nextData.actionVelocity, maxSpeed);
////    }

////    public void Clear()
////    {
////        if (titleText != null)
////            titleText.text = "Structure Details";

////        if (costText != null)
////            costText.text = "";

////        if (damageStat != null)
////            damageStat.Clear();

////        if (rangeStat != null)
////            rangeStat.Clear();

////        if (speedStat != null)
////            speedStat.Clear();
////    }

////    private void ShowSellDetails(ConstructionData currentData)
////    {
////        if (currentData.type == ConstructionType.Temple)
////        {
////            titleText.text = "Cannot Sell";
////            costText.text = "Temple cannot be sold";

////            damageStat.SetStat("Damage", currentData.attackDamage, currentData.attackDamage, maxDamage);
////            rangeStat.SetStat("Range", currentData.range, currentData.range, maxRange);
////            speedStat.SetStat("Speed", currentData.actionVelocity, currentData.actionVelocity, maxSpeed);

////            return;
////        }

////        titleText.text = "Sell";
////        costText.text = GetRefundText(currentData);

////        damageStat.SetStat("Damage", currentData.attackDamage, 0, maxDamage);
////        rangeStat.SetStat("Range", currentData.range, 0, maxRange);
////        speedStat.SetStat("Speed", currentData.actionVelocity, 0, maxSpeed);
////    }

////    private bool IsLocked(ConstructionData currentData, GodType selectedGod)
////    {
////        if (currentData.god == GodType.Base)
////            return false;

////        return currentData.god != selectedGod;
////    }

////    private GodType GetGodFromOption(RadialOption option)
////    {
////        switch (option)
////        {
////            case RadialOption.Aphrodite:
////                return GodType.Aphrodite;

////            case RadialOption.Ares:
////                return GodType.Ares;

////            case RadialOption.Hephaestus:
////                return GodType.Hephaestus;

////            default:
////                return GodType.Base;
////        }
////    }

////    private string GetCostText(ConstructionData data)
////    {
////        if (data.willToPay == null || data.willToPay.Count == 0)
////            return "Free";

////        WillProduction cost = data.willToPay[0];

////        return cost.amount.ToString("0") + " " + cost.type;
////    }

////    private string GetRefundText(ConstructionData currentData)
////    {
////        List<WillProduction> refunds = GetRefundList(currentData);

////        if (refunds.Count == 0)
////            return "Hold 3s | Refund: 0";

////        string text = "Hold 3s | Refund: ";

////        for (int i = 0; i < refunds.Count; i++)
////        {
////            float amount = refunds[i].amount * refundPercent;
////            text += amount.ToString("0") + " " + refunds[i].type;

////            if (i < refunds.Count - 1)
////                text += ", ";
////        }

////        return text;
////    }

////    private List<WillProduction> GetRefundList(ConstructionData currentData)
////    {
////        List<WillProduction> refunds = new List<WillProduction>();

////        AddCosts(refunds, database.GetData(currentData.type, GodType.Base, 1));

////        if (currentData.god != GodType.Base)
////        {
////            for (int level = 2; level <= currentData.level; level++)
////            {
////                AddCosts(refunds, database.GetData(currentData.type, currentData.god, level));
////            }
////        }

////        return refunds;
////    }

////    private void AddCosts(List<WillProduction> refunds, ConstructionData data)
////    {
////        if (data == null || data.willToPay == null)
////            return;

////        foreach (WillProduction cost in data.willToPay)
////        {
////            WillProduction existing = refunds.Find(x => x.type == cost.type);

////            if (existing != null)
////            {
////                existing.amount += cost.amount;
////            }
////            else
////            {
////                refunds.Add(new WillProduction
////                {
////                    type = cost.type,
////                    amount = cost.amount
////                });
////            }
////        }
////    }
////}


//using System.Collections.Generic;
//using UnityEngine;
//using TMPro;

//public class ConstructionDetailsUI : MonoBehaviour
//{
//    [Header("UI")]
//    [SerializeField] private TextMeshProUGUI titleText;
//    [SerializeField] private TextMeshProUGUI costText;

//    [Header("Stats")]
//    [SerializeField] private StatBarUI damageStat;
//    [SerializeField] private StatBarUI rangeStat;
//    [SerializeField] private StatBarUI speedStat;

//    [Header("Database")]
//    [SerializeField] private ConstructionDatabase database;

//    [Header("Stat Max Values")]
//    [SerializeField] private float maxDamage = 100f;
//    [SerializeField] private float maxRange = 20f;
//    [SerializeField] private float maxSpeed = 10f;

//    [Header("Sell")]
//    [SerializeField] private float refundPercent = 0.75f;

//    private void Awake()
//    {
//        if (database == null)
//            database = FindAnyObjectByType<ConstructionDatabase>();
//    }

//    public void ShowBaseDetails(ConstructionController construction)
//    {
//        if (construction == null)
//        {
//            Clear();
//            return;
//        }

//        BaseConstruction baseConstruction = construction.GetComponent<BaseConstruction>();

//        if (baseConstruction == null || baseConstruction.Data == null)
//        {
//            Clear();
//            return;
//        }

//        ConstructionData data = baseConstruction.Data;

//        titleText.text = data.type + " Base";
//        costText.text = "Current stats";

//        damageStat.SetStat("Damage", data.attackDamage, data.attackDamage, maxDamage);
//        rangeStat.SetStat("Range", data.range, data.range, maxRange);
//        speedStat.SetStat("Speed", data.actionVelocity, data.actionVelocity, maxSpeed);
//    }

//    public void ShowDetails(ConstructionController construction, RadialOption option)
//    {
//        if (construction == null || option == RadialOption.None)
//        {
//            ShowBaseDetails(construction);
//            return;
//        }

//        BaseConstruction baseConstruction = construction.GetComponent<BaseConstruction>();

//        if (baseConstruction == null || baseConstruction.Data == null)
//        {
//            Clear();
//            return;
//        }

//        ConstructionData currentData = baseConstruction.Data;

//        if (option == RadialOption.Sell)
//        {
//            ShowSellDetails(currentData);
//            return;
//        }

//        GodType god = GetGodFromOption(option);

//        if (IsLocked(currentData, god))
//        {
//            titleText.text = god + " Locked";
//            costText.text = "Path unavailable";

//            ShowSameStats(currentData);
//            return;
//        }

//        int nextLevel = currentData.level + 1;
//        ConstructionData nextData = database.GetData(currentData.type, god, nextLevel);

//        if (nextData == null)
//        {
//            titleText.text = god + " MAX";
//            costText.text = "No more upgrades";

//            ShowSameStats(currentData);
//            return;
//        }

//        titleText.text = god + " Lv. " + nextLevel;
//        costText.text = GetCostText(nextData);

//        damageStat.SetStat("Damage", currentData.attackDamage, nextData.attackDamage, maxDamage);
//        rangeStat.SetStat("Range", currentData.range, nextData.range, maxRange);
//        speedStat.SetStat("Speed", currentData.actionVelocity, nextData.actionVelocity, maxSpeed);
//    }

//    public void Clear()
//    {
//        if (titleText != null) titleText.text = "Structure Details";
//        if (costText != null) costText.text = "";

//        if (damageStat != null) damageStat.Clear();
//        if (rangeStat != null) rangeStat.Clear();
//        if (speedStat != null) speedStat.Clear();
//    }

//    private void ShowSellDetails(ConstructionData currentData)
//    {
//        if (currentData.type == ConstructionType.Temple)
//        {
//            titleText.text = "Cannot Sell";
//            costText.text = "Temple cannot be sold";
//            ShowSameStats(currentData);
//            return;
//        }

//        titleText.text = "Sell";
//        costText.text = GetRefundText(currentData);

//        damageStat.SetStat("Damage", currentData.attackDamage, 0, maxDamage);
//        rangeStat.SetStat("Range", currentData.range, 0, maxRange);
//        speedStat.SetStat("Speed", currentData.actionVelocity, 0, maxSpeed);
//    }

//    private void ShowSameStats(ConstructionData data)
//    {
//        damageStat.SetStat("Damage", data.attackDamage, data.attackDamage, maxDamage);
//        rangeStat.SetStat("Range", data.range, data.range, maxRange);
//        speedStat.SetStat("Speed", data.actionVelocity, data.actionVelocity, maxSpeed);
//    }

//    private bool IsLocked(ConstructionData currentData, GodType selectedGod)
//    {
//        if (currentData.god == GodType.Base)
//            return false;

//        return currentData.god != selectedGod;
//    }

//    private GodType GetGodFromOption(RadialOption option)
//    {
//        switch (option)
//        {
//            case RadialOption.Aphrodite: return GodType.Aphrodite;
//            case RadialOption.Ares: return GodType.Ares;
//            case RadialOption.Hephaestus: return GodType.Hephaestus;
//            default: return GodType.Base;
//        }
//    }

//    private string GetCostText(ConstructionData data)
//    {
//        if (data.willToPay == null || data.willToPay.Count == 0)
//            return "Free";

//        WillProduction cost = data.willToPay[0];
//        return cost.amount.ToString("0") + " " + cost.type;
//    }

//    private string GetRefundText(ConstructionData currentData)
//    {
//        List<WillProduction> refunds = GetRefundList(currentData);

//        if (refunds.Count == 0)
//            return "Hold 3s | Refund: 0";

//        string text = "Hold 3s | Refund: ";

//        for (int i = 0; i < refunds.Count; i++)
//        {
//            text += (refunds[i].amount * refundPercent).ToString("0") + " " + refunds[i].type;

//            if (i < refunds.Count - 1)
//                text += ", ";
//        }

//        return text;
//    }

//    private List<WillProduction> GetRefundList(ConstructionData currentData)
//    {
//        List<WillProduction> refunds = new List<WillProduction>();

//        AddCosts(refunds, currentData);

//        if (currentData.god != GodType.Base)
//        {
//            for (int level = 2; level <= currentData.level; level++)
//                AddCosts(refunds, database.GetData(currentData.type, currentData.god, level));
//        }

//        return refunds;
//    }

//    private void AddCosts(List<WillProduction> refunds, ConstructionData data)
//    {
//        if (data == null || data.willToPay == null) return;

//        foreach (WillProduction cost in data.willToPay)
//        {
//            WillProduction existing = refunds.Find(x => x.type == cost.type);

//            if (existing != null)
//                existing.amount += cost.amount;
//            else
//                refunds.Add(new WillProduction { type = cost.type, amount = cost.amount });
//        }
//    }
//}


using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConstructionDetailsUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI costText;

    [Header("Stats")]
    [SerializeField] private StatBarUI resistanceStat;
    [SerializeField] private StatBarUI aditamentResistanceStat;
    [SerializeField] private StatBarUI damageStat;
    [SerializeField] private StatBarUI actionSpeedStat;
    [SerializeField] private StatBarUI movementSpeedStat;
    [SerializeField] private StatBarUI rangeStat;
    [SerializeField] private StatBarUI projectileSpeedStat;

    [Header("Database")]
    [SerializeField] private ConstructionDatabase database;

    [Header("Max Values")]
    [SerializeField] private float maxResistance = 1000f;
    [SerializeField] private float maxAditamentResistance = 500f;
    [SerializeField] private float maxDamage = 500f;
    [SerializeField] private float maxActionSpeed = 10f;
    [SerializeField] private float maxMovementSpeed = 10f;
    [SerializeField] private float maxRange = 20f;
    [SerializeField] private float maxProjectileSpeed = 20f;

    [Header("Sell")]
    [SerializeField] private float refundPercent = 0.75f;

    private void Awake()
    {
        if (database == null)
            database = FindAnyObjectByType<ConstructionDatabase>();
    }

    public void ShowBaseDetails(ConstructionController construction)
    {
        if (construction == null)
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

        ConstructionData data = baseConstruction.Data;

        if (titleText != null)
            titleText.text = data.type + " Base";

        if (costText != null)
            costText.text = "Current stats";

        ShowStats(data, data);
    }

    public void ShowDetails(ConstructionController construction, RadialOption option)
    {
        if (construction == null || option == RadialOption.None)
        {
            ShowBaseDetails(construction);
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
            if (titleText != null)
                titleText.text = god + " Locked";

            if (costText != null)
                costText.text = "Path unavailable";

            ShowStats(currentData, currentData);
            return;
        }

        int nextLevel = currentData.level + 1;
        ConstructionData nextData = database.GetData(currentData.type, god, nextLevel);

        if (nextData == null)
        {
            if (titleText != null)
                titleText.text = god + " MAX";

            if (costText != null)
                costText.text = "No more upgrades";

            ShowStats(currentData, currentData);
            return;
        }

        if (titleText != null)
            titleText.text = god + " Lv. " + nextLevel;

        if (costText != null)
            costText.text = GetCostText(nextData);

        ShowStats(currentData, nextData);
    }

    private void ShowSellDetails(ConstructionData currentData)
    {
        if (currentData.type == ConstructionType.Temple)
        {
            if (titleText != null)
                titleText.text = "Cannot Sell";

            if (costText != null)
                costText.text = "Temple cannot be sold";

            ShowStats(currentData, currentData);
            return;
        }

        if (titleText != null)
            titleText.text = "Sell";

        if (costText != null)
            costText.text = GetRefundText(currentData);

        ShowStats(currentData, currentData);
    }

    private void ShowStats(ConstructionData currentData, ConstructionData nextData)
    {
        resistanceStat.SetStat("Resistance", currentData.resistance, nextData.resistance, maxResistance);
        aditamentResistanceStat.SetStat("Aditament", currentData.aditamentResistance, nextData.aditamentResistance, maxAditamentResistance);
        damageStat.SetStat("Damage", currentData.attackDamage, nextData.attackDamage, maxDamage);
        actionSpeedStat.SetStat("Action Speed", currentData.actionVelocity, nextData.actionVelocity, maxActionSpeed);
        movementSpeedStat.SetStat("Move Speed", currentData.movementSpeed, nextData.movementSpeed, maxMovementSpeed);
        rangeStat.SetStat("Range", currentData.range, nextData.range, maxRange);
        projectileSpeedStat.SetStat("Projectile", currentData.proyectileVelocity, nextData.proyectileVelocity, maxProjectileSpeed);
    }

    public void Clear()
    {
        if (titleText != null)
            titleText.text = "Structure Details";

        if (costText != null)
            costText.text = "";

        resistanceStat.Clear();
        aditamentResistanceStat.Clear();
        damageStat.Clear();
        actionSpeedStat.Clear();
        movementSpeedStat.Clear();
        rangeStat.Clear();
        projectileSpeedStat.Clear();
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

        string text = "";

        for (int i = 0; i < data.willToPay.Count; i++)
        {
            WillProduction cost = data.willToPay[i];

            text += cost.amount.ToString("0") + " " + cost.type;

            if (i < data.willToPay.Count - 1)
                text += " / ";
        }

        return text;
    }

    private string GetRefundText(ConstructionData currentData)
    {
        List<WillProduction> refunds = GetRefundList(currentData);

        if (refunds.Count == 0)
            return "Refund: 0";

        string text = "Refund: ";

        for (int i = 0; i < refunds.Count; i++)
        {
            text += (refunds[i].amount * refundPercent).ToString("0") + " " + refunds[i].type;

            if (i < refunds.Count - 1)
                text += " / ";
        }

        return text;
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
        if (data == null || data.willToPay == null)
            return;

        foreach (WillProduction cost in data.willToPay)
        {
            WillProduction existing = refunds.Find(x => x.type == cost.type);

            if (existing != null)
                existing.amount += cost.amount;
            else
                refunds.Add(new WillProduction { type = cost.type, amount = cost.amount });
        }
    }
}