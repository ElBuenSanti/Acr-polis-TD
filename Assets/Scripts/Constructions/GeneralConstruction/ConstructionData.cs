using UnityEngine;

[CreateAssetMenu(menuName = "Construction/Data")]
public class ConstructionData : ScriptableObject
{
    public ConstructionType type;
    public GodType god;
    public WillCurrency will;
    public int level;

    public float resistance;
    public float damage;
    public float resourceRate;
    public float actionVelocity;
    public float Range;
    public float proyectileVelocity;
    public float cost;
    public GameObject prefab;
}