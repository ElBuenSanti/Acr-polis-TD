using System.Collections.Generic;
using UnityEngine;



[System.Serializable]
public class WillProduction
{
    public Will type;
    public float amount;
}


//Definition of atributes that EVERY construction has
[CreateAssetMenu(menuName = "Construction/Data")]
public class ConstructionData : ScriptableObject
{
    public ConstructionType type;
    public GodType god;
    
    public int level;

    public float resistance; 
    public float aditamentResistance; 
    public float attackDamage; 

    public List<WillProduction> willObtaied;
    public float actionVelocity; 

    public float movementSpeed; 
    public float range; 
    public float proyectileVelocity; 

    public List<WillProduction> willToPay;
    public List<WillProduction> bonusWillToPay;

    public GameObject prefab; 
}