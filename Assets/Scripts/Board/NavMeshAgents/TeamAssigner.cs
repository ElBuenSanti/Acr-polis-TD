using UnityEngine;

public enum Team
{
    Ally,
    Enemy
}

public class TeamAssigner : MonoBehaviour
{
    protected Team team;

    public Team GetTeam()
    {
        return team;
    }

    public void SetTeam(Team newTeam)
    {
        team = newTeam;
    }
}
