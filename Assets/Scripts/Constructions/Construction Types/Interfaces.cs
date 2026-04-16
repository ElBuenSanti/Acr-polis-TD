using UnityEngine;

public interface IDamageable
{
    void ReceiveDamage(float damage);
}

public interface IAttacker
{
    void Attack(IDamageable target);
}

public interface ITeam
{
    Team GetTeam();
    void SetTeam(Team newTeam);
}
