using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WillProduction
{
    public Will type;
    public float amount;
}

[CreateAssetMenu(menuName = "Construction/Data")]
public class ConstructionData : ScriptableObject
{
    public ConstructionType type;
    public GodType god;
    
    public int level;

    public float resistance; //3
    public float aditamentResistance; //Defense, Barrack
    public float attackDamage; //Defense, Barrack

    public List<WillProduction> willObtaied;
    public float actionVelocity; //3

    public float movementSpeed; //Barrack
    public float range; //Defense
    public float proyectileVelocity; //defense

    public List<WillProduction> willToPay;
    public List<WillProduction> bonusWillToPay;

    public GameObject prefab; //3
}