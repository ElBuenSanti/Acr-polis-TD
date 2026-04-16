using UnityEngine;

[CreateAssetMenu(menuName = "Construction/Data")]
public class ConstructionData : ScriptableObject
{
    public ConstructionType type;
    public GodType god;
    public WillCurrency will;
    public int level;

    public float resistance; //3
    public float aditamentResistance; //Defense, Barrack
    public float attackDamage; //Defense, Barrack
    public float resourceRate; //3
    public float actionVelocity; //3
    public float movementSpeed; //Barrack
    public float range; //Defense
    public float proyectileVelocity; //defense
    public float cost; //3
    public GameObject prefab; //3
}