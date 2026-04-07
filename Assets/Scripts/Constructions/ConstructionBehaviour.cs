using UnityEngine;

public class ConstructionBehaviour : TeamAssigner, IConstructable
{
    public virtual void Start()
    {
        SetTeam(Team.Ally);
    }
    public void ReceiveDamage(float damage)
    {

    }
    public void Recover() { }
    public void Upgrade() { }
}
