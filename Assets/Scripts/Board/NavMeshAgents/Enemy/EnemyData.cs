using UnityEngine;

//Atributes EVERY enemy has
[CreateAssetMenu(menuName = "Enemy/Data")]
public class EnemyData : ScriptableObject
{
    public float resistance;
    public float attackDamage;
    public float actionVelocity;
    public float attackRange;
    public float movementSpeed;
    public GameObject enemyPrefab;
    public float proyectileVelocity;
}