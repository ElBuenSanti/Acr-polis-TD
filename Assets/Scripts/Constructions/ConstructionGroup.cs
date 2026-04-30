using System.Collections.Generic;
using UnityEngine;

public class ConstructionGroup : MonoBehaviour
{
    public List<ConstructionController> members = new List<ConstructionController>();
    public List<Tile> tiles = new List<Tile>();

    //Creates a group that has access or "contact" with the neighbor constructions, for example, walls.
    public void Add(ConstructionController c)
    {
        if (!members.Contains(c))
            members.Add(c);
    }
}
