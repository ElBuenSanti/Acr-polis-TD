using UnityEngine;

//IGNORALO!!!! ESTE NO ES NADA, PERO NO LO HE BORRADO AÚN
public class ConstructionBehaviour : TeamAssigner
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
