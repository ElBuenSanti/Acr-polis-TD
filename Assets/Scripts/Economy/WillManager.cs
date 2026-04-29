using System.Collections.Generic;
using UnityEngine;

public class WillManager : MonoBehaviour
{
    public static WillManager Instance;

    private Dictionary<Will, float> money = new Dictionary<Will, float>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        else
        {
            Destroy(gameObject);
        }
            
        foreach (Will w in System.Enum.GetValues(typeof(Will)))
        {
            if (w == Will.Agape)
            {
                money[w] = 400;
            }
            else
            {
                money[w] = 0;
            }
                
        }
    }

    public void AddMoney(Will type, float amount)
    {
        money[type] += amount; 
        Debug.Log($"+{amount} {type} | Total: {money[type]}");
    }

    public bool SpendMoney(Will type, float amount)
    {
        if (!CanAfford(type, amount))
            return false;

        money[type] -= amount;
        Debug.Log($"-{amount} {type} | Total: {money[type]}");
        return true;
    }

    public bool CanAfford(Will type, float amount)
    {
        return money[type] >= amount;
    }

    //como consulta, util para UI
    public float GetMoney(Will type)
    {
        return money[type];
    }
}
